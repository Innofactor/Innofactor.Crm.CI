using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Common.Interfaces;
using Cinteros.Crm.Utils.Misc;
using Cinteros.Crm.Utils.Shuffle.Types;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/// <summary>Common namespace for Cinteros Shuffle functionality</summary>
namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>
    /// Class for Shuffling functionality
    /// </summary>
    public partial class Shuffler
    {
        #region Public Fields

        /// <summary>Dictionary with solutions and versions read from CRM using the /A:V directive.</summary>
        public Dictionary<string, Version> ExistingSolutionVersions = null;

        #endregion Public Fields

        #region Private Fields

        private readonly IServicable crmsvc;
        private readonly ILoggable log;
        private XmlDocument definition;
        private ShuffleDefinition shuffledefinition;
        private string definitionpath;
        private Dictionary<Guid, Guid> guidmap = null;
        private bool stoponerror = false;
        private int timeout = 120;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>General constructor for the Shuffler class</summary>
        /// <param name="container"></param>
        public Shuffler(IContainable container)
        {
            this.crmsvc = container.Service;
            this.log = container.Logger;
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>Event handler to receive notifications in consuming code</summary>
        /// <param name="sender"></param>
        /// <param name="a">Arguments passed in notification</param>
        public delegate void ShuffleEventHandler(object sender, ShuffleEventArgs a);

        #endregion Public Delegates

        #region Public Events

        public event EventHandler<ShuffleEventArgs> RaiseShuffleEvent;

        #endregion Public Events

        #region Public Properties

        /// <summary>Shuffle Definition to be used</summary>
        public XmlDocument Definition
        {
            get { return definition; }
            set
            {
                var result = ShuffleHelper.ValidateDefinitionXml(value, log);
                if (!string.IsNullOrEmpty(result))
                {
                    SendLine(result);
                }
                definition = value;
            }
        }

        public ShuffleDefinition ShuffleDefinition
        {
            get
            {
                if (shuffledefinition == null && definition != null)
                {
                    var serializer = new XmlSerializer(typeof(ShuffleDefinition));
                    using (TextReader reader = new StringReader(definition.OuterXml))
                    {
                        shuffledefinition = serializer.Deserialize(reader) as ShuffleDefinition;
                    }
                }
                return shuffledefinition;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="container"></param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container)
        {
            return QuickExport(Definition, Type, Delimeter, ShuffleEventHandler, container, null);
        }

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="container"></param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container, string defpath)
        {
            return QuickExport(Definition, Type, Delimeter, ShuffleEventHandler, container, defpath, false);
        }

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="container"></param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container, string defpath, bool clearRemainingShuffleVars)
        {
            container.Logger.StartSection("QuickExport");
            Shuffler shuffle = new Shuffler(container);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            ShuffleBlocks blocks = shuffle.ExportFromCRM();
            XmlDocument result = shuffle.Serialize(blocks, Type, Delimeter);
            container.Logger.EndSection();
            return result;
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="container"></param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container)
        {
            return QuickImport(Definition, Data, ShuffleEventHandler, container, null);
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="container"></param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container, string defpath)
        {
            return QuickImport(Definition, Data, ShuffleEventHandler, container, defpath, false);
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="container"></param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, IContainable container, string defpath, bool clearRemainingShuffleVars)
        {
            container.Logger.StartSection("QuickImport");
            Shuffler shuffle = new Shuffler(container);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            ShuffleBlocks blocks = shuffle.Deserialize(Data);
            Tuple<int, int, int, int, int, EntityReferenceCollection> result = shuffle.ImportToCRM(blocks);
            container.Logger.EndSection();
            return result;
        }

        /// <summary>
        /// Deserialize xml/string to blocks with entities
        /// </summary>
        /// <param name="serialized"></param>
        /// <returns>Optional, only required for SerializationType: Text</returns>
        public ShuffleBlocks Deserialize(XmlDocument serialized)
        {
            log.StartSection("Deserialize");
            ShuffleBlocks result = new ShuffleBlocks();
            if (serialized != null)
            {
                XmlNode root = CintXML.FindChild(serialized, "ShuffleData");
                string sertype = CintXML.GetAttribute(root, "Type");
                SendLine("Deserialize from {0}", sertype);
                if (sertype == SerializationType.Full.ToString() ||
                    sertype == SerializationType.Simple.ToString() ||
                    sertype == SerializationType.SimpleNoId.ToString() ||
                    sertype == SerializationType.SimpleWithValue.ToString() ||
                    sertype == SerializationType.Explicit.ToString())
                {
                    foreach (XmlNode xBlock in root.ChildNodes)
                    {
                        if (xBlock.NodeType == XmlNodeType.Element && xBlock.Name == "Block" && xBlock.ChildNodes.Count == 1)
                        {
                            string name = CintXML.GetAttribute(xBlock, "Name");
                            XmlDocument xml = new XmlDocument();
                            xml.AppendChild(xml.ImportNode(xBlock.ChildNodes[0], true));
                            CintDynEntityCollection cEntities = new CintDynEntityCollection(xml, crmsvc, log);
                            SendLine("Block {0}: {1} records", name, cEntities.Count);
                            result.Add(name, cEntities);
                        }
                    }
                }
                else if (sertype == SerializationType.Text.ToString())
                {
                    string strdelimeter = CintXML.GetAttribute(root, "Delimeter");
                    char delimeter = strdelimeter.Length == 1 ? strdelimeter[0] : '\t';
                    XmlNode xText = CintXML.FindChild(root, "Text");
                    StringReader reader = new StringReader(xText.InnerText);
                    int line = 0;
                    string name = "";
                    StringBuilder serializedblock = null;
                    string current = reader.ReadLine();
                    while (current != null)
                    {
                        log.Log("Line {0:000}: {1}", line, current);
                        if (current.StartsWith("<<<") && current.Contains(">>>"))
                        {
                            log.Log("Block start");
                            if (!string.IsNullOrWhiteSpace(name) && serializedblock != null)
                            {
                                CintDynEntityCollection cEntities = new CintDynEntityCollection(serializedblock.ToString(), delimeter, crmsvc, log);
                                result.Add(name, cEntities);
                                SendLine("Block {0}: {1} records", name, cEntities.Count);
                            }
                            name = current.Substring(3);
                            name = name.Substring(0, name.IndexOf(">>>", StringComparison.Ordinal));
                            serializedblock = new StringBuilder();
                        }
                        else
                        {
                            serializedblock.AppendLine(current);
                        }
                        current = reader.ReadLine();
                        line++;
                    }
                    if (!string.IsNullOrWhiteSpace(serializedblock.ToString()))
                    {
                        CintDynEntityCollection cEntities = new CintDynEntityCollection(serializedblock.ToString(), delimeter, crmsvc, log);
                        result.Add(name, cEntities);
                        SendLine("Block {0}: {1} records", name, cEntities.Count);
                    }
                }
            }
            log.EndSection();
            return result;
        }

        /// <summary>
        /// Export entities from CRM to dictionary of blocks with entities
        /// </summary>
        /// <returns>Blocks with exported entities</returns>
        public ShuffleBlocks ExportFromCRM()
        {
            log.StartSection("ExportFromCRM");
            if (definition == null)
            {
                throw new ArgumentNullException("Definition", "Shuffle definition must be specified to export data");
            }
            ShuffleBlocks blocks = new ShuffleBlocks();
            ExistingSolutionVersions = null;
            if (ShuffleDefinition.Blocks?.Items?.Length > 0)
            {
                stoponerror = ShuffleDefinition.StopOnError;
                timeout = ShuffleDefinition.TimeoutSpecified ? ShuffleDefinition.Timeout : -1;
                double savedtimeout = -1;
                if (timeout > -1)
                {
                    savedtimeout = SetTimeout();
                }

                int totalBlocks = ShuffleDefinition.Blocks.Items.Length;
                int currentBlock = 0;
                foreach (var block in ShuffleDefinition.Blocks.Items)
                {
                    currentBlock++;
                    SendStatus(totalBlocks, currentBlock, -1, -1);
                    if (block is DataBlock datablock)
                    {
                        var cExported = ExportDataBlock(blocks, datablock);
                        var name = datablock.Name;
                        if (cExported != null)
                        {
                            if (blocks.ContainsKey(name))
                            {
                                SendLine("Block already added: {0}", name);
                            }
                            else
                            {
                                blocks.Add(name, cExported);
                            }
                        }
                    }
                    else if (block is SolutionBlock solutionblock)
                    {
                        if (ExistingSolutionVersions == null)
                        {
                            GetCurrentVersions();
                        }
                        ExportSolutionBlock(solutionblock);
                    }
                }
                SendStatus(0, 0, 0, 0);
                if (savedtimeout > -1)
                {
                    ResetTimeout(savedtimeout);
                }
            }
            log.EndSection();
            return blocks;
        }

        /// <summary>
        /// Import entities to CRM from dictionary of blocks
        /// </summary>
        /// <param name="blocks">Blocks with entities to import</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records</returns>
        public Tuple<int, int, int, int, int, EntityReferenceCollection> ImportToCRM(ShuffleBlocks blocks)
        {
            log.StartSection("ImportToCRM");
            if (definition == null)
            {
                throw new ArgumentNullException("Definition", "Shuffle definition must be specified to import data");
            }

            int created = 0;
            int updated = 0;
            int skipped = 0;
            int deleted = 0;
            int failed = 0;
            EntityReferenceCollection references = new EntityReferenceCollection();

            XmlNode xRoot = CintXML.FindChild(definition, "ShuffleDefinition");
            XmlNode xBlocks = CintXML.FindChild(xRoot, "Blocks");
            if (xBlocks != null)
            {
                guidmap = new Dictionary<Guid, Guid>();
                stoponerror = CintXML.GetBoolAttribute(xRoot, "StopOnError", false);
                timeout = CintXML.GetIntAttribute(xRoot, "Timeout", -1);
                double savedtimeout = -1;
                if (timeout > -1)
                {
                    savedtimeout = SetTimeout();
                }

                int totalBlocks = xBlocks.ChildNodes.Count;
                int currentBlock = 0;
                foreach (XmlNode xBlock in xBlocks.ChildNodes)
                {
                    currentBlock++;
                    SendStatus(totalBlocks, currentBlock, -1, -1);
                    if (xBlock.NodeType == XmlNodeType.Element)
                    {
                        switch (xBlock.Name)
                        {
                            case "DataBlock":
                                string name = CintXML.GetAttribute(xBlock, "Name");
                                if (!blocks.ContainsKey(name))
                                {
                                    blocks.Add(name, new CintDynEntityCollection());
                                }
                                Tuple<int, int, int, int, int, EntityReferenceCollection> dataresult = ImportDataBlock(xBlock, blocks[name]);
                                created += dataresult.Item1;
                                updated += dataresult.Item2;
                                skipped += dataresult.Item3;
                                deleted += dataresult.Item4;
                                failed += dataresult.Item5;
                                references.AddRange(dataresult.Item6);
                                break;

                            case "SolutionBlock":
                                var solutionresult = ImportSolutionBlock(xBlock);
                                switch (solutionresult)
                                {
                                    case ItemImportResult.Created: created++; break;
                                    case ItemImportResult.Updated: updated++; break;
                                    case ItemImportResult.Skipped: skipped++; break;
                                    case ItemImportResult.Failed: failed++; break;
                                }
                                break;
                        }
                    }
                }
                SendStatus(0, 0, 0, 0);
                if (savedtimeout > -1)
                {
                    ResetTimeout(savedtimeout);
                }
            }
            log.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

        /// <summary>
        /// Serialize blocks with entities with given serialization type
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="type"></param>
        /// <param name="delimeter">Optional, only required for SerializationType: Text</param>
        /// <returns></returns>
        public XmlDocument Serialize(ShuffleBlocks blocks, SerializationType type, char delimeter)
        {
            log.StartSection("Serialize");
            XmlDocument xml = null;
            if (blocks.Count > 0)
            {
                SendLine("Serializing {0} blocks with type {1}", blocks.Count, type);
                xml = new XmlDocument();
                XmlNode root = xml.CreateElement("ShuffleData");
                xml.AppendChild(root);
                CintXML.AppendAttribute(root, "Type", type.ToString());
                CintXML.AppendAttribute(root, "ExportTime", DateTime.Now.ToString("s"));
                switch (type)
                {
                    case SerializationType.Full:
                    case SerializationType.Simple:
                    case SerializationType.SimpleWithValue:
                    case SerializationType.SimpleNoId:
                    case SerializationType.Explicit:
                        foreach (string block in blocks.Keys)
                        {
                            SendLine("Serializing {0} records in block {1}", blocks[block].Count, block);
                            XmlNode xBlock = xml.CreateElement("Block");
                            root.AppendChild(xBlock);
                            CintXML.AppendAttribute(xBlock, "Name", block);
                            CintXML.AppendAttribute(xBlock, "Count", blocks[block].Count.ToString());
                            XmlDocument xSerialized = blocks[block].Serialize((SerializationStyle)type);
                            xBlock.AppendChild(xml.ImportNode(xSerialized.ChildNodes[0], true));
                        }
                        break;

                    case SerializationType.Text:
                        CintXML.AppendAttribute(root, "Delimeter", delimeter.ToString());
                        StringBuilder text = new StringBuilder();
                        foreach (string block in blocks.Keys)
                        {
                            SendLine("Serializing {0} records in block {1}", blocks[block].Count, block);
                            text.AppendLine("<<<" + block + ">>>");
                            string serializedblock = blocks[block].ToTextFile(delimeter);
                            text.Append(serializedblock);
                        }
                        CintXML.AddCDATANode(root, "Text", text.ToString());
                        break;
                }
            }
            log.EndSection();
            return xml;
        }

        private double SetTimeout()
        {
            SendLine("Setting timeout: {0} minutes", timeout);
            double savedtimeout = -1;
            if (crmsvc.Service is OrganizationServiceProxy orgsvcpxy)
            {
                savedtimeout = orgsvcpxy.Timeout.TotalMinutes;
                orgsvcpxy.Timeout = new TimeSpan(0, timeout, 0);
            }
            else if (crmsvc.Service is CrmServiceClient svcclient)
            {
                savedtimeout = svcclient.OrganizationServiceProxy.Timeout.TotalMinutes;
                svcclient.OrganizationServiceProxy.Timeout = new TimeSpan(0, timeout, 0);
            }
            return savedtimeout;
        }

        private void ResetTimeout(double savedtimeout)
        {
            if (crmsvc.Service is OrganizationServiceProxy orgsvcpxy)
            {
                orgsvcpxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
            }
            else if (crmsvc.Service is CrmServiceClient svcclient)
            {
                svcclient.OrganizationServiceProxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary></summary>
        /// <param name="e"></param>
        /// <remarks>Wrap event invocations inside a protected virtual method
        /// to allow derived classes to override the event invocation behavior
        /// Exempel från: http://msdn.microsoft.com/en-us/library/w369ty8x.aspx </remarks>
        protected virtual void OnRaiseShuffleEvent(ShuffleEventArgs e)
        {
            EventHandler<ShuffleEventArgs> handler = RaiseShuffleEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private CintDynEntityCollection GetExistingSolutions()
        {
            CintDynEntityCollection cSolutions = CintDynEntity.RetrieveMultiple(crmsvc, "solution",
                new string[] { "isvisible" },
                new object[] { true },
                new ColumnSet("solutionid", "uniquename", "friendlyname", "version", "ismanaged"), log);
            return cSolutions;
        }

        private void SendLine()
        {
            SendText("\n", false);
        }

        private void SendLine(string msg, params object[] args)
        {
            SendText(msg, false, args);
            SendLine();
        }

        private void SendLineUpdate(string msg, params object[] args)
        {
            SendText(msg, true, args);
            SendLine();
        }

        private void SendStatus(int totalBlocks, int currentBlock, int blockRecords, int currentRecord)
        {
            OnRaiseShuffleEvent(new ShuffleEventArgs(null, totalBlocks, currentBlock, blockRecords, currentRecord, false));
        }

        private void SendStatus(string block, string record)
        {
            OnRaiseShuffleEvent(new ShuffleEventArgs(null, block, record));
        }

        private void SendText(string msg, bool replacelast, params object[] args)
        {
            SendText(msg, -1, -1, -1, -1, replacelast, args);
        }

        private void SendText(string msg, int totalBlocks, int currentBlock, int blockRecords, int currentRecord, bool replacelast, params object[] args)
        {
            if (msg != null)
            {
                msg = string.Format(msg, args);
                if (msg.Length > 1)
                {
                    log.Log(msg, args);
                }
            }
            OnRaiseShuffleEvent(new ShuffleEventArgs(msg, totalBlocks, currentBlock, blockRecords, currentRecord, replacelast));
        }

        #endregion Private Methods
    }
}
namespace Cinteros.Crm.Utils.Shuffle
{
    using Cinteros.Crm.Utils.Shuffle.Types;
    using Innofactor.Xrm.Utils.Common.Extensions;
    using Innofactor.Xrm.Utils.Common.Interfaces;
    using Innofactor.Xrm.Utils.Common.Misc;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

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

        private readonly IExecutionContainer container;
        private XmlDocument definition;

        private string definitionpath;
        private Dictionary<Guid, Guid> guidmap = null;
        private ShuffleDefinition shuffledefinition;
        private bool stoponerror = false;
        private int timeout = 120;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>General constructor for the Shuffler class</summary>
        /// <param name="container"></param>
        public Shuffler(IExecutionContainer container)
        {
            this.container = container;
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>Event handler to receive notifications in consuming code</summary>
        /// <param name="sender"></param>
        /// <param name="a">Arguments passed in notification</param>
        public delegate void ShuffleEventHandler(object sender, ShuffleEventArgs a);

        #endregion Public Delegates

        #region Public Events
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ShuffleEventArgs> RaiseShuffleEvent;

        #endregion Public Events

        #region Public Properties

        /// <summary>Shuffle Definition to be used</summary>
        public XmlDocument Definition
        {
            get { return definition; }
            set
            {
                ShuffleHelper.ValidateDefinitionXml(value);
                definition = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(IExecutionContainer container, XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler) =>
            QuickExport(container, Definition, Type, Delimeter, ShuffleEventHandler, null);

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(IExecutionContainer container, XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, string defpath) =>
            QuickExport(container, Definition, Type, Delimeter, ShuffleEventHandler, defpath, false);

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(IExecutionContainer container, XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, string defpath, bool clearRemainingShuffleVars)
        {
            container.StartSection("QuickExport");
            var shuffle = new Shuffler(container);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            var blocks = shuffle.ExportFromCRM(container);
            var result = shuffle.Serialize(container, blocks, Type, Delimeter);
            container.EndSection();
            return result;
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(IExecutionContainer container, XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler) =>
            QuickImport(container, Definition, Data, ShuffleEventHandler, null);

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(IExecutionContainer container, XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, string defpath) =>
            QuickImport(container, Definition, Data, ShuffleEventHandler, defpath, false);

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="container"></param>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(IExecutionContainer container, XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, string defpath, bool clearRemainingShuffleVars)
        {
            container.StartSection("QuickImport");
            var shuffle = new Shuffler(container);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            var blocks = shuffle.Deserialize(container, Data);
            var result = shuffle.ImportToCRM(container, blocks);
            container.EndSection();
            return result;
        }

        /// <summary>
        /// Deserialize xml/string to blocks with entities
        /// </summary>
        /// <param name="container"></param>
        /// <param name="serialized"></param>
        /// <returns>Optional, only required for SerializationType: Text</returns>
        public ShuffleBlocks Deserialize(IExecutionContainer container, XmlDocument serialized)
        {
            container.StartSection("Deserialize");
            var result = new ShuffleBlocks();
            if (serialized != null)
            {
                var root = XML.FindChild(serialized, "ShuffleData");
                var sertype = XML.GetAttribute(root, "Type");
                SendLine(container, $"Deserialize from {sertype}");
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
                            var name = XML.GetAttribute(xBlock, "Name");
                            var xml = new XmlDocument();
                            xml.AppendChild(xml.ImportNode(xBlock.ChildNodes[0], true));
                            var cEntities = container.CreateEntityCollection(xml);
                            SendLine(container, $"Block {name}: {cEntities.Count()} records");
                            result.Add(name, cEntities);
                        }
                    }
                }
                else if (sertype == SerializationType.Text.ToString())
                {
                    var strdelimeter = XML.GetAttribute(root, "Delimeter");
                    var delimeter = strdelimeter.Length == 1 ? strdelimeter[0] : '\t';
                    var xText = XML.FindChild(root, "Text");
                    var reader = new StringReader(xText.InnerText);
                    var line = 0;
                    var name = "";
                    StringBuilder serializedblock = null;
                    var current = reader.ReadLine();
                    while (current != null)
                    {
                        container.Log("Line {0:000}: {1}", line, current);
                        if (current.StartsWith("<<<") && current.Contains(">>>"))
                        {
                            container.Log("Block start");
                            if (!string.IsNullOrWhiteSpace(name) && serializedblock != null)
                            {
                                var cEntities = container.CreateEntityCollection(serializedblock.ToString(), delimeter);
                                result.Add(name, cEntities);
                                SendLine(container, $"Block {name}: {cEntities.Count()} records");
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
                        var cEntities = container.CreateEntityCollection(serializedblock.ToString(), delimeter);
                        result.Add(name, cEntities);
                        SendLine(container, $"Block {name}: {cEntities.Count()} records");
                    }
                }
            }
            container.EndSection();
            return result;
        }

        /// <summary>
        /// Export entities from CRM to dictionary of blocks with entities
        /// </summary>
        /// <returns>Blocks with exported entities</returns>
        public ShuffleBlocks ExportFromCRM(IExecutionContainer container)
        {
            container.StartSection("ExportFromCRM");
            if (definition == null)
            {
                throw new ArgumentNullException("Definition", "Shuffle definition must be specified to export data");
            }
            var blocks = new ShuffleBlocks();
            ExistingSolutionVersions = null;
            if (ShuffleDefinition.Blocks.Items.Any(b => (b is DataBlock data && data.Export != null) || b is SolutionBlock sol && sol.Export != null))
            {
                stoponerror = ShuffleDefinition.StopOnError;
                timeout = ShuffleDefinition.TimeoutSpecified ? ShuffleDefinition.Timeout : -1;
                //double savedtimeout = -1;
                //if (timeout > -1)
                //{
                //    savedtimeout = SetTimeout();
                //}

                var totalBlocks = ShuffleDefinition.Blocks.Items.Length;
                var currentBlock = 0;
                foreach (var block in ShuffleDefinition.Blocks.Items)
                {
                    currentBlock++;
                    SendStatus(totalBlocks, currentBlock, -1, -1);
                    if (block is DataBlock datablock)
                    {
                        var cExported = ExportDataBlock(container, blocks, datablock);
                        var name = datablock.Name;
                        if (cExported != null)
                        {
                            if (blocks.ContainsKey(name))
                            {
                                SendLine(container, $"Block already added: {name}");
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
                            GetCurrentVersions(container);
                        }
                        ExportSolutionBlock(container, solutionblock);
                    }
                }
                SendStatus(0, 0, 0, 0);
                //if (savedtimeout > -1)
                //{
                //    ResetTimeout(container, savedtimeout);
                //}
            }
            container.EndSection();
            return blocks;
        }

        /// <summary>
        /// Import entities to CRM from dictionary of blocks
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blocks">Blocks with entities to import</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records</returns>
        public Tuple<int, int, int, int, int, EntityReferenceCollection> ImportToCRM(IExecutionContainer container, ShuffleBlocks blocks)
        {
            container.StartSection("ImportToCRM");
            if (definition == null)
            {
                throw new ArgumentNullException("Definition", "Shuffle definition must be specified to import data");
            }

            var created = 0;
            var updated = 0;
            var skipped = 0;
            var deleted = 0;
            var failed = 0;
            var references = new EntityReferenceCollection();

            if (ShuffleDefinition.Blocks.Items.Any(b => (b is DataBlock data && data.Import != null) || b is SolutionBlock sol && sol.Import != null))
            {
                guidmap = new Dictionary<Guid, Guid>();
                stoponerror = ShuffleDefinition.StopOnError;
                timeout = ShuffleDefinition.TimeoutSpecified ? ShuffleDefinition.Timeout : -1;
                //double savedtimeout = -1;
                //if (timeout > -1)
                //{
                //    savedtimeout = SetTimeout();
                //}

                var totalBlocks = ShuffleDefinition.Blocks.Items.Length;
                var currentBlock = 0;
                foreach (var block in ShuffleDefinition.Blocks.Items)
                {
                    currentBlock++;
                    SendStatus(totalBlocks, currentBlock, -1, -1);
                    if (block is DataBlock datablock)
                    {
                        var name = datablock.Name;
                        if (!blocks.ContainsKey(name))
                        {
                            blocks.Add(name, new EntityCollection());
                        }
                        var dataresult = ImportDataBlock(container, datablock, blocks[name]);
                        created += dataresult.Item1;
                        updated += dataresult.Item2;
                        skipped += dataresult.Item3;
                        deleted += dataresult.Item4;
                        failed += dataresult.Item5;
                        references.AddRange(dataresult.Item6);
                    }
                    else if (block is SolutionBlock solutionblock)
                    {
                        var solutionresult = ImportSolutionBlock(solutionblock);
                        switch (solutionresult)
                        {
                            case ItemImportResult.Created: created++; break;
                            case ItemImportResult.Updated: updated++; break;
                            case ItemImportResult.Skipped: skipped++; break;
                            case ItemImportResult.Failed: failed++; break;
                        }
                    }
                }
                SendStatus(0, 0, 0, 0);
                //if (savedtimeout > -1)
                //{
                //    ResetTimeout(container, savedtimeout);
                //}
            }
            container.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

        /// <summary>
        /// Serialize blocks with entities with given serialization type
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blocks"></param>
        /// <param name="type"></param>
        /// <param name="delimeter">Optional, only required for SerializationType: Text</param>
        /// <returns></returns>
        public XmlDocument Serialize(IExecutionContainer container, ShuffleBlocks blocks, SerializationType type, char delimeter)
        {
            container.StartSection("Serialize");
            XmlDocument xml = null;
            if (blocks.Count > 0)
            {
                SendLine(container, "Serializing {0} blocks with type {1}", blocks.Count, type);
                xml = new XmlDocument();
                XmlNode root = xml.CreateElement("ShuffleData");
                xml.AppendChild(root);
                XML.AppendAttribute(root, "Type", type.ToString());
                XML.AppendAttribute(root, "ExportTime", DateTime.Now.ToString("s"));
                switch (type)
                {
                    case SerializationType.Full:
                    case SerializationType.Simple:
                    case SerializationType.SimpleWithValue:
                    case SerializationType.SimpleNoId:
                    case SerializationType.Explicit:
                        foreach (var block in blocks.Keys)
                        {
                            SendLine(container, $"Serializing {blocks[block].Count()} records in block {block}");
                            XmlNode xBlock = xml.CreateElement("Block");
                            root.AppendChild(xBlock);
                            XML.AppendAttribute(xBlock, "Name", block);
                            XML.AppendAttribute(xBlock, "Count", blocks[block].Count().ToString());
                            
                            var xSerialized = blocks[block].Serialize(container, (SerializationStyle)type);
                            xBlock.AppendChild(xml.ImportNode(xSerialized.ChildNodes[0], true));
                        }
                        break;

                    case SerializationType.Text:
                        XML.AppendAttribute(root, "Delimeter", delimeter.ToString());
                        var text = new StringBuilder();
                        foreach (var block in blocks.Keys)
                        {
                            SendLine(container, $"Serializing {blocks[block].Count()} records in block {block}");
                            text.AppendLine("<<<" + block + ">>>");
                            var serializedblock = blocks[block].ToTextFile(container, delimeter);
                            text.Append(serializedblock);
                        }
                        XML.AddCDATANode(root, "Text", text.ToString());
                        break;
                }
            }
            container.EndSection();
            return xml;
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
            var handler = RaiseShuffleEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private EntityCollection GetExistingSolutions(IExecutionContainer container)
        {
            var cSolutions = container.RetrieveMultiple("solution",
                new string[] { "isvisible" },
                new object[] { true },
                new ColumnSet("solutionid", "uniquename", "friendlyname", "version", "ismanaged"));
            return cSolutions;
        }

        private void ResetTimeout(IExecutionContainer container, double savedtimeout)
        {
            if (container.Service is OrganizationServiceProxy orgsvcpxy)
            {
                orgsvcpxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
            }
            if (container.Service is CrmServiceClient svcclient)
            {
                //svcclient.
                // svcclient.OrganizationServiceProxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
            }
        }

        private void SendLine(IExecutionContainer container)
        {
            SendText(container, "\n", false);
        }

        private void SendLine(IExecutionContainer container, string msg, params object[] args)
        {
            SendText(container, msg, false, args);
            SendLine(container);
        }

        private void SendLineUpdate(IExecutionContainer container, string msg, params object[] args)
        {
            SendText(container, msg, true, args);
            SendLine(container);
        }

        private void SendStatus(int totalBlocks, int currentBlock, int blockRecords, int currentRecord)
        {
            OnRaiseShuffleEvent(new ShuffleEventArgs(null, totalBlocks, currentBlock, blockRecords, currentRecord, false));
        }

        private void SendStatus(string block, string record)
        {
            OnRaiseShuffleEvent(new ShuffleEventArgs(null, block, record));
        }

        private void SendText(IExecutionContainer container, string msg, bool replacelast, params object[] args)
        {
            SendText(container, msg, -1, -1, -1, -1, replacelast, args);
        }

        private void SendText(IExecutionContainer container, string msg, int totalBlocks, int currentBlock, int blockRecords, int currentRecord, bool replacelast, params object[] args)
        {
            if (msg != null)
            {
                msg = string.Format(msg, args);
                if (msg.Length > 1)
                {
                    container.Log(msg, args);
                }
            }
            OnRaiseShuffleEvent(new ShuffleEventArgs(msg, totalBlocks, currentBlock, blockRecords, currentRecord, replacelast));
        }

        //private double SetTimeout()
        //{
        //SendLine("Setting timeout: {0} minutes", timeout);
        //double savedtimeout = -1;
        //if (crmsvc.Service is OrganizationServiceProxy orgsvcpxy)
        //{
        //    savedtimeout = orgsvcpxy.Timeout.TotalMinutes;
        //    orgsvcpxy.Timeout = new TimeSpan(0, timeout, 0);
        //}
        //else if (crmsvc.Service is CrmServiceClient svcclient)
        //{
        //    savedtimeout = svcclient.OrganizationServiceProxy.Timeout.TotalMinutes;
        //    svcclient.OrganizationServiceProxy.Timeout = new TimeSpan(0, timeout, 0);
        //}
        //return savedtimeout;
        //}

        #endregion Private Methods
    }
}
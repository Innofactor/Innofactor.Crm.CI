using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Cinteros.Crm.Utils.Misc;
using Cinteros.Crm.Utils.Common;
using Ionic.Zip;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Cinteros.Crm.Utils.Shuffle.Types;

/// <summary>Common namespace for Cinteros Shuffle functionality</summary>
namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>
    /// Class for Shuffling functionality
    /// </summary>
    public class Shuffler
    {
        private XmlDocument definition;
        private string definitionpath;
        private readonly CRMLogger log;
        private readonly CrmServiceProxy crmsvc;
        private Dictionary<Guid, Guid> guidmap = null;
        private bool stoponerror = false;
        private int timeout = 120;

        /// <summary>Dictionary with solutions and versions read from CRM using the /A:V directive.</summary>
        public Dictionary<string, Version> ExistingSolutionVersions = null;

        /// <summary>Event handler to receive notifications in consuming code</summary>
        /// <param name="sender"></param>
        /// <param name="a">Arguments passed in notification</param>
        public delegate void ShuffleEventHandler(object sender, ShuffleEventArgs a);
        public event EventHandler<ShuffleEventArgs> RaiseShuffleEvent;

        /// <summary>Shuffle Definition to be used</summary>
        public XmlDocument Definition
        {
            get { return definition; }
            set
            {
                var result = ShuffleHelper.ValidateDefinitionXml(value, log);
                SendLine(result);
                definition = value;
            }
        }

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log)
        {
            return QuickExport(Definition, Type, Delimeter, ShuffleEventHandler, crmsvc, log, null);
        }

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log, string defpath)
        {
            return QuickExport(Definition, Type, Delimeter, ShuffleEventHandler, crmsvc, log, defpath, false);
        }

        /// <summary>Export data according to shuffle definition in Definition to format Type</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Type">Type of target file</param>
        /// <param name="Delimeter">Delimeter to use when exporting to Type: Text</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the export. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <param name="defpath">Folder path for the shuffle definition file.</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>XmlDocument with exported data</returns>
        public static XmlDocument QuickExport(XmlDocument Definition, SerializationType Type, char Delimeter, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log, string defpath, bool clearRemainingShuffleVars)
        {
            log.StartSection("QuickExport");
            Shuffler shuffle = new Shuffler(crmsvc, log);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            ShuffleBlocks blocks = shuffle.ExportFromCRM();
            XmlDocument result = shuffle.Serialize(blocks, Type, Delimeter);
            log.EndSection();
            return result;
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log)
        {
            return QuickImport(Definition, Data, ShuffleEventHandler, crmsvc, log, null);
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log, string defpath)
        {
            return QuickImport(Definition, Data, ShuffleEventHandler, crmsvc, log, defpath, false);
        }

        /// <summary>Import data in Data according to shuffle definition in Definition</summary>
        /// <param name="Definition">Shuffle Definition</param>
        /// <param name="Data">Exported data</param>
        /// <param name="ShuffleEventHandler">Event handler processing messages from the import. May be null.</param>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        /// <param name="defpath">Path to definition file, if not standard</param>
        /// <param name="clearRemainingShuffleVars"></param>
        /// <returns>Tuple with counters for: Created, Updated, Skipped and Failed records and a collection of entityreferences for the created/updated records</returns>
        public static Tuple<int, int, int, int, int, EntityReferenceCollection> QuickImport(XmlDocument Definition, XmlDocument Data, EventHandler<ShuffleEventArgs> ShuffleEventHandler, CrmServiceProxy crmsvc, CRMLogger log, string defpath, bool clearRemainingShuffleVars)
        {
            log.StartSection("QuickImport");
            Shuffler shuffle = new Shuffler(crmsvc, log);
            if (ShuffleEventHandler != null)
            {
                shuffle.RaiseShuffleEvent += ShuffleEventHandler;
            }
            ShuffleHelper.VerifyShuffleVars(Definition, clearRemainingShuffleVars);
            shuffle.Definition = Definition;
            shuffle.definitionpath = defpath;
            ShuffleBlocks blocks = shuffle.Deserialize(Data);
            Tuple<int, int, int, int, int, EntityReferenceCollection> result = shuffle.ImportToCRM(blocks);
            log.EndSection();
            return result;
        }

        /// <summary>General constructor for the Shuffler class</summary>
        /// <param name="crmsvc"></param>
        /// <param name="log"></param>
        public Shuffler(CrmServiceProxy crmsvc, CRMLogger log)
        {
            this.crmsvc = crmsvc;
            this.log = log;
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
            XmlNode xRoot = CintXML.FindChild(definition, "ShuffleDefinition");
            XmlNode xBlocks = CintXML.FindChild(xRoot, "Blocks");
            if (xBlocks != null)
            {
                stoponerror = CintXML.GetBoolAttribute(xRoot, "StopOnError", false);
                timeout = CintXML.GetIntAttribute(xRoot, "Timeout", -1);
                double savedtimeout = -1;
                if (timeout > -1)
                {
                    SendLine("Setting timeout: {0} minutes", timeout);
                    OrganizationServiceProxy orgsvcpxy = crmsvc.GetService<OrganizationServiceProxy>();
                    savedtimeout = orgsvcpxy.Timeout.TotalMinutes;
                    orgsvcpxy.Timeout = new TimeSpan(0, timeout, 0);
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
                                CintDynEntityCollection cExported = ExportDataBlock(blocks, xBlock);
                                string name = CintXML.GetAttribute(xBlock, "Name");
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
                                break;
                            case "SolutionBlock":
                                if (ExistingSolutionVersions == null)
                                {
                                    GetCurrentVersions();
                                }
                                ExportSolutionBlock(xBlock);
                                break;
                        }
                    }
                }
                SendStatus(0, 0, 0, 0);
                if (savedtimeout > -1)
                {
                    OrganizationServiceProxy orgsvcpxy = crmsvc.GetService<OrganizationServiceProxy>();
                    orgsvcpxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
                }
            }
            log.EndSection();
            return blocks;
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
                    try
                    {
                        SendLine("Setting timeout: {0} minutes", timeout);
                        OrganizationServiceProxy orgsvcpxy = crmsvc.GetService<OrganizationServiceProxy>();
                        savedtimeout = orgsvcpxy.Timeout.TotalMinutes;
                        orgsvcpxy.Timeout = new TimeSpan(0, timeout, 0);
                    }
                    catch (InvalidPluginExecutionException)
                    {   // Couldn't cast to correct service type, for some reason...
                        savedtimeout = -1;
                    }
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
                    OrganizationServiceProxy orgsvcpxy = crmsvc.GetService<OrganizationServiceProxy>();
                    orgsvcpxy.Timeout = new TimeSpan(0, (int)savedtimeout, 0);
                }
            }
            log.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

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

        #region Private Export methods

        private CintDynEntityCollection ExportDataBlock(ShuffleBlocks blocks, XmlNode xBlock)
        {
            log.StartSection("ExportDataBlock");
            string name = CintXML.GetAttribute(xBlock, "Name");
            log.Log("Block: {0}", name);
            CintDynEntityCollection cExportEntities = null;

            if (xBlock.Name != "DataBlock")
            {
                throw new ArgumentOutOfRangeException("Type", xBlock.Name, "Invalid Block type");
            }
            string entity = CintXML.GetAttribute(xBlock, "Entity");
            string type = CintXML.GetAttribute(xBlock, "Type");
            XmlNode xExport = CintXML.FindChild(xBlock, "Export");
            if (xExport != null)
            {
                if (string.IsNullOrEmpty(entity))
                    entity = CintXML.GetAttribute(xExport, "Entity");

                #region Define attributes
                XmlNode xAttributes = CintXML.FindChild(xExport, "Attributes");
                bool allcolumns = false;
                List<string> lAttributes = new List<string>();
                List<string> lNullAttributes = new List<string>();
                if (xAttributes != null)
                {
                    foreach (XmlNode xAttr in xAttributes.ChildNodes)
                    {
                        if (xAttr.Name == "Attribute")
                        {
                            string attr = CintXML.GetAttribute(xAttr, "Name");
                            log.Log("Adding column: {0}", attr);
                            lAttributes.Add(attr.Replace("*", "%"));
                            if (attr.Contains("*"))
                            {
                                allcolumns = true;
                                log.Log("Found wildcard");
                            }
                            else
                            {
                                bool includenull = CintXML.GetBoolAttribute(xAttr, "IncludeNull", false);
                                if (includenull)
                                {
                                    lNullAttributes.Add(attr);
                                }
                            }
                        }
                    }
                }
                else
                {
                    allcolumns = true;
                    lAttributes.Add("*");
                    log.Log("Attributes not specified, retrieving all");
                }
                #endregion

                if (type == "Entity" || string.IsNullOrEmpty(type))
                {
                    #region QueryExpression Entity
                    log.StartSection("Export entity " + entity);
                    QueryExpression qExport = new QueryExpression(entity);
                    if (CintXML.GetBoolAttribute(xExport, "ActiveOnly", true))
                        CintQryExp.AppendConditionActive(qExport.Criteria);

                    foreach (var xBlockChild in xBlock.ChildNodes.Cast<XmlNode>())
                    {
                        if (xBlockChild.Name == "Relation")
                        {
                            AddRelationFilter(blocks, xBlockChild, qExport.Criteria, log);
                        }
                    }
                    foreach (XmlNode xExpProp in xExport.ChildNodes)
                    {
                        if (xExport.NodeType == XmlNodeType.Element)
                        {
                            switch (xExpProp.Name)
                            {
                                case "#comment":
                                case "Attributes":
                                    break;
                                case "Filter":
                                    AddFilter(qExport, xExpProp);
                                    break;
                                case "Sort":
                                    qExport.AddOrder(
                                        CintXML.GetAttribute(xExpProp, "Attribute"),
                                        CintXML.GetAttribute(xExpProp, "Type") == "Desc" ? OrderType.Descending : OrderType.Ascending);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("Name", xExpProp.Name, "Invalid subitem to export block " + name);
                            }
                        }
                    }
                    if (allcolumns)
                    {
                        qExport.ColumnSet = new ColumnSet(true);
                    }
                    else
                    {
                        foreach (string attr in lAttributes)
                            qExport.ColumnSet.AddColumn(attr);
                    }
#if DEBUG
                    log.Log("Converting to FetchXML");
                    try
                    {
                        var fetch = CintQryExp.ConvertToFetchXml(qExport, crmsvc);
                        log.Log("Exporting {0}:\n{1}", entity, fetch);
                    }
                    catch (Exception ex)
                    {
                        log.Log("Conversion error:");
                        log.Log(ex);
                    }
#endif
                    cExportEntities = CintDynEntity.RetrieveMultiple(crmsvc, qExport, log);
                    if (allcolumns)
                    {
                        SelectAttributes(cExportEntities, lAttributes, lNullAttributes);
                    }
                    SendLine("Block {0} - {1} records", name, cExportEntities.Count);
                    log.EndSection();
                    #endregion
                }
                else if (type == "Intersect")
                {
                    #region FetchXML Intersect
                    log.StartSection("Export intersect " + entity);
                    XmlDocument xDoc = new XmlDocument();
                    XmlNode xEntity = CintFetchXML.Create(xDoc, entity);
                    CintFetchXML.AddAttribute(xEntity, lAttributes.ToArray());

                    foreach (var xBlockChild in xBlock.ChildNodes.Cast<XmlNode>())
                    {
                        if (xBlockChild.Name == "Relation")
                        {
                            AddRelationFilter(blocks, xBlockChild, xEntity, log);
                        }
                    }

                    var fetch = xDoc.OuterXml;
                    fetch = fetch.Replace("<fetch ", "<fetch {0} {1} ");    // Detta för att se till att CrmServiceProxy.RetrieveMultiple kan hantera paging
#if DEBUG
                    log.Log("Exporting intersect entity {0}\n{1}", entity, fetch);
#endif
                    var qExport = new FetchExpression(fetch);
                    cExportEntities = CintDynEntity.RetrieveMultiple(crmsvc, qExport, log);
                    foreach (var cde in cExportEntities)
                    {
                        List<KeyValuePair<string, object>> newattributes = new List<KeyValuePair<string, object>>();
                        foreach (var attr in cde.Attributes)
                        {
                            if (attr.Value is Guid)
                            {
                                var attrname = attr.Key;
                                string relatedentity = attrname.Substring(0, attrname.Length - (attrname.EndsWith("idone") || attrname.EndsWith("idtwo") ? 5 : 2));
                                newattributes.Add(new KeyValuePair<string, object>(attrname, new EntityReference(relatedentity, (Guid)attr.Value)));
                            }
                        }
                        foreach (var newattr in newattributes)
                        {
                            if (!newattr.Key.Equals(cde.PrimaryIdAttribute))
                            {
                                cde.AddProperty(newattr.Key, newattr.Value);
                            }
                        }
                    }
                    log.EndSection();
                    #endregion
                }

                log.Log("Returning {0} records", cExportEntities.Count);
            }
            log.EndSection();
            return cExportEntities;
        }

        private static void SelectAttributes(CintDynEntityCollection cExportEntities, List<string> lAttributes, List<string> lNullAttributes)
        {
            foreach (CintDynEntity cde in cExportEntities)
            {
                int i = 0;
                List<string> x = new List<string>(cde.Attributes.Keys);
                while (i < cde.Attributes.Count)
                {
                    string attr = x[i];
                    if (attr != cde.PrimaryIdAttribute && !IncludeAttribute(attr, lAttributes))
                    {
                        cde.Attributes.Remove(attr);
                        x.Remove(attr);
                    }
                    else
                    {
                        i++;
                    }
                }
                foreach (string nullattribute in lNullAttributes)
                {
                    if (!cde.Contains(nullattribute))
                    {
                        cde.Entity.Attributes.Add(nullattribute, null);
                    }
                }
            }
        }

        private static bool IncludeAttribute(string attr, List<string> lAttributes)
        {
            foreach (string attribute in lAttributes)
            {
                if (IsSqlLikeMatch(attr, attribute))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSqlLikeMatch(string input, string pattern)
        {
            input = input.ToLowerInvariant().Trim();
            pattern = pattern.ToLowerInvariant().Trim().Replace('%', '*');
            if (pattern.StartsWith("*"))
            {
                if (pattern.EndsWith("*"))
                {
                    return input.Contains(pattern.Trim('*'));
                }
                else
                {
                    return input.EndsWith(pattern.Trim('*'));
                }
            }
            else if (pattern.EndsWith("*"))
            {
                return input.StartsWith(pattern.Trim('*'));
            }
            else
            {
                return input == pattern;
            }
        }

        private void AddFilter(QueryExpression qExport, XmlNode xExpProp)
        {
            string valuestring = CintXML.GetAttribute(xExpProp, "Value");
            if (valuestring.Contains("{0}"))
            {
                throw new ArgumentOutOfRangeException("Name", "Filter", "Parameterized Filters not supported in embedded Shuffle Utils");
            }
            string operstr = CintXML.GetAttribute(xExpProp, "Operator");
            if (string.IsNullOrEmpty(operstr))
            {
                operstr = "Equal";
            }
            Microsoft.Xrm.Sdk.Query.ConditionOperator oper = (Microsoft.Xrm.Sdk.Query.ConditionOperator)Enum.Parse(typeof(Microsoft.Xrm.Sdk.Query.ConditionOperator), operstr, true);

            object value = null;
            if (oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.EqualBusinessId &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.EqualUserId &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.EqualUserLanguage &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqualBusinessId &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqualUserId &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.Null &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.ThisMonth &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.ThisWeek &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.ThisYear &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.Today &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.Tomorrow &&
                oper != Microsoft.Xrm.Sdk.Query.ConditionOperator.Yesterday)
            {
                switch (CintXML.GetAttribute(xExpProp, "Type").ToUpperInvariant())
                {
                    case "STRING":
                        value = valuestring;
                        break;
                    case "INT":
                        value = int.Parse(valuestring);
                        break;
                    case "BOOL":
                        value = bool.Parse(valuestring);
                        break;
                    case "DATETIME":
                        value = DateTime.Parse(valuestring);
                        break;
                    case "GUID":
                        value = new Guid(valuestring);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Type", CintXML.GetAttribute(xExpProp, "Type"), "Invalid filter attribute type");
                }
            }
            string attribute = CintXML.GetAttribute(xExpProp, "Attribute");
            log.Log("Adding filter: {0} {1} {2}", attribute, oper, value);
            CintQryExp.AppendCondition(qExport.Criteria, LogicalOperator.And, attribute, oper, value);
        }

        private void AddRelationFilter(ShuffleBlocks blocks, XmlNode xRelation, FilterExpression filter, CRMLogger log)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            if (blocks != null && blocks.Count > 0)
            {
                var block = CintXML.GetAttribute(xRelation, "Block");
                var attribute = CintXML.GetAttribute(xRelation, "Attribute");
                var pkattribute = CintXML.GetAttribute(xRelation, "PK-Attribute");
                var includenull = CintXML.GetBoolAttribute(xRelation, "IncludeNull", false);
                var entityName = xRelation.ParentNode.Attributes["Entity"].Value;

                var type = GetAttributeType(attribute, entityName);

                ConditionExpression cond = new ConditionExpression();
                cond.AttributeName = attribute;
                cond.Operator = Microsoft.Xrm.Sdk.Query.ConditionOperator.In;

                List<object> ids = new List<object>();
                CintDynEntityCollection parentcoll = blocks.ContainsKey(block) ? blocks[block] : null;
                if (parentcoll != null && parentcoll.Count > 0)
                {
                    foreach (CintDynEntity parent in parentcoll)
                    {
                        if (string.IsNullOrEmpty(pkattribute))
                            if (type == AttributeTypeCode.String)
                                ids.Add(parent.Id.ToString());
                            else
                                ids.Add(parent.Id);
                        else if (type == AttributeTypeCode.String)
                            ids.Add(parent.Property<EntityReference>(pkattribute, new EntityReference()).Id.ToString());
                        else
                            ids.Add(parent.Property<EntityReference>(pkattribute, new EntityReference()).Id);
                    }
                }
                else
                {
                    // Adding temp guid to indicate "no matches", as ConditionOperator.In will fail if no values are given
                    ids.Add(new Guid());
                }
                cond.Values.AddRange(ids);
                if (!includenull)
                {
                    filter.AddCondition(cond);
                }
                else
                {
                    var orfilter = new FilterExpression(LogicalOperator.Or);
                    orfilter.AddCondition(attribute, Microsoft.Xrm.Sdk.Query.ConditionOperator.Null);
                    orfilter.AddCondition(cond);
                    filter.AddFilter(orfilter);
                }
                log.Log("Adding relation condition for {0} in {1} values in {2}.{3}", attribute, ids.Count, block, pkattribute);
            }
            log.EndSection();
        }

        private AttributeTypeCode? GetAttributeType(string attribute, string entityName)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name + " " + entityName + "." + attribute);
            AttributeTypeCode? type = null;
            var eqe = new EntityQueryExpression();
            eqe.Properties = new MetadataPropertiesExpression();
            eqe.Properties.PropertyNames.Add("Attributes");
            eqe.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityName));
            var aqe = new AttributeQueryExpression();
            aqe.Properties = new MetadataPropertiesExpression("LogicalName", "AttributeType");
            eqe.AttributeQuery = aqe;
            var req = new RetrieveMetadataChangesRequest()
            {
                Query = eqe,
                ClientVersionStamp = null
            };
            var resp = (RetrieveMetadataChangesResponse)crmsvc.Execute(req);
            if (resp.EntityMetadata.Count == 1)
            {
                foreach (var attr in resp.EntityMetadata[0].Attributes)
                {
                    if (attr.LogicalName == attribute)
                    {
                        type = attr.AttributeType;
                        break;
                    }
                }
            }
            log.Log("Type: {0}", type);
            log.EndSection();
            return type;
        }

        private static void AddRelationFilter(ShuffleBlocks blocks, XmlNode xRelation, XmlNode xEntity, CRMLogger log)
        {
            if (blocks != null && blocks.Count > 0)
            {
                var block = CintXML.GetAttribute(xRelation, "Block");
                var attribute = CintXML.GetAttribute(xRelation, "Attribute");
                var pkattribute = CintXML.GetAttribute(xRelation, "PK-Attribute");
                var includenull = CintXML.GetBoolAttribute(xRelation, "IncludeNull", false);
                List<string> ids = new List<string>();
                CintDynEntityCollection parentcoll = blocks.ContainsKey(block) ? blocks[block] : null;
                if (parentcoll != null && parentcoll.Count > 0)
                {
                    foreach (CintDynEntity parent in parentcoll)
                    {
                        if (string.IsNullOrEmpty(pkattribute))
                            ids.Add(parent.Id.ToString());
                        else
                            ids.Add(parent.Property<EntityReference>(pkattribute, new EntityReference()).Id.ToString());
                    }
                }
                else
                {
                    // Adding temp guid to indicate "no matches", as ConditionOperator.In will fail if no values are given
                    ids.Add(new Guid().ToString());
                }
                if (!includenull)
                {
                    CintFetchXML.AppendFilter(xEntity, "and", attribute, "in", ids.ToArray());
                }
                else
                {
                    var xFilter = CintFetchXML.AppendFilter(xEntity, "or");
                    CintFetchXML.AppendCondition(xFilter, attribute, "null");
                    CintFetchXML.AppendCondition(xFilter, attribute, "in", ids.ToArray());
                }
                log.Log("Adding relation condition for {0} in {1} values in {2}.{3}", attribute, ids.Count, block, pkattribute);
            }
        }

        private void ExportSolutionBlock(XmlNode xBlock)
        {
            log.StartSection("ExportSolutionBlock");

            if (xBlock.Name != "SolutionBlock")
            {
                throw new ArgumentOutOfRangeException("Type", xBlock.Name, "Invalid Block type");
            }
            string name = CintXML.GetAttribute(xBlock, "Name");
            log.Log("Block: {0}", name);
            string path = CintXML.GetAttribute(xBlock, "Path");
            string file = CintXML.GetAttribute(xBlock, "File");
            if (string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(definitionpath))
            {
                path = definitionpath;
                path += path.EndsWith("\\") ? "" : "\\";
            }
            if (string.IsNullOrWhiteSpace(file))
            {
                file = name;
            }
            XmlNode xExport = CintXML.FindChild(xBlock, "Export");
            if (xExport != null)
            {
                string type = CintXML.GetAttribute(xExport, "Type");
                string setversion = CintXML.GetAttribute(xExport, "SetVersion");
                bool publish = CintXML.GetBoolAttribute(xExport, "PublishBeforeExport", false);
                string targetversion = CintXML.GetAttribute(xExport, "TargetVersion");

                CintDynEntity cdSolution = GetAndVerifySolutionForExport(name);
                var currentversion = new Version(cdSolution.Property("version", "1.0.0.0"));

                SendLine("Solution: {0} {1}", name, currentversion);

                if (!string.IsNullOrWhiteSpace(setversion))
                {
                    SetNewSolutionVersion(setversion, cdSolution, currentversion);
                }

                if (publish)
                {
                    SendLine("Publishing customizations");
                    crmsvc.Execute(new PublishAllXmlRequest());
                }

                ExportSolutionRequest req = new ExportSolutionRequest()
                {
                    SolutionName = name
                };
#if Crm8
                if (!string.IsNullOrWhiteSpace(targetversion))
                {
                    req.TargetVersion = targetversion;
                }
#endif
                XmlNode xSettings = CintXML.FindChild(xExport, "Settings");
                if (xSettings != null)
                {
                    req.ExportAutoNumberingSettings = CintXML.GetBoolAttribute(xSettings, "AutoNumbering", false);
                    req.ExportCalendarSettings = CintXML.GetBoolAttribute(xSettings, "Calendar", false);
                    req.ExportCustomizationSettings = CintXML.GetBoolAttribute(xSettings, "Customization", false);
                    req.ExportEmailTrackingSettings = CintXML.GetBoolAttribute(xSettings, "EmailTracking", false);
                    req.ExportGeneralSettings = CintXML.GetBoolAttribute(xSettings, "General", false);
                    req.ExportMarketingSettings = CintXML.GetBoolAttribute(xSettings, "Marketing", false);
                    req.ExportOutlookSynchronizationSettings = CintXML.GetBoolAttribute(xSettings, "OutlookSync", false);
                    req.ExportRelationshipRoles = CintXML.GetBoolAttribute(xSettings, "RelationshipRoles", false);
                    req.ExportIsvConfig = CintXML.GetBoolAttribute(xSettings, "IsvConfig", false);
                }

                if (type == "Managed" || type == "Both")
                {
                    string filename = path + file + "_managed.zip";
                    SendLine("Exporting solution to: {0}", filename);
                    req.Managed = true;
                    ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)crmsvc.Execute(req);
                    byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
                    File.WriteAllBytes(filename, exportXml);
                }
                if (type == "Unmanaged" || type == "Both")
                {
                    string filename = path + file + ".zip";
                    SendLine("Exporting solution to: {0}", filename);
                    req.Managed = false;
                    ExportSolutionResponse exportSolutionResponse = (ExportSolutionResponse)crmsvc.Execute(req);
                    byte[] exportXml = exportSolutionResponse.ExportSolutionFile;
                    File.WriteAllBytes(filename, exportXml);
                }
            }
            log.EndSection();
        }

        private void SetNewSolutionVersion(string setversion, CintDynEntity cdSolution, Version currentversion)
        {
            Version newversion = new Version();
            if (setversion.Equals("IncrementAll", StringComparison.OrdinalIgnoreCase))
            {
                newversion = new Version("1.0.0.0");
                foreach (var existingversion in ExistingSolutionVersions.Values)
                {
                    if (existingversion > newversion)
                    {
                        newversion = existingversion;
                    }
                }
                newversion = IncrementVersion(newversion);
            }
            else if (setversion.Equals("Increment", StringComparison.OrdinalIgnoreCase))
            {
                newversion = IncrementVersion(currentversion);
            }
            else if (setversion.Equals("Current", StringComparison.OrdinalIgnoreCase))
            {
                newversion = currentversion;
            }
            else
            {
                newversion = new Version(setversion);
            }
            if (!currentversion.Equals(newversion))
            {
                SendLine("Setting version: {0}", newversion);
                CintDynEntity cdSolUpd = cdSolution.Clone(true);
                cdSolUpd.AddProperty("version", newversion.ToString());
                cdSolUpd.Save();
            }
        }

        private CintDynEntity GetAndVerifySolutionForExport(string name)
        {
            CintDynEntityCollection cSolutions = CintDynEntity.RetrieveMultiple(crmsvc, "solution",
                new string[] { "isvisible", "uniquename" },
                new object[] { true, name },
                new ColumnSet("solutionid", "friendlyname", "version", "ismanaged"), log);
            if (cSolutions.Count == 0)
            {
                throw new ArgumentOutOfRangeException("SolutionUniqueName", name, "Cannot find solution");
            }
            if (cSolutions.Count > 1)
            {
                throw new ArgumentOutOfRangeException("SolutionUniqueName", name, "Found " + cSolutions.Count.ToString() + " matching solutions");
            }
            CintDynEntity cdSolution = cSolutions[0];
            return cdSolution;
        }

        #endregion

        #region Private Import methods

        private Tuple<int, int, int, int, int, EntityReferenceCollection> ImportDataBlock(XmlNode xBlock, CintDynEntityCollection cEntities)
        {
            log.StartSection("ImportDataBlock");
            int created = 0;
            int updated = 0;
            int skipped = 0;
            int deleted = 0;
            int failed = 0;
            EntityReferenceCollection references = new EntityReferenceCollection();

            string name = CintXML.GetAttribute(xBlock, "Name");
            log.Log("Block: {0}", name);
            SendStatus(name, null);
            SendLine();

            switch (xBlock.Name)
            {
                case "DataBlock":
                    string type = CintXML.GetAttribute(xBlock, "Type");
                    XmlNode xImport = CintXML.FindChild(xBlock, "Import");
                    if (xImport != null)
                    {
                        bool includeid = CintXML.GetBoolAttribute(xImport, "CreateWithId", false);
                        string save = CintXML.GetAttribute(xImport, "Save");
                        string delete = CintXML.GetAttribute(xImport, "Delete");
                        bool updateinactive = CintXML.GetBoolAttribute(xImport, "UpdateInactive", false);
                        string deprecatedoverwrite = CintXML.GetAttribute(xImport, "Overwrite");
                        if (!string.IsNullOrWhiteSpace(deprecatedoverwrite))
                        {
                            SendLine("DEPRECATED use of attribute Overwrite!");
                            bool overwrite = CintXML.GetBoolAttribute(xImport, "Overwrite", true);
                            save = overwrite ? "CreateUpdate" : "CreateOnly";
                        }
                        if (string.IsNullOrWhiteSpace(save))
                        {   // Default
                            save = "CreateUpdate";
                        }
                        if (string.IsNullOrWhiteSpace(delete))
                        {   // Default
                            delete = "None";
                        }
                        XmlNode xMatch = CintXML.FindChild(xImport, "Match");
                        var matchattributes = GetMatchAttributes(xMatch);
                        var preretrieveall = xMatch != null ? CintXML.GetBoolAttribute(xMatch, "PreRetrieveAll", false) : false;

                        SendLine();
                        SendLine("Importing block {0} - {1} records ", name, cEntities.Count);

                        var i = 1;

                        if (delete == "All" && (matchattributes.Count == 0))
                        {   // All records shall be deleted, no match attribute defined, so just get all and delete all
                            string entity = CintXML.GetAttribute(xBlock, "Entity");
                            var qDelete = new QueryExpression(entity);
                            qDelete.ColumnSet.AddColumn(crmsvc.PrimaryAttribute(entity, log));
                            var deleterecords = CintDynEntity.RetrieveMultiple(crmsvc, qDelete, log);
                            SendLine("Deleting ALL {0} - {1} records", entity, deleterecords.Count);
                            foreach (var record in deleterecords)
                            {
                                SendLine("{0:000} Deleting existing: {1}", i, record);
                                try
                                {
                                    record.Delete();
                                    deleted++;
                                }
                                catch (FaultException<OrganizationServiceFault> ex)
                                {
                                    if (ex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                                    {   // This may happen through delayed cascade delete in CRM
                                        SendLine("      ...already deleted");
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                                i++;
                            }
                        }
                        int totalRecords = cEntities.Count;
                        i = 1;
                        CintDynEntityCollection cAllRecordsToMatch = null;
                        foreach (CintDynEntity cdEntity in cEntities)
                        {
                            string unique = cdEntity.Id.ToString();
                            SendStatus(-1, -1, totalRecords, i);
                            try
                            {
                                Guid oldid = cdEntity.Id;
                                Guid newid = Guid.Empty;

                                ReplaceGuids(cdEntity, includeid);
                                ReplaceUpdateInfo(cdEntity);
                                unique = GetEntityDisplayString(xMatch, cdEntity);
                                SendStatus(null, unique);

                                if (type == "Entity" || string.IsNullOrEmpty(type))
                                {
                                    #region Entity
                                    if (matchattributes.Count == 0)
                                    {
                                        if (save == "Never" || save == "UpdateOnly")
                                        {
                                            skipped++;
                                            SendLine("{0:000} Not saving: {1}", i, unique);
                                        }
                                        else
                                        {
                                            if (!includeid)
                                            {
                                                cdEntity.Id = Guid.Empty;
                                            }
                                            if (SaveEntity(cdEntity, null, updateinactive, i, unique))
                                            {
                                                created++;
                                                newid = cdEntity.Id;
                                                references.Add(cdEntity.Entity.ToEntityReference());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CintDynEntityCollection matches = GetMatchingRecords(cdEntity, matchattributes, preretrieveall, ref cAllRecordsToMatch);
                                        if (delete == "All" || (matches.Count == 1 && delete == "Existing"))
                                        {
                                            foreach (CintDynEntity cdMatch in matches)
                                            {
                                                SendLine("{0:000} Deleting existing: {1}", i, unique);
                                                try
                                                {
                                                    cdMatch.Delete();
                                                    deleted++;
                                                }
                                                catch (FaultException<OrganizationServiceFault> ex)
                                                {
                                                    if (ex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                                                    {   // This may happen through cascade delete in CRM
                                                        SendLine("      ...already deleted");
                                                    }
                                                    else
                                                    {
                                                        throw;
                                                    }
                                                }
                                            }
                                            matches.Clear();
                                        }
                                        if (matches.Count == 0)
                                        {
                                            if (save == "Never" || save == "UpdateOnly")
                                            {
                                                skipped++;
                                                SendLine("{0:000} Not creating: {1}", i, unique);
                                            }
                                            else
                                            {
                                                if (!includeid)
                                                {
                                                    cdEntity.Id = Guid.Empty;
                                                }
                                                if (SaveEntity(cdEntity, null, updateinactive, i, unique))
                                                {
                                                    created++;
                                                    newid = cdEntity.Id;
                                                    references.Add(cdEntity.Entity.ToEntityReference());
                                                }
                                            }
                                        }
                                        else if (matches.Count == 1)
                                        {
                                            var match = matches[0];
                                            newid = match.Id;
                                            if (save == "CreateUpdate" || save == "UpdateOnly")
                                            {
                                                if (SaveEntity(cdEntity, match, updateinactive, i, unique))
                                                {
                                                    updated++;
                                                    references.Add(cdEntity.Entity.ToEntityReference());
                                                }
                                                else
                                                {
                                                    skipped++;
                                                }
                                            }
                                            else
                                            {
                                                skipped++;
                                                SendLine("{0:000} Exists: {1}", i, unique);
                                            }
                                        }
                                        else
                                        {
                                            failed++;
                                            SendLine("Import object matches {0} records in target database!", matches.Count);
                                            SendLine(unique);
                                        }
                                    }
                                    if (!oldid.Equals(Guid.Empty) && !newid.Equals(Guid.Empty) && !oldid.Equals(newid) && !guidmap.ContainsKey(oldid))
                                    {
                                        log.Log("Mapping IDs: {0} ==> {1}", oldid, newid);
                                        guidmap.Add(oldid, newid);
                                    }

                                    #endregion
                                }
                                else if (type == "Intersect")
                                {
                                    #region Intersect
                                    if (cdEntity.Attributes.Count != 2)
                                    {
                                        throw new ArgumentOutOfRangeException("Attributes", cdEntity.Attributes.Count, "Invalid Attribute count for intersect object");
                                    }
                                    string intersect = CintXML.GetAttribute(xBlock, "IntersectName");
                                    if (string.IsNullOrEmpty(intersect))
                                    {
                                        intersect = cdEntity.Name;
                                    }

                                    EntityReference ref1 = (EntityReference)cdEntity.Attributes.ElementAt(0).Value;
                                    EntityReference ref2 = (EntityReference)cdEntity.Attributes.ElementAt(1).Value;
                                    CintDynEntity party1 = CintDynEntity.InitFromNameAndId(ref1.LogicalName, ref1.Id, crmsvc, log);
                                    CintDynEntity party2 = CintDynEntity.InitFromNameAndId(ref2.LogicalName, ref2.Id, crmsvc, log);
                                    try
                                    {
                                        party1.Associate(party2, intersect);
                                        created++;
                                        SendLine("{0} Associated: {1}", i.ToString().PadLeft(3, '0'), name);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Message.Contains("duplicate"))
                                        {
                                            SendLine("{0} Association exists: {1}", i.ToString().PadLeft(3, '0'), name);
                                            skipped++;
                                        }
                                        else
                                        {
                                            throw;
                                        }
                                    }
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                failed++;
                                SendLine("\n*** Error record: {0} ***\n{1}", unique, ex.Message);
                                log.Log(ex);
                                if (stoponerror)
                                {
                                    throw;
                                }
                            }
                            i++;
                        }

                        SendLine("Created: {0} Updated: {1} Skipped: {2} Deleted: {3} Failed: {4}", created, updated, skipped, deleted, failed);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Type", xBlock.Name, "Invalid Block type");
            }
            log.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

        private CintDynEntityCollection GetMatchingRecords(CintDynEntity cdEntity, List<string> matchattributes, bool preretrieveall, ref CintDynEntityCollection cAllRecordsToMatch)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            CintDynEntityCollection matches = null;
            if (preretrieveall)
            {
                if (cAllRecordsToMatch == null)
                {
                    cAllRecordsToMatch = GetAllRecordsForMatching(matchattributes, cdEntity);
                }
                matches = GetMatchingRecordsFromPreRetrieved(matchattributes, cdEntity, cAllRecordsToMatch);
            }
            else
            {
                QueryExpression qMatch = new QueryExpression(cdEntity.Name);
                qMatch.ColumnSet = new ColumnSet(cdEntity.PrimaryIdAttribute);
                if (cdEntity.Contains("ownerid"))
                {
                    qMatch.ColumnSet.AddColumn("ownerid");
                }
                if (cdEntity.Contains("statecode") || cdEntity.Contains("statuscode"))
                {
                    qMatch.ColumnSet.AddColumns("statecode", "statuscode");
                }
                foreach (var matchattr in matchattributes)
                {
                    object value = null;
                    if (cdEntity.Entity.Contains(matchattr))
                    {
                        value = CintEntity.AttributeToBaseType(cdEntity.Entity[matchattr]);
                    }
                    else if (matchattr == cdEntity.PrimaryIdAttribute)
                    {
                        value = cdEntity.Id;
                    }
                    if (value != null)
                    {
                        CintQryExp.AppendCondition(qMatch.Criteria, LogicalOperator.And, matchattr, Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, value);
                    }
                    else
                    {
                        CintQryExp.AppendCondition(qMatch.Criteria, LogicalOperator.And, matchattr, Microsoft.Xrm.Sdk.Query.ConditionOperator.Null, null);
                    }
                }
#if DEBUG
                log.Log("Finding matches for {0}:\n{1}", cdEntity, CintQryExp.ConvertToFetchXml(qMatch, crmsvc));
#endif
                matches = CintDynEntity.RetrieveMultiple(crmsvc, qMatch, log);
            }
            log.EndSection();
            return matches;
        }

        private CintDynEntityCollection GetAllRecordsForMatching(List<string> matchattributes, CintDynEntity cdEntity)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            QueryExpression qMatch = new QueryExpression(cdEntity.Name);
            qMatch.ColumnSet = new ColumnSet(cdEntity.PrimaryIdAttribute);
            if (cdEntity.Contains("ownerid"))
            {
                qMatch.ColumnSet.AddColumn("ownerid");
            }
            if (cdEntity.Contains("statecode") || cdEntity.Contains("statuscode"))
            {
                qMatch.ColumnSet.AddColumns("statecode", "statuscode");
            }
            qMatch.ColumnSet.AddColumns(matchattributes.ToArray());
#if DEBUG
            log.Log("Retrieving all records for {0}:\n{1}", cdEntity.Name, CintQryExp.ConvertToFetchXml(qMatch, crmsvc));
#endif
            CintDynEntityCollection matches = CintDynEntity.RetrieveMultiple(crmsvc, qMatch, log);
            SendLine("Pre-retrieved {0} records for matching", matches.Count);
            log.EndSection();
            return matches;
        }

        private CintDynEntityCollection GetMatchingRecordsFromPreRetrieved(List<string> matchattributes, CintDynEntity cdEntity, CintDynEntityCollection cAllRecordsToMatch)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            log.Log("Searching matches for: {0} {1}", cdEntity.Id, cdEntity);
            var result = new CintDynEntityCollection();
            foreach (var cdRecord in cAllRecordsToMatch)
            {
                var match = true;
                foreach (var attr in matchattributes)
                {
                    var srcvalue = "";
                    if (attr == cdEntity.PrimaryIdAttribute)
                    {
                        srcvalue = cdEntity.Id.ToString();
                    }
                    else
                    {
                        srcvalue = cdEntity.PropertyAsBaseType(attr, "<null>", false, false, true).ToString();
                    }
                    var trgvalue = cdRecord.PropertyAsBaseType(attr, "<null>", false, false, true).ToString();
                    if (srcvalue != trgvalue)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    result.Add(cdRecord);
                    log.Log("Found match: {0} {1}", cdRecord.Id, cdRecord);
                }
            }
            log.Log("Returned matches: {0}", result.Count);
            log.EndSection();
            return result;
        }

        private List<string> GetMatchAttributes(XmlNode xMatch)
        {
            var result = new List<string>();
            if (xMatch != null)
            {
                foreach (XmlNode xMatchAttr in xMatch.ChildNodes)
                {
                    var matchattr = CintXML.GetAttribute(xMatchAttr, "Name");
                    if (string.IsNullOrEmpty(matchattr))
                    {
                        throw new ArgumentOutOfRangeException("Match Attribute name not set");
                    }
                    result.Add(matchattr);
                }
            }
            return result;
        }

        private static string GetEntityDisplayString(XmlNode xMatch, CintDynEntity cdEntity)
        {
            var unique = new List<string>();
            if (xMatch != null && xMatch.ChildNodes.Count > 0)
            {
                foreach (XmlNode xMatchAttr in xMatch.ChildNodes)
                {
                    string matchdisplay = CintXML.GetAttribute(xMatchAttr, "Display");
                    if (string.IsNullOrEmpty(matchdisplay))
                    {
                        matchdisplay = CintXML.GetAttribute(xMatchAttr, "Name");
                    }
                    var matchvalue = "<null>";
                    if (cdEntity.Contains(matchdisplay, true))
                    {
                        if (cdEntity.Entity[matchdisplay] is EntityReference)
                        {   // Don't use PropertyAsString, that would perform GetRelated that we don't want due to performance
                            var entref = cdEntity.Property<EntityReference>(matchdisplay, null);
                            if (!string.IsNullOrEmpty(entref.Name))
                            {
                                matchvalue = entref.Name;
                            }
                            else
                            {
                                matchvalue = entref.LogicalName + ":" + entref.Id.ToString();
                            }
                        }
                        else
                        {
                            matchvalue = cdEntity.PropertyAsString(matchdisplay, "", false, false, true);
                        }
                    }
                    unique.Add(matchvalue);
                }
            }
            if (unique.Count == 0)
            {
                unique.Add(cdEntity.Id.ToString());
            }
            return string.Join(", ", unique);
        }

        private bool SaveEntity(CintDynEntity cdNewEntity, CintDynEntity cdMatchEntity, bool updateInactiveRecord, int pos, string identifier)
        {
            log.StartSection("SaveEntity " + pos.ToString("000 ") + identifier);
            bool recordSaved = false;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                identifier = cdNewEntity.ToString();
            }
            var newOwner = cdNewEntity.Property<EntityReference>("ownerid", null);
            var newState = cdNewEntity.Property<OptionSetValue>("statecode", null);
            var newStatus = cdNewEntity.Property<OptionSetValue>("statuscode", null);
            var newActive = newState != null ? CintEntity.GetActiveStates(cdNewEntity.Name).Contains(newState.Value) : true;
            bool nowActive = true;
            if ((newState == null) != (newStatus == null))
            {
                throw new InvalidDataException("When setting status of the record, both statecode and statuscode must be present");
            }
            if (!newActive)
            {
                log.Log("Removing state+status from entity to update");
                cdNewEntity.RemoveProperty("statecode");
                cdNewEntity.RemoveProperty("statuscode");
            }
            if (cdMatchEntity == null)
            {
                cdNewEntity.Create();
                recordSaved = true;
                SendLine("{0:000} Created: {1}", pos, identifier);
            }
            else
            {
                var oldState = cdMatchEntity.Property<OptionSetValue>("statecode", null);
                var oldActive = oldState != null ? CintEntity.GetActiveStates(cdNewEntity.Name).Contains(oldState.Value) : true;
                nowActive = oldActive;
                cdNewEntity.Id = cdMatchEntity.Id;
                if (!oldActive && (newActive || updateInactiveRecord))
                {   // Inaktiv post som ska aktiveras eller uppdateras
                    cdNewEntity.SetState(0, 1);
                    SendLine("{0:000} Activated: {1} for update", pos, identifier);
                    nowActive = true;
                }

                if (nowActive)
                {
                    cdNewEntity.Update();
                    recordSaved = true;
                    SendLine("{0:000} Updated: {1}", pos, identifier);
                }
                else
                {
                    SendLine("{0:000} Inactive: {1}", pos, identifier);
                }
                if (newOwner != null && !newOwner.Equals(cdMatchEntity.Property("ownerid", new EntityReference())))
                {
                    cdNewEntity.Assign(newOwner);
                    SendLine("{0:000} Assigned: {1} to {2} {3}", pos, identifier, newOwner.LogicalName, string.IsNullOrEmpty(newOwner.Name) ? newOwner.Id.ToString() : newOwner.Name);
                }
            }
            if (newActive != nowActive)
            {   // Aktiv skall ändras på posten
                var newStatusValue = newStatus.Value;
                if (cdNewEntity.Name == "savedquery" && newState.Value == 1 && newStatusValue == 1)
                {   // Justering för inaktiverad men ej publicerad vy
                    newStatusValue = 2;
                }
                cdNewEntity.SetState(newState.Value, newStatusValue);
                SendLine("{0:000} SetState: {1}: {2}/{3}", pos, identifier, newState.Value, newStatus.Value);
            }
            log.EndSection();
            return recordSaved;
        }

        private void ReplaceGuids(CintDynEntity cdEntity, bool includeid)
        {
            foreach (KeyValuePair<string, object> prop in cdEntity.Attributes)
            {
                if (prop.Value is Guid && guidmap.ContainsKey((Guid)prop.Value))
                    if (includeid)
                        throw new NotImplementedException("Cannot handle replacement of Guid type attributes");
                    else
                        log.Log("No action, we don't care about the guid of the object");
                if (prop.Value is EntityReference && guidmap.ContainsKey(((EntityReference)prop.Value).Id))
                    ((EntityReference)prop.Value).Id = guidmap[((EntityReference)prop.Value).Id];
            }
        }

        private static void ReplaceUpdateInfo(CintDynEntity cdEntity)
        {
            List<string> removeAttr = new List<string>();
            List<KeyValuePair<string, object>> newAttr = new List<KeyValuePair<string, object>>();
            foreach (KeyValuePair<string, object> attr in cdEntity.Attributes)
            {
                if (attr.Key == "createdby")
                {
                    if (!cdEntity.Attributes.Contains("createdonbehalfby"))
                    {
                        newAttr.Add(new KeyValuePair<string, object>("createdonbehalfby", attr.Value));
                    }
                    removeAttr.Add("createdby");
                }
                else if (attr.Key == "modifiedby")
                {
                    if (!cdEntity.Attributes.Contains("modifiedonbehalfby"))
                    {
                        newAttr.Add(new KeyValuePair<string, object>("modifiedonbehalfby", attr.Value));
                    }
                    removeAttr.Add("modifiedby");
                }
                else if (attr.Key == "createdon")
                {
                    if (!cdEntity.Attributes.Contains("overriddencreatedon"))
                    {
                        newAttr.Add(new KeyValuePair<string, object>("overriddencreatedon", attr.Value));
                    }
                    removeAttr.Add("createdon");
                }
            }
            foreach (string key in removeAttr)
            {
                cdEntity.Attributes.Remove(key);
            }
            if (newAttr.Count > 0)
            {
                cdEntity.Attributes.AddRange(newAttr);
            }
        }

        private ItemImportResult ImportSolutionBlock(XmlNode xBlock)
        {
            log.StartSection("ImportSolutionBlock");
            var importResult = ItemImportResult.None;
            if (xBlock.Name != "SolutionBlock")
            {
                throw new ArgumentOutOfRangeException("Type", xBlock.Name, "Invalid Block type");
            }
            XmlNode xImport = CintXML.FindChild(xBlock, "Import");
            if (xImport != null)
            {
                string name = CintXML.GetAttribute(xBlock, "Name");
                log.Log("Block: {0}", name);
                SendStatus(name, null);
                string type = CintXML.GetAttribute(xImport, "Type");
                SendLine();
                SendLine("Importing solution: {0}", name);

                string filename = GetSolutionFilename(xBlock, name, type);
                var version = ExtractVersionFromSolutionZip(filename);
                try
                {
                    ValidatePreReqs(xImport, version);
                    SolutionImportConditions ImportCondition = CheckIfImportRequired(xImport, name, version);
                    if (ImportCondition != SolutionImportConditions.Skip)
                    {
                        if (DoImportSolution(xImport, filename, version))
                        {
                            if (ImportCondition == SolutionImportConditions.Create)
                            {
                                importResult = ItemImportResult.Created;
                            }
                            else
                            {
                                importResult = ItemImportResult.Updated;
                            }
                        }
                        else
                        {
                            importResult = ItemImportResult.Failed;
                            log.Log("Failed during import");
                        }
                        bool publish = CintXML.GetBoolAttribute(xImport, "PublishAll", false);
                        if (publish)
                        {
                            SendLine("Publishing customizations");
                            crmsvc.Execute(new PublishAllXmlRequest());
                        }
                    }
                    else
                    {
                        importResult = ItemImportResult.Skipped;
                        log.Log("Skipped due to import condition");
                    }
                }
                catch (Exception ex)
                {
                    log.Log(ex);
                    importResult = ItemImportResult.Failed;
                    if (stoponerror)
                    {
                        throw;
                    }
                }
            }
            log.EndSection();
            return importResult;
        }

        private void ValidatePreReqs(XmlNode xImport, Version thisversion)
        {
            log.StartSection("ValidatePreReqs");
            XmlNode xPreReqs = CintXML.FindChild(xImport, "PreRequisites");
            if (xPreReqs != null)
            {
                CintDynEntityCollection cSolutions = GetExistingSolutions();
                foreach (XmlNode xPreReq in xPreReqs.ChildNodes)
                {
                    if (xPreReq.NodeType == XmlNodeType.Element && xPreReq.Name == "Solution")
                    {
                        bool valid = false;
                        string name = CintXML.GetAttribute(xPreReq, "Name");
                        string comparer = CintXML.GetAttribute(xPreReq, "Comparer");
                        var version = new Version();
                        log.Log("Prereq: {0} {1} {2}", name, comparer, version);

                        if (comparer.Contains("this"))
                        {
                            version = thisversion;
                            comparer = comparer.Replace("-this", "");
                        }
                        else if (comparer != "any")
                        {
                            version = new Version(CintXML.GetAttribute(xPreReq, "Version").Replace('*', '0'));
                        }

                        foreach (CintDynEntity cdSolution in cSolutions)
                        {
                            if (cdSolution.Property("uniquename", "") == name)
                            {
                                log.Log("Found matching solution");
                                switch (comparer)
                                {
                                    case "any":
                                        valid = true;
                                        break;
                                    case "eq":
                                        valid = new Version(cdSolution.Property("version", "1.0.0.0")).Equals(version);
                                        break;
                                    case "ge":
                                        valid = new Version(cdSolution.Property("version", "<undefined>")) >= version;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException("Comparer", comparer, "Invalid comparer value");
                                }
                            }
                            if (valid)
                            {
                                break;
                            }
                        }
                        if (valid)
                        {
                            SendLine("Prerequisite {0} {1} {2} is satisfied", name, comparer, version);
                        }
                        else
                        {
                            SendLine("Prerequisite {0} {1} {2} is NOT satisfied", name, comparer, version);
                            throw new Exception("Prerequisite NOT satisfied (" + name + " " + comparer + " " + version + ")");
                        }
                    }
                }
            }
            else
            {
                log.Log("No prereqs for solution import");
            }
            log.EndSection();
        }

        private SolutionImportConditions CheckIfImportRequired(XmlNode xImport, string name, Version thisversion)
        {
            log.StartSection("CheckIfImportRequired");
            SolutionImportConditions result = SolutionImportConditions.Create;
            bool overwritesame = CintXML.GetBoolAttribute(xImport, "OverwriteSameVersion", true);
            bool overwritenewer = CintXML.GetBoolAttribute(xImport, "OverwriteNewerVersion", false);
            CintDynEntityCollection cSolutions = GetExistingSolutions();
            foreach (CintDynEntity cdSolution in cSolutions)
            {
                if (cdSolution.Property("uniquename", "") == name)
                {   // Now we have found the same solution in target environment
                    result = SolutionImportConditions.Update;
                    var existingversion = new Version(cdSolution.Property("version", "1.0.0.0"));
                    log.Log("Existing solution has version: {0}", existingversion);
                    var comparison = thisversion.CompareTo(existingversion);
                    if (!overwritesame && comparison == 0)
                    {
                        result = SolutionImportConditions.Skip;
                        SendLine("Solution {0} {1} already exists in target", name, thisversion);
                    }
                    else if (!overwritenewer && comparison < 0)
                    {
                        result = SolutionImportConditions.Skip;
                        SendLine("Existing solution {0} {1} is newer than {2}", name, existingversion, thisversion);
                    }
                    else if (existingversion == thisversion)
                    {
                        SendLine("Updating version {0}", thisversion);
                    }
                    else
                    {
                        SendLine("Replacing version {0} with {1}", existingversion, thisversion);
                    }
                    break;
                }
            }
            log.Log("Import Condition: {0}", result);
            log.EndSection();
            return result;
        }

        private Version ExtractVersionFromSolutionZip(string filename)
        {
            log.StartSection("ExtractVersionFromSolutionZip");
            using (ZipFile zip = ZipFile.Read(filename))
            {
                zip["solution.xml"].Extract(definitionpath, ExtractExistingFileAction.OverwriteSilently);
            }
            if (!System.IO.File.Exists(definitionpath + "\\solution.xml"))
            {
                throw new Exception("Unable to unzip solution.xml from file: " + filename);
            }
            XmlDocument xSolution = new XmlDocument();
            xSolution.Load(definitionpath + "\\solution.xml");
            System.IO.File.Delete(definitionpath + "\\solution.xml");
            XmlNode xRoot = CintXML.FindChild(xSolution, "ImportExportXml");
            if (xRoot == null)
            {
                throw new XmlException("Cannot find root element ImportExportXml");
            }
            XmlNode xManifest = CintXML.FindChild(xRoot, "SolutionManifest");
            if (xManifest == null)
            {
                throw new XmlException("Cannot find element SolutionManifest");
            }
            XmlNode xVersion = CintXML.FindChild(xManifest, "Version");
            if (xVersion == null)
            {
                throw new XmlException("Cannot find element Version");
            }
            var version = new Version(xVersion.InnerText);
            log.Log("Version {0} extracted", version);
            log.EndSection();
            return version;
        }

        private string GetSolutionFilename(XmlNode xBlock, string name, string type)
        {
            log.StartSection("GetSolutionFilename");
            string file = CintXML.GetAttribute(xBlock, "File");
            if (string.IsNullOrWhiteSpace(file))
            {
                file = name;
            }
            string path = CintXML.GetAttribute(xBlock, "Path");
            if (string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(definitionpath))
            {
                path = definitionpath;
            }
            path += path.EndsWith("\\") ? "" : "\\";
            string filename;
            if (type == "Managed")
            {
                filename = path + file + "_managed.zip";
            }
            else if (type == "Unmanaged")
            {
                filename = path + file + ".zip";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Type", type, "Invalid Solution type");
            }

            if (filename.Contains("%"))
            {
                IDictionary envvars = Environment.GetEnvironmentVariables();
                foreach (DictionaryEntry de in envvars)
                {
                    filename = filename.Replace("%" + de.Key.ToString() + "%", de.Value.ToString());
                }
            }
            log.Log("Filename: {0}", filename);
            log.EndSection();
            return filename;
        }

        private bool DoImportSolution(XmlNode xImport, string filename, Version version)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            var result = false;
            bool activatecode = CintXML.GetBoolAttribute(xImport, "ActivateServersideCode", false);
            bool overwrite = CintXML.GetBoolAttribute(xImport, "OverwriteCustomizations", false);
            Exception ex = null;
            SendLine("Importing solution: {0} Version: {1}", filename, version);
            byte[] fileBytes = File.ReadAllBytes(filename);
            ImportSolutionRequest impSolReq = new ImportSolutionRequest()
            {
                CustomizationFile = fileBytes,
                OverwriteUnmanagedCustomizations = overwrite,
                PublishWorkflows = activatecode,
                ImportJobId = Guid.NewGuid()
            };
            if (crmsvc.CrmVersion.Major >= 6)
            {   // CRM 2013 or later, import async
                result = DoImportSolutionAsync(impSolReq, ref ex);
            }
            else
            {   // Pre CRM 2013, import sync
                result = DoImportSolutionSync(impSolReq, ref ex);
            }
            if (!result && stoponerror)
            {
                if (ex != null)
                {
                    throw ex;
                }
                else
                {
                    throw new Exception("Solution import failed");
                }
            }
            log.Log("Returning: {0}", result);
            log.EndSection();
            return result;
        }

        private bool DoImportSolutionAsync(ImportSolutionRequest impSolReq, ref Exception ex)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            // Code cred to Wael Hamze
            // http://waelhamze.com/2013/11/17/asynchronous-solution-import-dynamics-crm-2013/
            var result = false;
            ExecuteAsyncRequest asyncRequest = new ExecuteAsyncRequest()
            {
                Request = impSolReq
            };
            ExecuteAsyncResponse asyncResponse = crmsvc.Execute(asyncRequest) as ExecuteAsyncResponse;
            var asyncJobId = asyncResponse.AsyncJobId;
            DateTime end = timeout > 0 ? DateTime.Now.AddMinutes(timeout) : DateTime.Now.AddMinutes(2);
            log.Log("Timout until: {0}", end.ToString("HH:mm:ss.fff"));
            var importStatus = -1;
            var progress = 0;
            var statustext = "Submitting job";
            SendLineUpdate("Import status: {0}", statustext);
            while (end >= DateTime.Now)
            {
                CintDynEntity cdAsyncOperation = null;
                try
                {
                    cdAsyncOperation = CintDynEntity.Retrieve("asyncoperation", asyncJobId,
                        new ColumnSet("asyncoperationid", "statecode", "statuscode", "message", "friendlymessage"), crmsvc, log);
                }
                catch (Exception asyncex)
                {
                    cdAsyncOperation = null;
                    log.Log(asyncex);
                    log.EndSection();   // Ending section started by Retrieve above to prevent indentation inflation
                }
                if (cdAsyncOperation != null)
                {
                    statustext = cdAsyncOperation.PropertyAsString("statuscode", "?", false, false);
                    var newStatus = cdAsyncOperation.Property("statuscode", new OptionSetValue()).Value;
                    if (newStatus != importStatus)
                    {
                        importStatus = newStatus;
                        SendLineUpdate("Import status: {0}", statustext);
                        log.Log("Import message:\n{0}", cdAsyncOperation.Property("message", "<none>"));
                        if (importStatus == 30)
                        {   // Succeeded
                            result = true;
                            break;
                        }
                        else if (importStatus == 21 || importStatus == 22 || importStatus == 31 || importStatus == 32)
                        {   // Error statuses
                            var friendlymessage = cdAsyncOperation.Property("friendlymessage", "");
                            SendLine("Message: {0}", friendlymessage);
                            if (friendlymessage == "Access is denied.")
                            {
                                SendLine("When importing to onprem environment, the async service user must be granted read/write permission to folder:");
                                SendLine("  C:\\Program Files\\Microsoft Dynamics CRM\\CustomizationImport");
                            }
                            else
                            {
                                SendLine("See log file for technical details.");
                            }
                            ex = new Exception(string.Format("Solution Import Failed: {0} - {1}",
                                cdAsyncOperation.PropertyAsString("statecode", "?", false, false),
                                cdAsyncOperation.PropertyAsString("statuscode", "?", false, false)));
                            break;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
                if (importStatus == 20)
                {   // In progress, read percent
                    try
                    {
                        var job = CintDynEntity.Retrieve("importjob", impSolReq.ImportJobId, new ColumnSet("progress"), crmsvc, log);
                        if (job != null)
                        {
                            var newProgress = job.Property("progress", 0D);
                            if (newProgress > progress + 5)
                            {
                                progress = Convert.ToInt32(Math.Round(newProgress));
                                SendStatus(-1, -1, 100, progress);
                                SendLineUpdate("Import status: {0} - {1}%", statustext, progress);
                            }
                        }
                    }
                    catch (Exception jobex)
                    {   // We probably tried before the job was created
                        if (jobex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                        {
                            log.Log("Importjob not created yet or already deleted");
                        }
                        else
                        {
                            log.Log(jobex);
                        }
                        log.EndSection();   // Ending section started by Retrieve above to prevent indentation inflation
                    }
                }
            }
            if (end < DateTime.Now)
            {
                SendLine("Import timed out.");
            }
            SendStatus(-1, -1, 100, 0);
            log.EndSection();
            return result;
        }

        private bool DoImportSolutionSync(ImportSolutionRequest impSolReq, ref Exception ex)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            bool result;
            try
            {
                crmsvc.Execute(impSolReq);
            }
            catch (Exception e)
            {
                ex = e;
                SendLine("Error during import: {0}", ex.Message);
            }
            finally
            {
                result = ReadAndLogSolutionImportJobStatus(impSolReq.ImportJobId);
            }
            log.EndSection();
            return result;
        }

        private bool ReadAndLogSolutionImportJobStatus(Guid jobid)
        {
            log.StartSection("ReadAndLogSolutionImportJobStatus " + jobid);
            var success = false;
            var job = CintDynEntity.LoadFromNameAndId("importjob", jobid, new ColumnSet("startedon", "completedon", "progress", "data"), crmsvc, log);
            if (job != null)
            {
                var name = "?";
                var result = "?";
                var err = "";
                var start = job.Property("startedon", DateTime.MinValue);
                var complete = job.Property("completedon", DateTime.MinValue);
                var time = complete != null && start != null ? complete.Subtract(start) : new TimeSpan();
                var prog = job.Property<double>("progress", 0);
                if (job.Contains("data", true))
                {
                    XmlDocument doc = new XmlDocument();
                    var data = job.Property("data", "");
                    log.Log("Job data length: {0}", data.Length);
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        doc.LoadXml(data);
                        var namenode = doc.SelectSingleNode("//solutionManifest/UniqueName");
                        if (namenode != null) { name = namenode.InnerText; }
                        var resultnode = doc.SelectSingleNode("//solutionManifest/result/@result");
                        if (resultnode != null) { result = resultnode.Value; }
                        var errnode = doc.SelectSingleNode("//solutionManifest/result/@errortext");
                        if (errnode != null) { err = errnode.Value; }
                    }
                }
                if (prog >= 100 && result == "success")
                {
                    SendLine("Solution {0} imported in {1}", name, time);
                    log.Log("Result: {0}\nError:  {1}\nTime:   {2}", result, err, time);
                    success = true;
                }
                else
                {
                    SendLine("Solution: {0}", name);
                    SendLine("Result:   {0}", result);
                    SendLine("Error:    {0}", err);
                    SendLine("Progress: {0}", prog);
                    SendLine("Time:     {0}", time);
                }
            }
            log.Log("Returning: {0}", success);
            log.EndSection();
            return success;
        }

        #endregion

        #region Private helper methods

        private CintDynEntityCollection GetExistingSolutions()
        {
            CintDynEntityCollection cSolutions = CintDynEntity.RetrieveMultiple(crmsvc, "solution",
                new string[] { "isvisible" },
                new object[] { true },
                new ColumnSet("solutionid", "uniquename", "friendlyname", "version", "ismanaged"), log);
            return cSolutions;
        }

        /// <summary>Get the current versions for all solutions defined in the definition file</summary>
        /// <remarks>Results will be placed in the public dictionary <c ref="ExistingSolutionVersions">ExistingSolutionVersions</c></remarks>
        public void GetCurrentVersions()
        {
            log.StartSection("GetCurrentVersions");
            ExistingSolutionVersions = new Dictionary<string, Version>();
            XmlNode xRoot = CintXML.FindChild(definition, "ShuffleDefinition");
            XmlNode xBlocks = CintXML.FindChild(xRoot, "Blocks");
            if (xBlocks != null)
            {
                var solutions = GetExistingSolutions();
                foreach (XmlNode xBlock in xBlocks.ChildNodes)
                {
                    if (xBlock.NodeType == XmlNodeType.Element)
                    {
                        switch (xBlock.Name)
                        {
                            case "DataBlock":
                                break;
                            case "SolutionBlock":
                                var xmlNode = CintXML.FindChild(xBlock, "Export");
                                if (xmlNode != null)
                                {
                                    var name = CintXML.GetAttribute(xBlock, "Name");
                                    log.Log("Getting version for: {0}", name);
                                    foreach (var solution in solutions)
                                    {
                                        if (name.Equals(solution.Property("uniquename", ""), StringComparison.OrdinalIgnoreCase))
                                        {
                                            ExistingSolutionVersions.Add(name, new Version(solution.Property("version", "1.0.0.0")));
                                            log.Log("Version found: {0}", ExistingSolutionVersions[name]);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            log.EndSection();
        }

        private Version IncrementVersion(Version version)
        {
            var verparts = version;
            var newversion = verparts.Major.ToString() + "." + verparts.Minor.ToString() + "." + DateTime.Today.ToString("yyMM") + "." + (verparts.Revision + 1).ToString();
            log.Log("Increasing {0} to {1}", version, newversion);
            return new Version(newversion);
        }

        private void SendLine()
        {
            SendText("\n", false);
        }

        private void SendLineUpdate(string msg, params object[] args)
        {
            SendText(msg, true, args);
            SendLine();
        }

        private void SendLine(string msg, params object[] args)
        {
            SendText(msg, false, args);
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

        #endregion
    }
}

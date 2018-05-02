using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Common.Interfaces;
using Cinteros.Crm.Utils.Misc;
using Cinteros.Crm.Utils.Shuffle.Types;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Cinteros.Crm.Utils.Shuffle
{
    public partial class Shuffler
    {
        #region Private Methods

        private static void AddRelationFilter(ShuffleBlocks blocks, XmlNode xRelation, XmlNode xEntity, ILoggable log)
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

        private void AddRelationFilter(ShuffleBlocks blocks, XmlNode xRelation, FilterExpression filter, ILoggable log)
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
                {
                    entity = CintXML.GetAttribute(xExport, "Entity");
                }

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

                #endregion Define attributes

                if (CintXML.FindChild(xExport, "FetchXML") is XmlNode xFetchXML &&
                    CintXML.FindChild(xFetchXML, "#text") is XmlText xFetchXMLText)
                {
                    log.StartSection("Export entity using FetchXML");
                    var fetchxml = xFetchXMLText.Value;
#if DEBUG
                    log.Log("FetchXML:\n{0}", fetchxml);
#endif
                    cExportEntities = CintDynEntity.RetrieveMultiple(crmsvc, new FetchExpression(fetchxml), log);
                    log.EndSection();
                }
                else if (type == "Entity" || string.IsNullOrEmpty(type))
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

                    #endregion QueryExpression Entity
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

                    #endregion FetchXML Intersect
                }

                log.Log("Returning {0} records", cExportEntities.Count);
            }
            log.EndSection();
            return cExportEntities;
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

        #endregion Private Methods
    }
}
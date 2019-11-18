namespace Cinteros.Crm.Utils.Shuffle
{
    //using Cinteros.Crm.Utils.Common;
    //using Cinteros.Crm.Utils.Common.Interfaces;
    //using Cinteros.Crm.Utils.Misc;
    using Innofactor.Xrm.Utils.Common;
    using Innofactor.Xrm.Utils.Common.Fluent;
    using Innofactor.Xrm.Utils.Common.Fluent.Entity;
    using Innofactor.Xrm.Utils.Common.Fluent.Attribute;
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
    using Innofactor.Xrm.Utils.Common.Extensions;
    using Innofactor.Xrm.Utils.Common.Interfaces;

    public partial class Shuffler
    {
        #region Private Methods

        private static void AddRelationFilter(IExecutionContainer container, ShuffleBlocks blocks, DataBlockRelation relation, XmlNode xEntity, ILoggable log)
        {
            if (blocks != null && blocks.Count > 0)
            {
                var block = relation.Block;
                var attribute = relation.Attribute;
                var pkattribute = relation.PKAttribute;
                var includenull = relation.IncludeNull;
                var ids = new List<string>();
                var parentcoll = blocks.ContainsKey(block) ? blocks[block] : null;
                if (parentcoll != null && parentcoll.Entities.Count > 0)
                {
                    foreach (var parent in parentcoll.Entities)
                    {
                        if (string.IsNullOrEmpty(pkattribute))
                        {
                            ids.Add(parent.Id.ToString());
                        }
                        else
                        {
                            ids.Add(parent.GetAttribute(pkattribute, new EntityReference()).Id.ToString());
                        }
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
                container.Log("Adding relation condition for {0} in {1} values in {2}.{3}", attribute, ids.Count, block, pkattribute);
            }
        }

        private static bool IncludeAttribute(string attr, List<string> lAttributes)
        {
            foreach (var attribute in lAttributes)
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

        private static void SelectAttributes(IExecutionContainer container, EntityCollection cExportEntities, List<string> lAttributes, List<string> lNullAttributes)
        {
            foreach (var cde in cExportEntities.Entities)
            {
                var i = 0;
                var x = new List<string>(cde.Attributes.Keys);
                while (i < cde.Attributes.Count)
                {
                    var attr = x[i];
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
                foreach (var nullattribute in lNullAttributes)
                {
                    if (!cde.Contains(nullattribute))
                    {
                        cde.Entity.Attributes.Add(nullattribute, null);
                    }
                }
            }
        }

        private void AddFilter(IExecutionContainer container, QueryExpression qExport, DataBlockExportFilter filter)
        {
            var valuestring = filter.Value;
            if (valuestring.Contains("{0}"))
            {
                throw new ArgumentOutOfRangeException("Name", "Filter", "Parameterized Filters not supported in embedded Shuffle Utils");
            }
            var operstr = filter.Operator.ToString();
            if (string.IsNullOrEmpty(operstr))
            {
                operstr = "Equal";
            }
            var oper = (Microsoft.Xrm.Sdk.Query.ConditionOperator)Enum.Parse(typeof(Microsoft.Xrm.Sdk.Query.ConditionOperator), operstr, true);

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
                if (filter.TypeSpecified)
                {
                    switch (filter.Type)
                    {
                        case FilterTypes.@string:
                            value = valuestring;
                            break;

                        case FilterTypes.@int:
                            value = int.Parse(valuestring);
                            break;

                        case FilterTypes.@bool:
                            value = bool.Parse(valuestring);
                            break;

                        case FilterTypes.datetime:
                            value = DateTime.Parse(valuestring);
                            break;

                        case FilterTypes.guid:
                            value = new Guid(valuestring);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("Type", filter.Type, "Invalid filter attribute type");
                    }
                }
            }
            var attribute = filter.Attribute;
            container.Log("Adding filter: {0} {1} {2}", attribute, oper, value);
            CintQryExp.AppendCondition(qExport.Criteria, LogicalOperator.And, attribute, oper, value);
        }

        private void AddRelationFilter(IExecutionContainer container, ShuffleBlocks blocks, string entityName, DataBlockRelation relation, FilterExpression filter, ILoggable log)
        {
            container.StartSection(MethodBase.GetCurrentMethod().Name);
            if (blocks != null && blocks.Count > 0)
            {
                var block = relation.Block;
                var attribute = relation.Attribute;
                var pkattribute = relation.PKAttribute;
                var includenull = relation.IncludeNull;

                var type = GetAttributeType(attribute, entityName);

                var cond = new ConditionExpression
                {
                    AttributeName = attribute,
                    Operator = Microsoft.Xrm.Sdk.Query.ConditionOperator.In
                };

                var ids = new List<object>();
                var parentcoll = blocks.ContainsKey(block) ? blocks[block] : null;
                if (parentcoll != null && parentcoll.Entities.Count > 0)
                {
                    foreach (var parent in parentcoll.Entities)
                    {
                        if (string.IsNullOrEmpty(pkattribute))
                        {
                            if (type == AttributeTypeCode.String)
                            {
                                ids.Add(parent.Id.ToString());
                            }
                            else
                            {
                                ids.Add(parent.Id);
                            }
                        }
                        else if (type == AttributeTypeCode.String)
                        {
                            ids.Add(parent.GetAttribute(pkattribute, new EntityReference()).Id.ToString());
                        }
                        else
                        {
                            ids.Add(parent.GetAttribute(pkattribute, new EntityReference()).Id);
                        }
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
                container.Log("Adding relation condition for {0} in {1} values in {2}.{3}", attribute, ids.Count, block, pkattribute);
            }
            container.EndSection();
        }

        private EntityCollection ExportDataBlock(IExecutionContainer container, ShuffleBlocks blocks, DataBlock block)
        {
            container.StartSection("ExportDataBlock");
            container.Log("Block: {0}", block.Name);
            EntityCollection cExportEntities = null;

            if (block.Export != null)
            {
                #region Define attributes

                var attributes = block.Export.Items.Where(i => i is DataBlockExportAttributes).FirstOrDefault() as DataBlockExportAttributes;
                var allcolumns = false;
                var lAttributes = new List<string>();
                var lNullAttributes = new List<string>();
                if (attributes != null)
                {
                    foreach (var attribute in attributes.Attribute)
                    {
                        var attr = attribute.Name;
                        container.Log("Adding column: {0}", attr);
                        lAttributes.Add(attr.Replace("*", "%"));
                        if (attr.Contains("*"))
                        {
                            allcolumns = true;
                            container.Log("Found wildcard");
                        }
                        else
                        {
                            if (attribute.IncludeNull)
                            {
                                lNullAttributes.Add(attr);
                            }
                        }
                    }
                }
                else
                {
                    allcolumns = true;
                    lAttributes.Add("*");
                    container.Log("Attributes not specified, retrieving all");
                }

                #endregion Define attributes

                var fetchxml = block.Export.Items.Where(i => i is string).FirstOrDefault() as string;
                if (!string.IsNullOrWhiteSpace(fetchxml))
                {
                    container.StartSection("Export entity using FetchXML");
#if DEBUG
                    container.Log("FetchXML:\n{0}", fetchxml);
#endif
                    cExportEntities = container.RetrieveMultiple(new FetchExpression(fetchxml));
                    
                    container.EndSection();
                }
                else if (!block.TypeSpecified || block.Type == EntityTypes.Entity)
                {
                    #region QueryExpression Entity

                    container.StartSection("Export entity " + block.Entity);
                    var qExport = new QueryExpression(block.Entity);
                    if (block.Export.ActiveOnly)
                    {
                        CintQryExp.AppendConditionActive(qExport.Criteria);
                    }

                    if (block.Relation != null)
                    {
                        foreach (var relation in block.Relation)
                        {
                            AddRelationFilter(blocks, block.Entity, relation, qExport.Criteria, log);
                        }
                    }
                    foreach (var filter in block.Export.Items.Where(i => i is DataBlockExportFilter).Cast<DataBlockExportFilter>())
                    {
                        AddFilter(container, qExport, filter);
                    }
                    foreach (var sort in block.Export.Items.Where(i => i is DataBlockExportSort).Cast<DataBlockExportSort>())
                    {
                        qExport.AddOrder(sort.Attribute, sort.Type == SortTypes.Desc ? OrderType.Descending : OrderType.Ascending);
                    }
                    if (allcolumns)
                    {
                        qExport.ColumnSet = new ColumnSet(true);
                    }
                    else
                    {
                        foreach (var attr in lAttributes)
                        {
                            qExport.ColumnSet.AddColumn(attr);
                        }
                    }
#if DEBUG
                    container.Log("Converting to FetchXML");
                    try
                    {
                        var fetch = CintQryExp.ConvertToFetchXml(qExport, crmsvc);
                        container.Log("Exporting {0}:\n{1}", block.Entity, fetch);
                    }
                    catch (Exception ex)
                    {
                        container.Log("Conversion error:");
                        container.Log(ex);
                    }
#endif
                    cExportEntities = CintDynEntity.RetrieveMultiple(crmsvc, qExport, log);
                    if (allcolumns)
                    {
                        SelectAttributes(container, cExportEntities, lAttributes, lNullAttributes);
                    }
                    SendLine(container, "Block {0} - {1} records", block.Name, cExportEntities.Count());
                    container.EndSection();

                    #endregion QueryExpression Entity
                }
                else if (block.Type == EntityTypes.Intersect)
                {
                    #region FetchXML Intersect

                    container.StartSection("Export intersect " + block.Entity);
                    var xDoc = new XmlDocument();
                    var xEntity = CintFetchXML.Create(xDoc, block.Entity);
                    CintFetchXML.AddAttribute(xEntity, lAttributes.ToArray());

                    foreach (var relation in block.Relation)
                    {
                        AddRelationFilter(blocks, relation, xEntity, log);
                    }

                    var fetch = xDoc.OuterXml;
                    fetch = fetch.Replace("<fetch ", "<fetch {0} {1} ");    // Detta för att se till att CrmServiceProxy.RetrieveMultiple kan hantera paging
#if DEBUG
                    container.Log("Exporting intersect entity {0}\n{1}", block.Entity, fetch);
#endif
                    var qExport = new FetchExpression(fetch);
                    cExportEntities = CintDynEntity.RetrieveMultiple(crmsvc, qExport, log);
                    foreach (var cde in cExportEntities)
                    {
                        var newattributes = new List<KeyValuePair<string, object>>();
                        foreach (var attr in cde.Attributes)
                        {
                            if (attr.Value is Guid)
                            {
                                var attrname = attr.Key;
                                var relatedentity = attrname.Substring(0, attrname.Length - (attrname.EndsWith("idone") || attrname.EndsWith("idtwo") ? 5 : 2));
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
                    container.EndSection();

                    #endregion FetchXML Intersect
                }

                container.Log("Returning {0} records", cExportEntities.Count());
            }
            container.EndSection();
            return cExportEntities;
        }

        private AttributeTypeCode? GetAttributeType(IExecutionContainer container, string attribute, string entityName)
        {
            container.StartSection(MethodBase.GetCurrentMethod().Name + " " + entityName + "." + attribute);
            AttributeTypeCode? type = null;
            var eqe = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression()
            };
            eqe.Properties.PropertyNames.Add("Attributes");
            eqe.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityName));
            var aqe = new AttributeQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "AttributeType")
            };
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
            container.Log("Type: {0}", type);
            container.EndSection();
            return type;
        }

        #endregion Private Methods
    }
}
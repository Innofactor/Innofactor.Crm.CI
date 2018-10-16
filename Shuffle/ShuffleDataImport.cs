namespace Cinteros.Crm.Utils.Shuffle
{
    using Cinteros.Crm.Utils.Common;
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Cinteros.Crm.Utils.Common.Slim;
    using Cinteros.Crm.Utils.Shuffle.Types;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;

    public partial class Shuffler
    {
        #region Private Methods

        private static bool EntityAttributesEqual(IContainable container, List<string> matchattributes, Entity entity1, Entity entity2)
        {
            var match = true;
            foreach (var attr in matchattributes)
            {
                var srcvalue = "";
                if(attr == container.Service.IdAttribute(entity1.LogicalName, container.Logger))
                {
                    srcvalue = entity1.Id.ToString();
                }
                else
                {
                    srcvalue = entity1.PropertyAsBaseType(attr, "<null>", true).ToString();
                }
                var trgvalue = entity2.PropertyAsBaseType(attr, "<null>", true).ToString();
                if (srcvalue != trgvalue)
                {
                    match = false;
                    break;
                }
            }
            return match;
        }

        private static string GetEntityDisplayString(IContainable container, DataBlockImportMatch match, Entity cdEntity)
        {
            var unique = new List<string>();
            if (match != null && match.Attribute.Length > 0)
            {
                foreach (var attribute in match.Attribute)
                {
                    var matchdisplay = attribute.Display;
                    if (string.IsNullOrEmpty(matchdisplay))
                    {
                        matchdisplay = attribute.Name;
                    }
                    var matchvalue = "<null>";
                    if (cdEntity.Contains(matchdisplay, true))
                    {
                        if (cdEntity[matchdisplay] is EntityReference)
                        {   // Don't use PropertyAsString, that would perform GetRelated that we don't want due to performance
                            var entref = cdEntity.GetAttribute<EntityReference>(matchdisplay, null);
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
                            matchvalue = cdEntity.PropertyAsString(matchdisplay, "", true);
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

        private static void ReplaceUpdateInfo(Entity cdEntity)
        {
            var removeAttr = new List<string>();
            var newAttr = new List<KeyValuePair<string, object>>();
            foreach (var attr in cdEntity.Attributes)
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
            foreach (var key in removeAttr)
            {
                cdEntity.Attributes.Remove(key);
            }
            if (newAttr.Count > 0)
            {
                cdEntity.Attributes.AddRange(newAttr);
            }
        }

        private EntityCollection GetAllRecordsForMatching(IContainable container, List<string> allattributes, Entity cdEntity)
        {
            container.Logger.StartSection(MethodBase.GetCurrentMethod().Name);
            var qMatch = new QueryExpression(cdEntity.LogicalName)
            {
                ColumnSet = new ColumnSet(allattributes.ToArray())
            };
#if DEBUG
            container.Logger.Log("Retrieving all records for {0}:\n{1}", cdEntity.LogicalName, container.Convert(qMatch));
#endif
            var matches = container.RetrieveMultiple(qMatch);
            SendLine(container, $"Pre-retrieved {matches.Count()} records for matching");
            container.Logger.EndSection();
            return matches;
        }

        private List<string> GetMatchAttributes(DataBlockImportMatch match)
        {
            var result = new List<string>();
            if (match != null)
            {
                foreach (var attribute in match.Attribute)
                {
                    var matchattr = attribute.Name;
                    if (string.IsNullOrEmpty(matchattr))
                    {
                        throw new ArgumentOutOfRangeException("Match Attribute name not set");
                    }
                    result.Add(matchattr);
                }
            }
            return result;
        }

        private EntityCollection GetMatchingRecords(IContainable container, Entity entity, List<string> matchattributes, List<string> updateattributes, bool preretrieveall, ref EntityCollection cAllRecordsToMatch)
        {
            try
            {
                container.Logger.StartSection(MethodBase.GetCurrentMethod().Name);
                EntityCollection matches = null;
                var allattributes = new List<string>
                {
                    //allattributes.Add(entity.PrimaryIdAttribute);
                    container.Entity(entity.LogicalName).PrimaryIdAttribute
                };


                if (entity.Contains("ownerid"))
                {
                    allattributes.Add("ownerid");
                }
                if (entity.Contains("statecode") || entity.Contains("statuscode"))
                {
                    allattributes.Add("statecode");
                    allattributes.Add("statuscode");
                }
                allattributes = allattributes.Union(matchattributes.Union(updateattributes)).ToList();
                if (preretrieveall)
                {
                    if (cAllRecordsToMatch == null)
                    {
                        cAllRecordsToMatch = GetAllRecordsForMatching(container, allattributes, entity);
                    }
                    matches = GetMatchingRecordsFromPreRetrieved(container, matchattributes, entity, cAllRecordsToMatch);
                }
                else
                {
                    var qMatch = new QueryExpression(entity.LogicalName)
                    {
                        // We need to be able to see if any attributes have changed, so lets make sure matching records have all the attributes that will be updated
                        ColumnSet = new ColumnSet(allattributes.ToArray())
                    };

                    foreach (var matchattr in matchattributes)
                    {
                        object value = null;
                        if (entity.Contains(matchattr))
                        {
                            value = CintEntity.AttributeToBaseType(entity[matchattr]);
                        }
                        else if (matchattr == container.Entity(entity.LogicalName).PrimaryIdAttribute)
                        {
                            value = entity.Id;
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
                    container.Logger.Log("Finding matches for {0}:\n{1}", entity, container.Convert(qMatch));
#endif
                    matches = container.RetrieveMultiple(qMatch);
                }

                return matches;
            }
            catch (Exception ex)
            {
                container.Logger.Log(ex.ToString());
            }
            finally
            {
                container.Logger.EndSection();
            }
            return null;
        }

        private EntityCollection GetMatchingRecordsFromPreRetrieved(IContainable container, List<string> matchattributes, Entity cdEntity, EntityCollection cAllRecordsToMatch)
        {
            container.Logger.StartSection(MethodBase.GetCurrentMethod().Name);
            container.Logger.Log("Searching matches for: {0} {1}", cdEntity.Id, cdEntity);
            var result = new EntityCollection();
            foreach (var cdRecord in cAllRecordsToMatch.Entities)
            {
                if (EntityAttributesEqual(container, matchattributes, cdEntity, cdRecord))
                {
                    result.Add(cdRecord);
                    container.Logger.Log("Found match: {0} {1}", cdRecord.Id, cdRecord);
                }
            }
            container.Logger.Log("Returned matches: {0}", result.Count());
            container.Logger.EndSection();
            return result;
        }

        private List<string> GetUpdateAttributes(EntityCollection entities)
        {
            var result = new List<string>();
            foreach (var entity in entities.Entities)
            {
                foreach (var attribute in entity.Attributes.Keys)
                {
                    if (!result.Contains(attribute))
                    {
                        result.Add(attribute);
                    }
                }
            }
            return result;
        }

        private Tuple<int, int, int, int, int, EntityReferenceCollection> ImportDataBlock(IContainable container, DataBlock block, EntityCollection cEntities)
        {
            container.Logger.StartSection(MethodBase.GetCurrentMethod().Name);
            var created = 0;
            var updated = 0;
            var skipped = 0;
            var deleted = 0;
            var failed = 0;
            var references = new EntityReferenceCollection();

            var name = block.Name;
            container.Logger.Log("Block: {0}", name);
            SendStatus(name, null);
            SendLine(container);

            if (block.Import != null)
            {
                var includeid = block.Import.CreateWithId;
                var save = block.Import.Save;
                var delete = block.Import.Delete;
                var updateinactive = block.Import.UpdateInactive;
                var updateidentical = block.Import.UpdateIdentical;
                if (block.Import.OverwriteSpecified)
                {
                    SendLine(container, "DEPRECATED use of attribute Overwrite!");
                    save = block.Import.Overwrite ? SaveTypes.CreateUpdate : SaveTypes.CreateOnly;
                }
                var matchattributes = GetMatchAttributes(block.Import.Match);
                var updateattributes = !updateidentical ? GetUpdateAttributes(cEntities) : new List<string>();
                var preretrieveall = block.Import.Match?.PreRetrieveAll ?? false;

                SendLine(container);
                SendLine(container, $"Importing block {name} - {cEntities.Entities.Count} records ");

                var i = 1;

                if (delete == DeleteTypes.All && (matchattributes.Count == 0))
                {   // All records shall be deleted, no match attribute defined, so just get all and delete all
                    var entity = block.Entity;
                    var qDelete = new QueryExpression(entity);
                    qDelete.ColumnSet.AddColumn(container.Entity(entity).PrimaryIdAttribute);
                    var deleterecords = container.RetrieveMultiple(qDelete);
                    SendLine(container, "Deleting ALL {0} - {1} records", entity, deleterecords.Entities.Count);
                    foreach (var record in deleterecords.Entities)
                    {
                        SendLine(container, "{0:000} Deleting existing: {1}", i, record);
                        try
                        {
                            container.Delete(record);
                            deleted++;
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            if (ex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                            {   // This may happen through delayed cascade delete in CRM
                                SendLine(container, "      ...already deleted");
                            }
                            else
                            {
                                throw ex;
                            }
                        }
                        i++;
                    }
                }
                var totalRecords = cEntities.Entities.Count;
                i = 1;
                EntityCollection cAllRecordsToMatch = null;
                foreach (var cdEntity in cEntities.Entities)
                {
                    var unique = cdEntity.Id.ToString();
                    SendStatus(-1, -1, totalRecords, i);
                    try
                    {
                        var oldid = cdEntity.Id;
                        var newid = Guid.Empty;

                        ReplaceGuids(container, cdEntity, includeid);
                        ReplaceUpdateInfo(cdEntity);
                        unique = GetEntityDisplayString(container, block.Import.Match, cdEntity);
                        SendStatus(null, unique);

                        if (!block.TypeSpecified || block.Type == EntityTypes.Entity)
                        {
                            #region Entity

                            if (matchattributes.Count == 0)
                            {
                                if (save == SaveTypes.Never || save == SaveTypes.UpdateOnly)
                                {
                                    skipped++;
                                    SendLine(container, "{0:000} Not saving: {1}", i, unique);
                                }
                                else
                                {
                                    if (!includeid)
                                    {
                                        cdEntity.Id = Guid.Empty;
                                    }
                                    if (SaveEntity(container, cdEntity, null, updateinactive, updateidentical, i, unique))
                                    {
                                        created++;
                                        newid = cdEntity.Id;
                                        references.Add(cdEntity.ToEntityReference());
                                    }
                                }
                            }
                            else
                            {
                                var matches = GetMatchingRecords(container, cdEntity, matchattributes, updateattributes, preretrieveall, ref cAllRecordsToMatch);
                                if (delete == DeleteTypes.All || (matches.Entities.Count == 1 && delete == DeleteTypes.Existing))
                                {
                                    foreach (var cdMatch in matches.Entities)
                                    {
                                        SendLine(container, "{0:000} Deleting existing: {1}", i, unique);
                                        try
                                        {
                                            container.Delete(cdMatch);
                                            deleted++;
                                        }
                                        catch (FaultException<OrganizationServiceFault> ex)
                                        {
                                            if (ex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                                            {   // This may happen through cascade delete in CRM
                                                SendLine(container, "      ...already deleted");
                                            }
                                            else
                                            {
                                                throw ex;
                                            }
                                        }
                                    }
                                    matches.Entities.Clear();
                                }
                                if (matches.Count() == 0)
                                {
                                    if (save == SaveTypes.Never || save == SaveTypes.UpdateOnly)
                                    {
                                        skipped++;
                                        SendLine(container, "{0:000} Not creating: {1}", i, unique);
                                    }
                                    else
                                    {
                                        if (!includeid)
                                        {
                                            cdEntity.Id = Guid.Empty;
                                        }
                                        if (SaveEntity(container, cdEntity, null, updateinactive, updateidentical, i, unique))
                                        {
                                            created++;
                                            newid = cdEntity.Id;
                                            references.Add(cdEntity.ToEntityReference());
                                        }
                                    }
                                }
                                else if (matches.Count() == 1)
                                {
                                    var match = matches[0];
                                    newid = match.Id;
                                    if (save == SaveTypes.CreateUpdate || save == SaveTypes.UpdateOnly)
                                    {
                                        if (SaveEntity(container, cdEntity, match, updateinactive, updateidentical, i, unique))
                                        {
                                            updated++;
                                            references.Add(cdEntity.ToEntityReference());
                                        }
                                        else
                                        {
                                            skipped++;
                                        }
                                    }
                                    else
                                    {
                                        skipped++;
                                        SendLine(container, "{0:000} Exists: {1}", i, unique);
                                    }
                                }
                                else
                                {
                                    failed++;
                                    SendLine(container, "Import object matches {0} records in target database!", matches.Entities.Count);
                                    SendLine(container, unique);
                                }
                            }
                            if (!oldid.Equals(Guid.Empty) && !newid.Equals(Guid.Empty) && !oldid.Equals(newid) && !guidmap.ContainsKey(oldid))
                            {
                                container.Logger.Log("Mapping IDs: {0} ==> {1}", oldid, newid);
                                guidmap.Add(oldid, newid);
                            }

                            #endregion Entity
                        }
                        else if (block.Type == EntityTypes.Intersect)
                        {
                            #region Intersect

                            if (cdEntity.Attributes.Count != 2)
                            {
                                throw new ArgumentOutOfRangeException("Attributes", cdEntity.Attributes.Count, "Invalid Attribute count for intersect object");
                            }
                            var intersect = block.IntersectName;
                            if (string.IsNullOrEmpty(intersect))
                            {
                                intersect = cdEntity.LogicalName;
                            }

                            var ref1 = (EntityReference)cdEntity.Attributes.ElementAt(0).Value;
                            var ref2 = (EntityReference)cdEntity.Attributes.ElementAt(1).Value;
                            var party1 = new Entity(ref1.LogicalName, ref1.Id);
                            var party2 = new Entity(ref2.LogicalName, ref2.Id);
                            try
                            {
                                
                                container.Associate(party1, party2, intersect);
                                created++;
                                SendLine(container, "{0} Associated: {1}", i.ToString().PadLeft(3, '0'), name);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("duplicate"))
                                {
                                    SendLine(container, "{0} Association exists: {1}", i.ToString().PadLeft(3, '0'), name);
                                    skipped++;
                                }
                                else
                                {
                                    throw ex;
                                }
                            }

                            #endregion Intersect
                        }
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        SendLine(container, $"\n*** Error record: {unique} ***\n{ex.Message}");
                        container.Logger.Log(ex);
                        if (stoponerror)
                        {
                            throw ex;
                        }
                    }
                    i++;
                }

                SendLine(container, $"Created: {created} Updated: {updated} Skipped: {skipped} Deleted: {deleted} Failed: {failed}");
            }
            container.Logger.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

        private void ReplaceGuids(IContainable container, Entity cdEntity, bool includeid)
        {
            foreach (var prop in cdEntity.Attributes)
            {
                if (prop.Value is Guid && guidmap.ContainsKey((Guid)prop.Value))
                {
                    if (includeid)
                    {
                        throw new NotImplementedException("Cannot handle replacement of Guid type attributes");
                    }
                    else
                    {
                        container.Logger.Log("No action, we don't care about the guid of the object");
                    }
                }

                if (prop.Value is EntityReference && guidmap.ContainsKey(((EntityReference)prop.Value).Id))
                {
                    ((EntityReference)prop.Value).Id = guidmap[((EntityReference)prop.Value).Id];
                }
            }
        }

        private bool SaveEntity(IContainable container, Entity cdNewEntity, Entity cdMatchEntity, bool updateInactiveRecord, bool updateIdentical, int pos, string identifier)
        {
            container.Logger.StartSection("SaveEntity " + pos.ToString("000 ") + identifier);
            var recordSaved = false;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                identifier = cdNewEntity.ToStringExt();
            }
            var newOwner = cdNewEntity.GetAttribute<EntityReference>("ownerid", null);
            var newState = cdNewEntity.GetAttribute<OptionSetValue>("statecode", null);
            var newStatus = cdNewEntity.GetAttribute<OptionSetValue>("statuscode", null);
            var newActive = newState != null ? CintEntity.GetActiveStates(cdNewEntity.LogicalName).Contains(newState.Value) : true;
            var nowActive = true;
            if ((newState == null) != (newStatus == null))
            {
                throw new InvalidDataException("When setting status of the record, both statecode and statuscode must be present");
            }
            if (!newActive)
            {
                container.Logger.Log("Removing state+status from entity to update");
                cdNewEntity.RemoveProperty("statecode");
                cdNewEntity.RemoveProperty("statuscode");
            }
            if (cdMatchEntity == null)
            {
                container.Create(cdNewEntity);
                recordSaved = true;
                SendLine(container, "{0:000} Created: {1}", pos, identifier);
            }
            else
            {
                var oldState = cdMatchEntity.GetAttribute<OptionSetValue>("statecode", null);
                var oldActive = oldState != null ? CintEntity.GetActiveStates(cdNewEntity.LogicalName).Contains(oldState.Value) : true;
                nowActive = oldActive;
                cdNewEntity.Id = cdMatchEntity.Id;
                if (!oldActive && (newActive || updateInactiveRecord))
                {   // Inaktiv post som ska aktiveras eller uppdateras
                    container.SetState(cdNewEntity, 0, 1);
                    SendLine(container, "{0:000} Activated: {1} for update", pos, identifier);
                    nowActive = true;
                }

                if (nowActive)
                {
                    var updateattributes = cdNewEntity.Attributes.Keys.ToList();

                    if (updateattributes.Contains(container.Entity(cdNewEntity.LogicalName).PrimaryIdAttribute))
                    {
                        updateattributes.Remove(container.Entity(cdNewEntity.LogicalName).PrimaryIdAttribute);
                    }
                    if (updateIdentical || !EntityAttributesEqual(container, updateattributes, cdNewEntity, cdMatchEntity))
                    {
                        container.Update(cdNewEntity);
                        
                        recordSaved = true;
                        SendLine(container, "{0:000} Updated: {1}", pos, identifier);
                    }
                    else
                    {
                        SendLine(container, "{0:000} TEST Skipped: {1} (Identical)", pos, identifier);
                    }
                }
                else
                {
                    SendLine(container, "{0:000} Inactive: {1}", pos, identifier);
                }
                if (newOwner != null && !newOwner.Equals(cdMatchEntity.GetAttribute("ownerid", new EntityReference())))
                {
                    container.Principal(newOwner).On(cdNewEntity).Assign();
                    SendLine(container, "{0:000} Assigned: {1} to {2} {3}", pos, identifier, newOwner.LogicalName, string.IsNullOrEmpty(newOwner.Name) ? newOwner.Id.ToString() : newOwner.Name);
                }
            }
            if (newActive != nowActive)
            {   // Aktiv skall ändras på posten
                var newStatusValue = newStatus.Value;
                if (cdNewEntity.LogicalName == "savedquery" && newState.Value == 1 && newStatusValue == 1)
                {   // Justering för inaktiverad men ej publicerad vy
                    newStatusValue = 2;
                }
                if (cdNewEntity.LogicalName == "duplicaterule")
                {
                    if (newStatusValue == 2)
                    {
                        cdNewEntity.PublishDuplicateRule(container);
                        SendLine(container, "{0:000} Publish Duplicate Rule: {1}", pos, identifier);
                    }
                    else
                    {
                        cdNewEntity.UnpublishDuplicateRule(container);
                        SendLine(container, "{0:000} Unpublish Duplicate Rule: {1}", pos, identifier);
                    }
                }
                else
                {
                    container.SetState(cdNewEntity, newState.Value, newStatusValue);
                    SendLine(container, "{0:000} SetState: {1}: {2}/{3}", pos, identifier, newState.Value, newStatus.Value);
                }

            }
            container.Logger.EndSection();
            return recordSaved;
        }

        #endregion Private Methods
    }

    internal static class DuplicateRuleExt
    {
        public static void UnpublishDuplicateRule(this Entity duplicateRule, IContainable container) => container.Service.Execute(new UnpublishDuplicateRuleRequest { DuplicateRuleId = duplicateRule.Id });
        public static void PublishDuplicateRule(this Entity duplicateRule, IContainable container) => container.Service.Execute(new PublishDuplicateRuleRequest { DuplicateRuleId = duplicateRule.Id });
    }
}
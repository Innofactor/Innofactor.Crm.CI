using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Shuffle.Types;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace Cinteros.Crm.Utils.Shuffle
{
    public partial class Shuffler
    {
        #region Private Methods

        private static bool EntityAttributesEqual(List<string> matchattributes, CintDynEntity entity1, CintDynEntity entity2)
        {
            var match = true;
            foreach (var attr in matchattributes)
            {
                var srcvalue = "";
                if (attr == entity1.PrimaryIdAttribute)
                {
                    srcvalue = entity1.Id.ToString();
                }
                else
                {
                    srcvalue = entity1.PropertyAsBaseType(attr, "<null>", false, false, true).ToString();
                }
                var trgvalue = entity2.PropertyAsBaseType(attr, "<null>", false, false, true).ToString();
                if (srcvalue != trgvalue)
                {
                    match = false;
                    break;
                }
            }
            return match;
        }

        private static string GetEntityDisplayString(DataBlockImportMatch match, CintDynEntity cdEntity)
        {
            var unique = new List<string>();
            if (match != null && match.Attribute.Length > 0)
            {
                foreach (var attribute in match.Attribute)
                {
                    string matchdisplay = attribute.Display;
                    if (string.IsNullOrEmpty(matchdisplay))
                    {
                        matchdisplay = attribute.Name;
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

        private CintDynEntityCollection GetAllRecordsForMatching(List<string> allattributes, CintDynEntity cdEntity)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            QueryExpression qMatch = new QueryExpression(cdEntity.Name);
            qMatch.ColumnSet = new ColumnSet(allattributes.ToArray());
#if DEBUG
            log.Log("Retrieving all records for {0}:\n{1}", cdEntity.Name, CintQryExp.ConvertToFetchXml(qMatch, crmsvc));
#endif
            CintDynEntityCollection matches = CintDynEntity.RetrieveMultiple(crmsvc, qMatch, log);
            SendLine("Pre-retrieved {0} records for matching", matches.Count);
            log.EndSection();
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

        private CintDynEntityCollection GetMatchingRecords(CintDynEntity cdEntity, List<string> matchattributes, List<string> updateattributes, bool preretrieveall, ref CintDynEntityCollection cAllRecordsToMatch)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            CintDynEntityCollection matches = null;
            var allattributes = new List<string>();
            allattributes.Add(cdEntity.PrimaryIdAttribute);
            if (cdEntity.Contains("ownerid"))
            {
                allattributes.Add("ownerid");
            }
            if (cdEntity.Contains("statecode") || cdEntity.Contains("statuscode"))
            {
                allattributes.Add("statecode");
                allattributes.Add("statuscode");
            }
            allattributes = allattributes.Union(matchattributes.Union(updateattributes)).ToList();
            if (preretrieveall)
            {
                if (cAllRecordsToMatch == null)
                {
                    cAllRecordsToMatch = GetAllRecordsForMatching(allattributes, cdEntity);
                }
                matches = GetMatchingRecordsFromPreRetrieved(matchattributes, cdEntity, cAllRecordsToMatch);
            }
            else
            {
                QueryExpression qMatch = new QueryExpression(cdEntity.Name);
                // We need to be able to see if any attributes have changed, so lets make sure matching records have all the attributes that will be updated
                qMatch.ColumnSet = new ColumnSet(allattributes.ToArray());

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

        private CintDynEntityCollection GetMatchingRecordsFromPreRetrieved(List<string> matchattributes, CintDynEntity cdEntity, CintDynEntityCollection cAllRecordsToMatch)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            log.Log("Searching matches for: {0} {1}", cdEntity.Id, cdEntity);
            var result = new CintDynEntityCollection();
            foreach (var cdRecord in cAllRecordsToMatch)
            {
                if (EntityAttributesEqual(matchattributes, cdEntity, cdRecord))
                {
                    result.Add(cdRecord);
                    log.Log("Found match: {0} {1}", cdRecord.Id, cdRecord);
                }
            }
            log.Log("Returned matches: {0}", result.Count);
            log.EndSection();
            return result;
        }

        private List<string> GetUpdateAttributes(CintDynEntityCollection entities)
        {
            var result = new List<string>();
            foreach (var entity in entities)
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

        private Tuple<int, int, int, int, int, EntityReferenceCollection> ImportDataBlock(DataBlock block, CintDynEntityCollection cEntities)
        {
            log.StartSection("ImportDataBlock");
            int created = 0;
            int updated = 0;
            int skipped = 0;
            int deleted = 0;
            int failed = 0;
            EntityReferenceCollection references = new EntityReferenceCollection();

            string name = block.Name;
            log.Log("Block: {0}", name);
            SendStatus(name, null);
            SendLine();

            if (block.Import != null)
            {
                var includeid = block.Import.CreateWithId;
                var save = block.Import.Save;
                var delete = block.Import.Delete;
                var updateinactive = block.Import.UpdateInactive;
                var updateidentical = block.Import.UpdateIdentical;
                if (block.Import.OverwriteSpecified)
                {
                    SendLine("DEPRECATED use of attribute Overwrite!");
                    save = block.Import.Overwrite ? SaveTypes.CreateUpdate : SaveTypes.CreateOnly;
                }
                var matchattributes = GetMatchAttributes(block.Import.Match);
                var updateattributes = !updateidentical ? GetUpdateAttributes(cEntities) : new List<string>();
                var preretrieveall = (bool)block.Import.Match?.PreRetrieveAll;

                SendLine();
                SendLine("Importing block {0} - {1} records ", name, cEntities.Count);

                var i = 1;

                if (delete == DeleteTypes.All && (matchattributes.Count == 0))
                {   // All records shall be deleted, no match attribute defined, so just get all and delete all
                    string entity = block.Entity;
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
                        unique = GetEntityDisplayString(block.Import.Match, cdEntity);
                        SendStatus(null, unique);

                        if (!block.TypeSpecified || block.Type == EntityTypes.Entity)
                        {
                            #region Entity

                            if (matchattributes.Count == 0)
                            {
                                if (save == SaveTypes.Never || save == SaveTypes.UpdateOnly)
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
                                    if (SaveEntity(cdEntity, null, updateinactive, updateidentical, i, unique))
                                    {
                                        created++;
                                        newid = cdEntity.Id;
                                        references.Add(cdEntity.Entity.ToEntityReference());
                                    }
                                }
                            }
                            else
                            {
                                var matches = GetMatchingRecords(cdEntity, matchattributes, updateattributes, preretrieveall, ref cAllRecordsToMatch);
                                if (delete == DeleteTypes.All || (matches.Count == 1 && delete == DeleteTypes.Existing))
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
                                    if (save == SaveTypes.Never || save == SaveTypes.UpdateOnly)
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
                                        if (SaveEntity(cdEntity, null, updateinactive, updateidentical, i, unique))
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
                                    if (save == SaveTypes.CreateUpdate || save == SaveTypes.UpdateOnly)
                                    {
                                        if (SaveEntity(cdEntity, match, updateinactive, updateidentical, i, unique))
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

                            #endregion Entity
                        }
                        else if (block.Type == EntityTypes.Intersect)
                        {
                            #region Intersect

                            if (cdEntity.Attributes.Count != 2)
                            {
                                throw new ArgumentOutOfRangeException("Attributes", cdEntity.Attributes.Count, "Invalid Attribute count for intersect object");
                            }
                            string intersect = block.IntersectName;
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

                            #endregion Intersect
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
            log.EndSection();
            return new Tuple<int, int, int, int, int, EntityReferenceCollection>(created, updated, skipped, deleted, failed, references);
        }

        private void ReplaceGuids(CintDynEntity cdEntity, bool includeid)
        {
            foreach (KeyValuePair<string, object> prop in cdEntity.Attributes)
            {
                if (prop.Value is Guid && guidmap.ContainsKey((Guid)prop.Value))
                {
                    if (includeid)
                    {
                        throw new NotImplementedException("Cannot handle replacement of Guid type attributes");
                    }
                    else
                    {
                        log.Log("No action, we don't care about the guid of the object");
                    }
                }

                if (prop.Value is EntityReference && guidmap.ContainsKey(((EntityReference)prop.Value).Id))
                {
                    ((EntityReference)prop.Value).Id = guidmap[((EntityReference)prop.Value).Id];
                }
            }
        }

        private bool SaveEntity(CintDynEntity cdNewEntity, CintDynEntity cdMatchEntity, bool updateInactiveRecord, bool updateIdentical, int pos, string identifier)
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
                    var updateattributes = cdNewEntity.Attributes.Keys.ToList();
                    if (updateattributes.Contains(cdNewEntity.PrimaryIdAttribute))
                    {
                        updateattributes.Remove(cdNewEntity.PrimaryIdAttribute);
                    }
                    if (updateIdentical || !EntityAttributesEqual(updateattributes, cdNewEntity, cdMatchEntity))
                    {
                        cdNewEntity.Update();
                        recordSaved = true;
                        SendLine("{0:000} Updated: {1}", pos, identifier);
                    }
                    else
                    {
                        SendLine("{0:000} Skipped: {1} (Identical)", pos, identifier);
                    }
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

        #endregion Private Methods
    }
}
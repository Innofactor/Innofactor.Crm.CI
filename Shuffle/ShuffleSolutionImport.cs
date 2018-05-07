using Cinteros.Crm.Utils.Common;
using Cinteros.Crm.Utils.Misc;
using Cinteros.Crm.Utils.Shuffle.Types;
using Ionic.Zip;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Cinteros.Crm.Utils.Shuffle
{
    public partial class Shuffler
    {
        #region Private Methods

        private static string ExtractErrorMessage(string message)
        {
            const string fault = "(Fault Detail is equal to Microsoft.Xrm.Sdk.OrganizationServiceFault).: ";
            const string unhandled = "Unhandled Exception: ";
            while (message.Contains(fault))
            {
                message = message.Substring(message.IndexOf(fault, StringComparison.InvariantCultureIgnoreCase) + fault.Length);
            }
            message = message.Replace("&lt;", "<").Replace("&gt;", ">");
            while (message.Contains("<InnerFault>") && message.Contains("</InnerFault>"))
            {
                message = message.Substring(message.IndexOf("<InnerFault>", StringComparison.InvariantCultureIgnoreCase) + 9);
                message = message.Substring(0, message.LastIndexOf("</InnerFault>", StringComparison.InvariantCultureIgnoreCase));
            }
            if (message.Contains("<Message>") && message.Contains("</Message>"))
            {
                message = message.Substring(message.IndexOf("<Message>", StringComparison.InvariantCultureIgnoreCase) + 9);
                message = message.Substring(0, message.LastIndexOf("</Message>", StringComparison.InvariantCultureIgnoreCase));
            }
            else if (message.StartsWith(unhandled))
            {
                message = message.Substring(message.IndexOf(unhandled, StringComparison.InvariantCultureIgnoreCase) + unhandled.Length);
                if (message.Contains(":"))
                {
                    message = message.Split(':')[0];
                }
            }
            return message;
        }

        private SolutionImportConditions CheckIfImportRequired(SolutionBlockImport import, string name, Version thisversion)
        {
            log.StartSection("CheckIfImportRequired");
            SolutionImportConditions result = SolutionImportConditions.Create;
            bool overwritesame = import.OverwriteSameVersion;
            bool overwritenewer = import.OverwriteNewerVersion;
            var cSolutions = GetExistingSolutions();
            foreach (var cdSolution in cSolutions)
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

        private bool DoImportSolution(SolutionBlockImport import, string filename, Version version)
        {
            log.StartSection(MethodBase.GetCurrentMethod().Name);
            var result = false;
            bool activatecode = import.ActivateServersideCode;
            bool overwrite = import.OverwriteCustomizations;
            Exception ex = null;
            SendLine("Importing solution: {0} Version: {1}", filename, version);
            byte[] fileBytes = File.ReadAllBytes(filename);
            var impSolReq = new ImportSolutionRequest()
            {
                CustomizationFile = fileBytes,
                OverwriteUnmanagedCustomizations = overwrite,
                PublishWorkflows = activatecode,
                ImportJobId = Guid.NewGuid()
            };
            if (crmsvc is CrmServiceProxy && ((CrmServiceProxy)crmsvc).CrmVersion.Major >= 6)
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
            var asyncRequest = new ExecuteAsyncRequest()
            {
                Request = impSolReq
            };
            var asyncResponse = crmsvc.Execute(asyncRequest) as ExecuteAsyncResponse;
            var asyncJobId = asyncResponse.AsyncJobId;
            var end = DateTime.MaxValue;
            var importStatus = -1;
            var progress = 0;
            var statustext = "Submitting job";
            SendLineUpdate("Import status: {0}", statustext);
            while (end >= DateTime.Now)
            {
                CintDynEntity cdAsyncOperation = null;
                try
                {
                    cdAsyncOperation = CintDynEntity.Retrieve(SystemJob.EntityName, asyncJobId,
                        new ColumnSet(SystemJob.PrimaryKey, SystemJob.Status, SystemJob.StatusReason, SystemJob.Message, SystemJob.Friendlymessage), crmsvc, log);
                }
                catch (Exception asyncex)
                {
                    cdAsyncOperation = null;
                    log.Log(asyncex);
                    log.EndSection();   // Ending section started by Retrieve above to prevent indentation inflation
                }
                if (cdAsyncOperation != null)
                {
                    statustext = cdAsyncOperation.PropertyAsString(SystemJob.StatusReason, "?", false, false);
                    var newStatus = cdAsyncOperation.Property(SystemJob.StatusReason, new OptionSetValue()).Value;
                    if (newStatus != importStatus)
                    {
                        importStatus = newStatus;
                        if (end.Equals(DateTime.MaxValue) && importStatus != (int)SystemJob.StatusReason_OptionSet.Waiting)
                        {
                            end = timeout > 0 ? DateTime.Now.AddMinutes(timeout) : DateTime.Now.AddMinutes(2);
                            SendLineUpdate("Import job picked up at {0}", DateTime.Now);
                            log.Log("Timout until: {0}", end.ToString("HH:mm:ss.fff"));
                            SendLine("Import status: {0}", statustext);
                        }
                        SendLineUpdate("Import status: {0}", statustext);
                        log.Log("Import message:\n{0}", cdAsyncOperation.Property(SystemJob.Message, "<none>"));
                        if (importStatus == (int)SystemJob.StatusReason_OptionSet.Succeeded)
                        {   // Succeeded
                            result = true;
                            break;
                        }
                        else if (importStatus == (int)SystemJob.StatusReason_OptionSet.Pausing
                            || importStatus == (int)SystemJob.StatusReason_OptionSet.Canceling
                            || importStatus == (int)SystemJob.StatusReason_OptionSet.Failed
                            || importStatus == (int)SystemJob.StatusReason_OptionSet.Canceled)
                        {   // Error statuses
                            var friendlymessage = cdAsyncOperation.Property(SystemJob.Friendlymessage, "");
                            SendLine("Message: {0}", friendlymessage);
                            if (friendlymessage == "Access is denied.")
                            {
                                SendLine("When importing to onprem environment, the async service user must be granted read/write permission to folder:");
                                SendLine("  C:\\Program Files\\Microsoft Dynamics CRM\\CustomizationImport");
                            }
                            else
                            {
                                var message = cdAsyncOperation.Property(SystemJob.Message, "<none>");
                                message = ExtractErrorMessage(message);
                                if (!string.IsNullOrWhiteSpace(message) && !message.Equals(friendlymessage, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    SendLine("Detailed message: \n{0}", message);
                                }
                                else
                                {
                                    SendLine("See log file for technical details.");
                                }
                            }
                            ex = new Exception(string.Format("Solution Import Failed: {0} - {1}",
                                cdAsyncOperation.PropertyAsString(SystemJob.Status, "?", false, false),
                                cdAsyncOperation.PropertyAsString(SystemJob.StatusReason, "?", false, false)));
                            break;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
                if (importStatus == 20)
                {   // In progress, read percent
                    try
                    {
                        var job = CintDynEntity.Retrieve(ImportJob.EntityName, impSolReq.ImportJobId, new ColumnSet(ImportJob.Progress), crmsvc, log);
                        if (job != null)
                        {
                            var newProgress = Convert.ToInt32(Math.Round(job.Property(ImportJob.Progress, 0D)));
                            if (newProgress > progress)
                            {
                                progress = newProgress;
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

        private string GetSolutionFilename(SolutionBlock block)
        {
            log.StartSection("GetSolutionFilename");
            string file = block.File;
            if (string.IsNullOrWhiteSpace(file))
            {
                file = block.Name;
            }
            string path = block.Path;
            if (string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(definitionpath))
            {
                path = definitionpath;
            }
            path += path.EndsWith("\\") ? "" : "\\";
            string filename;
            if (block.Import.Type == SolutionTypes.Managed)
            {
                filename = path + file + "_managed.zip";
            }
            else if (block.Import.Type == SolutionTypes.Unmanaged)
            {
                filename = path + file + ".zip";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Type", block.Import.Type, "Invalid Solution type");
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

        private ItemImportResult ImportSolutionBlock(SolutionBlock block)
        {
            log.StartSection("ImportSolutionBlock");
            var importResult = ItemImportResult.None;
            if (block.Import != null)
            {
                var name = block.Name;
                log.Log("Block: {0}", name);
                SendStatus(name, null);
                SendLine();
                SendLine("Importing solution: {0}", name);

                var filename = GetSolutionFilename(block);
                var version = ExtractVersionFromSolutionZip(filename);
                try
                {
                    ValidatePreReqs(block.Import, version);
                    SolutionImportConditions ImportCondition = CheckIfImportRequired(block.Import, name, version);
                    if (ImportCondition != SolutionImportConditions.Skip)
                    {
                        if (DoImportSolution(block.Import, filename, version))
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
                        bool publish = block.Import.PublishAll;
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

        private bool ReadAndLogSolutionImportJobStatus(Guid jobid)
        {
            log.StartSection("ReadAndLogSolutionImportJobStatus " + jobid);
            var success = false;
            var job = CintDynEntity.Retrieve("importjob", jobid, new ColumnSet("startedon", "completedon", "progress", "data"), crmsvc, log);
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

        private void ValidatePreReqs(SolutionBlockImport import, Version thisversion)
        {
            if (import.PreRequisites == null)
            {
                log.Log("No prereqs for solution import");
                return;
            }
            log.StartSection("ValidatePreReqs");
            var cSolutions = GetExistingSolutions();
            foreach (var prereq in import.PreRequisites)
            {
                var valid = false;
                var name = prereq.Name;
                var comparer = prereq.Comparer;
                var version = new Version();
                log.Log("Prereq: {0} {1} {2}", name, comparer, version);

                if (comparer == SolutionVersionComparers.eqthis || comparer == SolutionVersionComparers.gethis)
                {
                    version = thisversion;
                    comparer = comparer == SolutionVersionComparers.eqthis ? SolutionVersionComparers.eq : comparer == SolutionVersionComparers.gethis ? SolutionVersionComparers.ge : comparer;
                }
                else if (comparer != SolutionVersionComparers.any)
                {
                    version = new Version(prereq.Version.Replace('*', '0'));
                }

                foreach (CintDynEntity cdSolution in cSolutions)
                {
                    if (cdSolution.Property("uniquename", "") == name)
                    {
                        log.Log("Found matching solution");
                        switch (comparer)
                        {
                            case SolutionVersionComparers.any:
                                valid = true;
                                break;

                            case SolutionVersionComparers.eq:
                                valid = new Version(cdSolution.Property("version", "1.0.0.0")).Equals(version);
                                break;

                            case SolutionVersionComparers.ge:
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
            log.EndSection();
        }
    }

    #endregion Private Methods
}

namespace Cinteros.Crm.Utils.Shuffle
{
    using Cinteros.Crm.Utils.Common;
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Cinteros.Crm.Utils.Misc;
    using Cinteros.Crm.Utils.Shuffle.Types;
    using Cinteros.Crm.Utils.Common.Slim;
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

    public partial class Shuffler
    {
        #region Private Methods

        private static string ExtractErrorMessage(IContainable container, string message)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
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
            container.EndSection();
            return message;
        }

        private SolutionImportConditions CheckIfImportRequired(IContainable container, SolutionBlockImport import, string name, Version thisversion)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            var result = SolutionImportConditions.Create;
            var overwritesame = import.OverwriteSameVersion;
            var overwritenewer = import.OverwriteNewerVersion;
            var cSolutions = GetExistingSolutions(container);
            foreach (var cdSolution in cSolutions.Entities)
            {
                if (cdSolution.GetAttribute("uniquename", "") == name)
                {   // Now we have found the same solution in target environment
                    result = SolutionImportConditions.Update;
                    var existingversion = new Version(cdSolution.GetAttribute("version", "1.0.0.0"));
                    container.Logger.Log("Existing solution has version: {0}", existingversion);
                    var comparison = thisversion.CompareTo(existingversion);
                    if (!overwritesame && comparison == 0)
                    {
                        result = SolutionImportConditions.Skip;
                        SendLine(container, "Solution {0} {1} already exists in target", name, thisversion);
                    }
                    else if (!overwritenewer && comparison < 0)
                    {
                        result = SolutionImportConditions.Skip;
                        SendLine(container, "Existing solution {0} {1} is newer than {2}", name, existingversion, thisversion);
                    }
                    else if (existingversion == thisversion)
                    {
                        SendLine(container, "Updating version {0}", thisversion);
                    }
                    else
                    {
                        SendLine(container, "Replacing version {0} with {1}", existingversion, thisversion);
                    }
                    break;
                }
            }
            container.Logger.Log("Import Condition: {0}", result);
            container.Logger.EndSection();
            return result;
        }

        private bool DoImportSolution(IContainable container, SolutionBlockImport import, string filename, Version version)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            var result = false;
            var activatecode = import.ActivateServersideCode;
            var overwrite = import.OverwriteCustomizations;
            Exception ex = null;
            SendLine(container, "Importing solution: {0} Version: {1}", filename, version);
            var fileBytes = File.ReadAllBytes(filename);
            var impSolReq = new ImportSolutionRequest()
            {
                CustomizationFile = fileBytes,
                OverwriteUnmanagedCustomizations = overwrite,
                PublishWorkflows = activatecode,
                ImportJobId = Guid.NewGuid()
            };
            if (container.Service is CrmServiceProxy && ((CrmServiceProxy)container.Service).CrmVersion.Major >= 6)
            {   // CRM 2013 or later, import async
                result = DoImportSolutionAsync(container, impSolReq, ref ex);
            }
            else
            {   // Pre CRM 2013, import sync
                result = DoImportSolutionSync(container, impSolReq, ref ex);
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
            container.Logger.Log("Returning: {0}", result);
            container.Logger.EndSection();
            return result;
        }

        private bool DoImportSolutionAsync(IContainable container, ImportSolutionRequest impSolReq, ref Exception ex)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            // Code cred to Wael Hamze
            // http://waelhamze.com/2013/11/17/asynchronous-solution-import-dynamics-crm-2013/
            var result = false;
            var asyncRequest = new ExecuteAsyncRequest()
            {
                Request = impSolReq
            };
            var asyncResponse = container.Service.Execute(asyncRequest) as ExecuteAsyncResponse;
            var asyncJobId = asyncResponse.AsyncJobId;
            var end = DateTime.MaxValue;
            var importStatus = -1;
            var progress = 0;
            var statustext = "Submitting job";
            SendLineUpdate(container, "Import status: {0}", statustext);
            while (end >= DateTime.Now)
            {
                Entity cdAsyncOperation = null;
                try
                {
                    cdAsyncOperation = container.Retrieve( SystemJob.EntityName, asyncJobId,
                        new ColumnSet(SystemJob.PrimaryKey, SystemJob.Status, SystemJob.StatusReason, SystemJob.Message, SystemJob.Friendlymessage));
                }
                catch (Exception asyncex)
                {
                    cdAsyncOperation = null;
                    container.Logger.Log(asyncex);
                    container.Logger.EndSection();   // Ending section started by Retrieve above to prevent indentation inflation
                }
                if (cdAsyncOperation != null)
                {
                    statustext = cdAsyncOperation.PropertyAsString(SystemJob.StatusReason, "?", false);
                    var newStatus = cdAsyncOperation.GetAttribute(SystemJob.StatusReason, new OptionSetValue()).Value;
                    if (newStatus != importStatus)
                    {
                        importStatus = newStatus;
                        if (end.Equals(DateTime.MaxValue) && importStatus != (int)SystemJob.StatusReason_OptionSet.Waiting)
                        {
                            end = timeout > 0 ? DateTime.Now.AddMinutes(timeout) : DateTime.Now.AddMinutes(2);
                            SendLineUpdate(container, "Import job picked up at {0}", DateTime.Now);
                            container.Logger.Log("Timout until: {0}", end.ToString("HH:mm:ss.fff"));
                            SendLine(container, "Import status: {0}", statustext);
                        }
                        SendLineUpdate(container, "Import status: {0}", statustext);
                        container.Logger.Log("Import message:\n{0}", cdAsyncOperation.GetAttribute(SystemJob.Message, "<none>"));
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
                            var friendlymessage = cdAsyncOperation.GetAttribute(SystemJob.Friendlymessage, "");
                            SendLine(container, "Message: {0}", friendlymessage);
                            if (friendlymessage == "Access is denied.")
                            {
                                SendLine(container, "When importing to onprem environment, the async service user must be granted read/write permission to folder:");
                                SendLine(container, "  C:\\Program Files\\Microsoft Dynamics CRM\\CustomizationImport");
                            }
                            else
                            {
                                var message = cdAsyncOperation.GetAttribute(SystemJob.Message, "<none>");
                                message = ExtractErrorMessage(container, message);
                                if (!string.IsNullOrWhiteSpace(message) && !message.Equals(friendlymessage, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    SendLine(container, "Detailed message: \n{0}", message);
                                }
                                else
                                {
                                    SendLine(container, "See log file for technical details.");
                                }
                            }
                            ex = new Exception(string.Format("Solution Import Failed: {0} - {1}",
                               cdAsyncOperation.PropertyAsString(SystemJob.Status, "?", false),
                               cdAsyncOperation.PropertyAsString(SystemJob.StatusReason, "?", false)));
                            break;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
                if (importStatus == 20)
                {   // In progress, read percent
                    try
                    {
                        var job = container.Retrieve(ImportJob.EntityName, impSolReq.ImportJobId, new ColumnSet(ImportJob.Progress));
                        if (job != null)
                        {
                            var newProgress = Convert.ToInt32(Math.Round(job.GetAttribute(ImportJob.Progress, 0D)));
                            if (newProgress > progress)
                            {
                                progress = newProgress;
                                SendStatus(-1, -1, 100, progress);
                                SendLineUpdate(container, "Import status: {0} - {1}%", statustext, progress);
                            }
                        }
                    }
                    catch (Exception jobex)
                    {   // We probably tried before the job was created
                        if (jobex.Message.ToUpperInvariant().Contains("DOES NOT EXIST"))
                        {
                            container.Logger.Log("Importjob not created yet or already deleted");
                        }
                        else
                        {
                            container.Logger.Log(jobex);
                        }
                        container.Logger.EndSection();   // Ending section started by Retrieve above to prevent indentation inflation
                    }
                }
            }
            if (end < DateTime.Now)
            {
                SendLine(container, "Import timed out.");
            }
            SendStatus(-1, -1, 100, 0);
            container.Logger.EndSection();
            return result;
        }

        private bool DoImportSolutionSync(IContainable container, ImportSolutionRequest impSolReq, ref Exception ex)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            bool result;
            try
            {
                container.Service.Execute(impSolReq);
            }
            catch (Exception e)
            {
                ex = e;
                SendLine(container, "Error during import: {0}", ex.Message);
            }
            finally
            {
                result = ReadAndLogSolutionImportJobStatus(container, impSolReq.ImportJobId);
            }
            container.Logger.EndSection();
            return result;
        }

        private Version ExtractVersionFromSolutionZip(IContainable container, string filename)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            using (var zip = ZipFile.Read(filename))
            {
                zip["solution.xml"].Extract(definitionpath, ExtractExistingFileAction.OverwriteSilently);
            }
            if (!System.IO.File.Exists(definitionpath + "\\solution.xml"))
            {
                throw new Exception("Unable to unzip solution.xml from file: " + filename);
            }
            var xSolution = new XmlDocument();
            xSolution.Load(definitionpath + "\\solution.xml");
            System.IO.File.Delete(definitionpath + "\\solution.xml");
            var xRoot = CintXML.FindChild(xSolution, "ImportExportXml");
            if (xRoot == null)
            {
                throw new XmlException("Cannot find root element ImportExportXml");
            }
            var xManifest = CintXML.FindChild(xRoot, "SolutionManifest");
            if (xManifest == null)
            {
                throw new XmlException("Cannot find element SolutionManifest");
            }
            var xVersion = CintXML.FindChild(xManifest, "Version");
            if (xVersion == null)
            {
                throw new XmlException("Cannot find element Version");
            }
            var version = new Version(xVersion.InnerText);
            container.Logger.Log("Version {0} extracted", version);
            container.Logger.EndSection();
            return version;
        }

        private string GetSolutionFilename(IContainable container, SolutionBlock block)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            var file = block.File;
            if (string.IsNullOrWhiteSpace(file))
            {
                file = block.Name;
            }
            var path = block.Path;
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
                var envvars = Environment.GetEnvironmentVariables();
                foreach (DictionaryEntry de in envvars)
                {
                    filename = filename.Replace("%" + de.Key.ToString() + "%", de.Value.ToString());
                }
            }
            container.Logger.Log("Filename: {0}", filename);
            container.Logger.EndSection();
            return filename;
        }

        private ItemImportResult ImportSolutionBlock(IContainable container, SolutionBlock block)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name}");
            var importResult = ItemImportResult.None;
            if (block.Import != null)
            {
                var name = block.Name;
                container.Logger.Log("Block: {0}", name);
                SendStatus(name, null);
                SendLine(container);
                SendLine(container, "Importing solution: {0}", name);

                var filename = GetSolutionFilename(container, block);
                var version = ExtractVersionFromSolutionZip(container, filename);
                try
                {
                    ValidatePreReqs(container, block.Import, version);
                    var ImportCondition = CheckIfImportRequired(container, block.Import, name, version);
                    if (ImportCondition != SolutionImportConditions.Skip)
                    {
                        if (DoImportSolution(container, block.Import, filename, version))
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
                            container.Logger.Log("Failed during import");
                        }
                        var publish = block.Import.PublishAll;
                        if (publish)
                        {
                            SendLine(container, "Publishing customizations");
                            container.Service.Execute(new PublishAllXmlRequest());
                        }
                    }
                    else
                    {
                        importResult = ItemImportResult.Skipped;
                        container.Logger.Log("Skipped due to import condition");
                    }
                }
                catch (Exception ex)
                {
                    container.Logger.Log(ex);
                    importResult = ItemImportResult.Failed;
                    if (stoponerror)
                    {
                        throw ex;
                    }
                }
            }
            container.Logger.EndSection();
            return importResult;
        }

        private bool ReadAndLogSolutionImportJobStatus(IContainable container, Guid jobid)
        {
            container.StartSection($@"{MethodBase.GetCurrentMethod().DeclaringType.Name}\{MethodBase.GetCurrentMethod().Name} - {jobid}");
            var success = false;
            var job = container.Retrieve("importjob", jobid, new ColumnSet("startedon", "completedon", "progress", "data"));
            if (job != null)
            {
                var name = "?";
                var result = "?";
                var err = "";
                var start = job.GetAttribute("startedon", DateTime.MinValue);
                var complete = job.GetAttribute("completedon", DateTime.MinValue);
                var time = complete != null && start != null ? complete.Subtract(start) : new TimeSpan();
                var prog = job.GetAttribute<double>("progress", 0);
                if (job.Contains("data", true))
                {
                    var doc = new XmlDocument();
                    var data = job.GetAttribute("data", "");
                    container.Logger.Log("Job data length: {0}", data.Length);
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
                    SendLine(container, "Solution {0} imported in {1}", name, time);
                    container.Logger.Log("Result: {0}\nError:  {1}\nTime:   {2}", result, err, time);
                    success = true;
                }
                else
                {
                    SendLine(container, "Solution: {0}", name);
                    SendLine(container, "Result:   {0}", result);
                    SendLine(container, "Error:    {0}", err);
                    SendLine(container, "Progress: {0}", prog);
                    SendLine(container, "Time:     {0}", time);
                }
            }
            container.Logger.Log("Returning: {0}", success);
            container.Logger.EndSection();
            return success;
        }

        private void ValidatePreReqs(IContainable container, SolutionBlockImport import, Version thisversion)
        {
            if (import.PreRequisites == null)
            {
                container.Logger.Log("No prereqs for solution import");
                return;
            }
            container.Logger.StartSection("ValidatePreReqs");
            var cSolutions = GetExistingSolutions(container);
            foreach (var prereq in import.PreRequisites)
            {
                var valid = false;
                var name = prereq.Name;
                var comparer = prereq.Comparer;
                var version = new Version();
                container.Logger.Log("Prereq: {0} {1} {2}", name, comparer, version);

                if (comparer == SolutionVersionComparers.eqthis || comparer == SolutionVersionComparers.gethis)
                {
                    version = thisversion;
                    comparer = comparer == SolutionVersionComparers.eqthis ? SolutionVersionComparers.eq : comparer == SolutionVersionComparers.gethis ? SolutionVersionComparers.ge : comparer;
                }
                else if (comparer != SolutionVersionComparers.any)
                {
                    version = new Version(prereq.Version.Replace('*', '0'));
                }

                foreach (var cdSolution in cSolutions.Entities)
                {
                    if (cdSolution.GetAttribute("uniquename", "") == name)
                    {
                        container.Logger.Log("Found matching solution");
                        switch (comparer)
                        {
                            case SolutionVersionComparers.any:
                                valid = true;
                                break;

                            case SolutionVersionComparers.eq:
                                valid = new Version(cdSolution.GetAttribute("version", "1.0.0.0")).Equals(version);
                                break;

                            case SolutionVersionComparers.ge:
                                valid = new Version(cdSolution.GetAttribute("version", "<undefined>")) >= version;
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
                    SendLine(container, "Prerequisite {0} {1} {2} is satisfied", name, comparer, version);
                }
                else
                {
                    SendLine(container, "Prerequisite {0} {1} {2} is NOT satisfied", name, comparer, version);
                    throw new Exception("Prerequisite NOT satisfied (" + name + " " + comparer + " " + version + ")");
                }
            }
            container.Logger.EndSection();
        }
    }

    #endregion Private Methods
}
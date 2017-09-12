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
                                var message = cdAsyncOperation.Property("message", "<none>");
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

        private static string ExtractErrorMessage(string message)
        {
            const string fault = "(Fault Detail is equal to Microsoft.Xrm.Sdk.OrganizationServiceFault).: ";
            const string unhandled = "Unhandled Exception: ";
            while (message.Contains(fault))
            {
                message = message.Substring(message.IndexOf(fault) + fault.Length);
            }
            message = message.Replace("&lt;", "<").Replace("&gt;", ">");
            while (message.Contains("<InnerFault>") && message.Contains("</InnerFault>"))
            {
                message = message.Substring(message.IndexOf("<InnerFault>") + 9);
                message = message.Substring(0, message.LastIndexOf("</InnerFault>"));
            }
            if (message.Contains("<Message>") && message.Contains("</Message>"))
            {
                message = message.Substring(message.IndexOf("<Message>") + 9);
                message = message.Substring(0, message.LastIndexOf("</Message>"));
            }
            else if (message.StartsWith(unhandled))
            {
                message = message.Substring(message.IndexOf(unhandled) + unhandled.Length);
                if (message.Contains(":"))
                {
                    message = message.Split(':')[0];
                }
            }
            return message;
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
    }
}

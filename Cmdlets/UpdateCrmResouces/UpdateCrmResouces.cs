namespace Innofactor.Crm.CI.Cmdlets
{
    using Innofactor.Crm.CI.Cmdlets.Vendor;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Xml;

    [Cmdlet(VerbsData.Update, "CrmResources")]
    [OutputType(typeof(XmlDocument))]
    public class UpdateCrmResouces : XrmCmdletBase
    {
        #region Public Properties

        [Parameter(
            Position = 2,
            HelpMessage = "File/folder include/exclude pattern"
        ), Alias("P")]
        public string Pattern { get; set; }

        [Parameter(
            Position = 2,
            HelpMessage = "File path for include/exclude patterns"
        ), Alias("PF")]
        public string PatternFile { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Publisher prefix to add to the web resource path"
        ), Alias("Pre")]
        public string Prefix { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Path to folder with web resources"
        ), Alias("R")]
        public string RootFolder { get; set; }

        [Parameter(
            HelpMessage = "Set this to allow updating managed web resources"
        ), Alias("UM")]
        public bool UpdateManaged { get; set; } = false;

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            var files = GetWebResourcesFromDisk();
            UpdateWebResources(files);
        }

        #endregion Protected Methods

        #region Private Methods

        private static void ExtractFilePatterns(List<string> includepatterns, List<string> excludepatterns, string patterns)
        {
            var patternparts = patterns.Contains(";") ? patterns.Split(';').Select(p => p.Trim()) : patterns.Split('\n').Select(p => p.Trim());
            foreach (var patternpart in patternparts)
            {
                if (patternpart.StartsWith("!"))
                {
                    excludepatterns.Add(patternpart.Substring(1));
                }
                else
                {
                    includepatterns.Add(patternpart);
                }
            }
        }

        private string GetCrmPath(string localfile)
        {
            if (!localfile.StartsWith(RootFolder, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileLoadException("File not under root folder", localfile);
            }

            var crmpath = Prefix + (Prefix.EndsWith("_") ? "" : "_") + "/";
            crmpath += localfile.Replace(RootFolder.EndsWith("\\") ? RootFolder : RootFolder + "\\", "").Replace("\\", "/");
            return crmpath;
        }

        private List<string> GetWebResourcesFromDisk()
        {
            var patterns = "**\\*.*";
            var includepatterns = new List<string>();
            var excludepatterns = new List<string>();
            if (string.IsNullOrEmpty(Pattern) && !string.IsNullOrEmpty(PatternFile))
            {
                patterns = File.ReadAllText(PatternFile);
            }
            else
            {
                patterns = Pattern;
            }
            ExtractFilePatterns(includepatterns, excludepatterns, patterns);

            var di = new DirectoryInfo(RootFolder);

            var resourcefiles = new List<string>();
            foreach (var pattern in includepatterns.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var filepattern = pattern;
                var option = SearchOption.TopDirectoryOnly;
                if (pattern.StartsWith("**\\"))
                {
                    option = SearchOption.AllDirectories;
                    filepattern = pattern.Substring(3);
                }
                WriteVerbose($"Including files matching \"{filepattern}\"");
                try
                {
                    var files = di.GetFiles(filepattern, option);
                    WriteObject($"Including {files.Count()} files matching \"{filepattern}\"");
                    resourcefiles.AddRange(files.Select(f => f.FullName));
                }
                catch (DirectoryNotFoundException e)
                {
                    WriteWarning(e.Message);
                }
            }

            foreach (var pattern in excludepatterns.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var filepattern = pattern;
                var option = SearchOption.TopDirectoryOnly;
                if (pattern.StartsWith("**\\"))
                {
                    option = SearchOption.AllDirectories;
                    filepattern = pattern.Substring(3);
                }
                WriteVerbose($"Excluding files matching \"{filepattern}\"");
                try
                {
                    var files = di.GetFiles(filepattern, option);
                    WriteObject($"Excluding {files.Count()} files matching \"{filepattern}\"");
                    foreach (var fileToExclude in files)
                    {
                        resourcefiles.RemoveAll(x => x.IndexOf(fileToExclude.FullName, StringComparison.InvariantCultureIgnoreCase) >= 0);
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    WriteWarning(e.Message);
                }
            }

            return resourcefiles;
        }

        private void UpdateWebResources(List<string> files)
        {
            WriteObject(string.Format("Updating {0} webresources", files.Count));

            var progress = new ProgressRecord(0, "UpdateWebResources", "Idle");
            var fileno = 0;
            var updatecount = 0;
            foreach (var file in files)
            {
                fileno++;
                var crmpath = GetCrmPath(file);
                WriteVerbose("  " + crmpath);
                progress.StatusDescription = $"Retrieving {crmpath}";
                progress.PercentComplete = (fileno * 50) / files.Count;
                WriteProgress(progress);

                var wr = WebResourceManager.RetrieveWebResource(Service, crmpath);
                if (wr == null)
                {
                    throw new ArgumentOutOfRangeException("file", crmpath, $"{crmpath} does not exist in target CRM. Make sure it is uploaded before updating.");
                }
                if (wr.Attributes["ismanaged"] as bool? ?? false)
                {
                    if (!UpdateManaged)
                    {
                        throw new ArgumentOutOfRangeException("file", crmpath, $"{crmpath} is managed in target CRM. Use parameter UpdateManaged to allow this.");
                    }
                    else
                    {
                        WriteWarning("Updating managed webresource: " + crmpath);
                    }
                }
                fileno++;
                progress.StatusDescription = $"Updating {crmpath}";
                progress.PercentComplete = (fileno * 50) / files.Count;
                WriteProgress(progress);
                var filecontent = Convert.ToBase64String(File.ReadAllBytes(file));
                if (filecontent != wr.Attributes["content"] as string)
                {
                    var updatewr = wr;
                    updatewr.Attributes.Add("content", filecontent);
                    Service.Update(updatewr);
                    WriteObject($"Updated {wr}");
                    updatecount++;
                }
                else
                {
                    WriteVerbose($"No change: {wr}");
                }
            }
            WriteObject($"Successfully updated {updatecount} webresources.");
        }

        #endregion Private Methods
    }
}
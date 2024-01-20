namespace Innofactor.Crm.CI.Cmdlets
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Management.Automation;
    using System.Web.UI.WebControls;
    using System.Xml;

    [Cmdlet(VerbsData.Update, "PluginPackage")]
    public class UpdatePluginPackage : XrmCmdletBase
    {
        #region Public Properties

        [Parameter(
           Mandatory = true,
           Position = 0,
           HelpMessage = "Package Name of the nuget package in CRM. "
       ), Alias("PackageName", "p")]
        public string PackageName { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Path to the nuget package file containing the plugin"
        ), Alias("PackageFile", "PF")]
        public string PluginPackageFile { get; set; }

        [Parameter(
            HelpMessage = "Set this to allow updating managed plugin package"
        ), Alias("UM")]
        public bool UpdateManaged { get; set; } = false;

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            var package = GetPluginPackage();
            if (package != null)
            {
                UpdatePackage(package);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private Entity GetPluginPackage()
        {
            WriteObject($"Reading plugin package file {PluginPackageFile}");
            if (!PackageName.EndsWith("_"))
            {
                PackageName += "_";
                WriteObject($"PackageName did not end with underscore, adding it. PackageName is now {PackageName}");
            }

            var query = new QueryExpression("pluginpackage");
            query.ColumnSet.AddColumns("name", "ismanaged");
            query.Criteria.AddCondition("name", ConditionOperator.Equal, PackageName);

            var pluginPackage = Service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (pluginPackage != null)
            {
                WriteObject($"Found plugin package: {pluginPackage.Attributes["name"]} {pluginPackage.Attributes["version"]}");
                if (pluginPackage.Attributes["ismanaged"] as bool? ?? false)
                {
                    if (!UpdateManaged)
                    {
                        throw new ArgumentOutOfRangeException("PackageName", PackageName, "Plugin package is managed in target CRM. Use parameter UpdateManaged to allow this.");
                    }
                    else
                    {
                        WriteWarning("Updating managed plugin package");
                    }
                }
                return pluginPackage;
            }
            else
            {
                throw new ArgumentOutOfRangeException("PackageName", PackageName, "Plugin package does not appear to be registered in CRM");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="version"></param>
        /// <param name="id"></param>
        /// <returns>Base64 string for package file or `string.Empty`</returns>
        private string ReadPackage(string fileName, out string version, out string id)
        {
            string content = string.Empty;
            id = string.Empty;
            version = string.Empty;
            using (Package p = Package.Open(fileName, FileMode.Open))
            {
                foreach (var part in p.GetParts())
                {
                    if (part.Uri.ToString().EndsWith(".nuspec"))
                    {
                        using (var stream = part.GetStream())
                        {
                            XmlTextReader xReader = new XmlTextReader(stream);
                            var doc = new XmlDocument();
                            doc.Load(xReader);

                            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                            nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");

                            var metadata = doc.SelectSingleNode("ns:package/ns:metadata", nsmgr);
                            id = metadata.SelectSingleNode("ns:id", nsmgr).InnerText;
                            version = metadata.SelectSingleNode("ns:version", nsmgr).InnerText;
                        }
                    }
                }
            }

            using (FileStream reader = new FileStream(fileName, FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    reader.CopyTo(ms);

                    content = Convert.ToBase64String(ms.ToArray());
                }
            }
            return content;
        }

        private void UpdatePackage(Entity pluginPackage)
        {
            try
            {
                WriteVerbose($"Reading plugin package file {PluginPackageFile}");
                var file = ReadPackage(PluginPackageFile, out string version, out string id);
                if (string.IsNullOrWhiteSpace(file))
                {
                    WriteError(new ErrorRecord(new ArgumentNullException("Could not read plugin package file, the file seems to be empty."), "UpdatePluginPackage", ErrorCategory.WriteError, file));
                    return;
                }
                WriteVerbose("Adding Base64String to entity");
                var updatePluginPackage = pluginPackage;

                // Update version attribute
                if (updatePluginPackage.Attributes.Contains("version"))
                {
                    updatePluginPackage.Attributes["version"] = version;
                }
                else
                {
                    updatePluginPackage.Attributes.Add("version", version);
                }

                // Update content attribute
                if (updatePluginPackage.Attributes.Contains("content"))
                {
                    updatePluginPackage.Attributes["content"] = file;
                }
                else
                {
                    updatePluginPackage.Attributes.Add("content", file);
                }
                WriteObject("Saving updated plugin package record");
                Service.Update(updatePluginPackage);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "UpdatePluginPackage", ErrorCategory.WriteError, pluginPackage));
            }
        }

        #endregion Private Methods
    }
}
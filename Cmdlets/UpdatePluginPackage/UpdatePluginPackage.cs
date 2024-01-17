using Innofactor.Crm.CI.Cmdlets;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace UpdatePluginPackage
{
    [Cmdlet(VerbsData.Update, "PluginPackage")]
    public class UpdatePluginPackage : XrmCmdletBase
    {
        #region Private Fields

        private string fileculture;

        private string filename;

        private string filetoken;

        private Version fileversion;

        #endregion Private Fields

        #region Public Properties

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Path to the nuget package file containing the plugin"
        ), Alias("Nupkg", "N")]
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
            var file = ReadFile(PluginPackageFile);
            WriteVerbose("Loading plugin package file");
            var assembly = Assembly.Load(file);
            var chunks = assembly.FullName.Split(new string[] { ", ", "Version=", "Culture=", "PublicKeyToken=" }, StringSplitOptions.RemoveEmptyEntries);
            var packageName = chunks[0];
            var packageVersion = new Version(chunks[1]);
            var packageCulture = chunks[2];
            var packageToken = chunks[3];
            WriteObject($"Loaded plugin package {packageName} {packageVersion}");
            
            var query = new QueryExpression("pluginpackage");
            query.ColumnSet.AddColumns("name", "version", "ismanaged");
            query.Criteria.AddCondition("name", ConditionOperator.Equal, packageName);
            query.Criteria.AddCondition("version", ConditionOperator.Like, packageVersion.ToString(2) + "%");
            
            var pluginPackage = Service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (pluginPackage != null)
            {
                WriteObject($"Found plugin package: {pluginPackage.Attributes["name"]} {pluginPackage.Attributes["version"]}");
                if (pluginPackage.Attributes["ismanaged"] as bool? ?? false)
                {
                    if (!UpdateManaged)
                    {
                        throw new ArgumentOutOfRangeException("PluginPackageFile", PluginPackageFile, "Plugin package is managed in target CRM. Use parameter UpdateManaged to allow this.");
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
                throw new ArgumentOutOfRangeException("PluginPackageFile", PluginPackageFile, "Plugin package does not appear to be registered in CRM");
            }
        }


        private byte[] ReadFile(string fileName)
        {
            byte[] buffer = null;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        private void UpdatePackage(Entity pluginPackage)
        {
            try
            {
                WriteVerbose($"Reading plugin package file {PluginPackageFile}");
                var file = ReadFile(PluginPackageFile);
                WriteVerbose("Adding Base64String to entity");
                var updatePluginPackage = pluginPackage;

                // Update version attribute
                if (updatePluginPackage.Attributes.Contains("version"))
                {
                    updatePluginPackage.Attributes["version"] = fileversion.ToString();
                }
                else
                {
                    updatePluginPackage.Attributes.Add("version", fileversion.ToString());
                }

                // Update content attribute
                if (updatePluginPackage.Attributes.Contains("content"))
                {
                    updatePluginPackage.Attributes["content"] = Convert.ToBase64String(file);
                }
                else
                {
                    updatePluginPackage.Attributes.Add("content", Convert.ToBase64String(file));
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
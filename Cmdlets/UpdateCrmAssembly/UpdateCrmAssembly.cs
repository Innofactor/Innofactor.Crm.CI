namespace Innofactor.Crm.CI.Cmdlets
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    [Cmdlet(VerbsData.Update, "CrmAssembly")]
    public class UpdateCrmAssembly : XrmCmdletBase
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
            HelpMessage = "Path to assembly file"
        ), Alias("DLL", "D")]
        public string AssemblyFile { get; set; }

        [Parameter(
            HelpMessage = "Set this to allow updating managed assembly"
        ), Alias("UM")]
        public bool UpdateManaged { get; set; } = false;

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            var assembly = GetAssembly();
            if (assembly != null)
            {
                UpdateAssembly(assembly);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private Entity GetAssembly()
        {
            WriteObject($"Reading assembly file {AssemblyFile}");
            var file = ReadFile(AssemblyFile);
            WriteVerbose("Loading assembly file");
            var assembly = Assembly.Load(file);

            var chunks = assembly.FullName.Split(new string[] { ", ", "Version=", "Culture=", "PublicKeyToken=" }, StringSplitOptions.RemoveEmptyEntries);
            filename = chunks[0];
            fileversion = new Version(chunks[1]);
            fileculture = chunks[2];
            filetoken = chunks[3];
            WriteObject($"Loaded assembly {filename} {fileversion}");
            WriteVerbose("Culture: " + fileculture);
            WriteVerbose("Token  : " + filetoken);

            var query = new QueryExpression("pluginassembly");
            query.ColumnSet.AddColumns("name", "version", "ismanaged");
            query.Criteria.AddCondition("name", ConditionOperator.Equal, filename);
            query.Criteria.AddCondition("version", ConditionOperator.Like, fileversion.ToString(2) + "%");
            query.Criteria.AddCondition("culture", ConditionOperator.Equal, fileculture);
            query.Criteria.AddCondition("publickeytoken", ConditionOperator.Equal, filetoken);

            var plugin = Service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (plugin != null)
            {
                WriteObject($"Found plugin: {plugin} {plugin.Attributes["version"]}");
                if (plugin.Attributes["ismanaged"] as bool? ?? false)
                {
                    if (!UpdateManaged)
                    {
                        throw new ArgumentOutOfRangeException("AssemblyFile", AssemblyFile, "Assembly is managed in target CRM. Use parameter UpdateManaged to allow this.");
                    }
                    else
                    {
                        WriteWarning("Updating managed assembly");
                    }
                }
                return plugin;
            }
            else
            {
                throw new ArgumentOutOfRangeException("AssemblyFile", AssemblyFile, "Assembly does not appear to be registered in CRM");
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

        private void UpdateAssembly(Entity plugin)
        {
            WriteVerbose("Reading assembly file " + AssemblyFile);
            var file = ReadFile(AssemblyFile);
            WriteVerbose("Adding Base64String to entity");
            var updateplugin = plugin;
            updateplugin.Attributes.Add("version", fileversion.ToString());
            updateplugin.Attributes.Add("content", Convert.ToBase64String(file));
            WriteObject("Saving updated assembly record");
            Service.Update(updateplugin);
        }

        #endregion Private Methods
    }
}
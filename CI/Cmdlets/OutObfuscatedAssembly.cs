namespace Cinteros.Crm.Utils.CI.Cmdlets
{
    using Cinteros.Crm.Utils.CI.Cmdlets.Structure;
    using Confuser.Core;
    using Confuser.Core.Project;
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;

    [Cmdlet(VerbsData.Out, "ObfuscatedAssembly")]
    public class OutObfuscatedAssembly : Cmdlet
    {
        #region Public Properties

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Path to assembly file"
        ), Alias("DLL", "D")]
        public string AssemblyFile { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Path to key file to sign assembly"
        ), Alias("Key", "K")]
        public string KeyFile { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Level of obfuscation"
        ), Alias("Level", "L")]
        public int ObfuscationLevel { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            try
            {
                var parameters = new ConfuserParameters
                {
                    Logger = new ProxyLogger(this),
                    Project = new ConfuserProject()
                };

                var rule = new Rule(preset: ProtectionPreset.Normal)
                {
                    new SettingItem<Protection>("rename", SettingItemAction.Add)
                    {
                        { "mode", "sequential" }
                    }
                };

                var module = new ProjectModule
                {
                    Path = AssemblyFile,
                    SNKeyPath = KeyFile
                };

                WriteVerbose(module.Path);
                WriteVerbose(module.SNKeyPath);

                parameters.Project.BaseDirectory = Path.GetDirectoryName(AssemblyFile);
                parameters.Project.OutputDirectory = Path.Combine(parameters.Project.BaseDirectory, "result");

                WriteVerbose(parameters.Project.BaseDirectory);
                WriteVerbose(parameters.Project.OutputDirectory);

                if (!Directory.Exists(parameters.Project.OutputDirectory))
                {
                    Directory.CreateDirectory(parameters.Project.OutputDirectory);
                }

                parameters.Project.Add(module);
                parameters.Project.Rules.Add(rule);

                WriteVerbose(ObfuscationLevel.ToString());

                ConfuserEngine.Run(parameters).Wait();
            }
            catch (Exception ex)
            {
                WriteVerbose(ex.Message);
                WriteVerbose(ex.StackTrace);

                System.Diagnostics.Debugger.Launch();
            }
        }

        #endregion Protected Methods
    }
}
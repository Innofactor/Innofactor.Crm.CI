namespace Innofactor.Crm.CI
{
    using Confuser.Core;
    using Confuser.Core.Project;
    using Innofactor.Crm.CI.Structure;
    using System;
    using System.IO;
    using System.Management.Automation;

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
        ), Alias("Obfuscation", "L")]
        public int ObfuscationLevel { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void ProcessRecord()
        {
            try
            {
                var parameters = new ConfuserParameters
                {
                    Logger = new ConfuserLogger(),
                    Project = new ConfuserProject()
                };

                parameters.Project.BaseDirectory = Path.GetDirectoryName(AssemblyFile);
                parameters.Project.OutputDirectory = Path.GetDirectoryName(AssemblyFile);

                if (!Directory.Exists(parameters.Project.OutputDirectory))
                {
                    Directory.CreateDirectory(parameters.Project.OutputDirectory);
                }

                var module = new ProjectModule
                {
                    Path = AssemblyFile
                };

                if (!string.IsNullOrEmpty(KeyFile))
                {
                    module.SNKeyPath = KeyFile;
                }

                parameters.Project.Add(module);

                // Having no protection doesn't make any sense, so recommended settings will be used
                var rule = ((ProtectionPreset)ObfuscationLevel == ProtectionPreset.None) ?
                    new Rule
                    {
                        new SettingItem<Protection>("rename", SettingItemAction.Add)
                        {
                            { "mode", "sequential" },
                            { "forceRen", "true" }
                        },

                        new SettingItem<Protection>("ctrl flow", SettingItemAction.Add)
                        {
                            { "intensity", "30" }
                        },

                        new SettingItem<Protection>("constants", SettingItemAction.Add)
                        {
                            { "decoderCount", "10" },
                            { "elements", "SNPI" }
                        }
                    }
                    : new Rule(preset: (ProtectionPreset)ObfuscationLevel);

                parameters.Project.Rules.Add(rule);

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
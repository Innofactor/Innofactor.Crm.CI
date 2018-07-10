namespace Cinteros.Crm.Utils.CI.Cmdlets
{
    using Cinteros.Crm.Utils.CI.Cmdlets.Structure;
    using Confuser.Core;
    using System.Management.Automation;

    [Cmdlet(VerbsData.Out, "ObfuscatedAssembly")]
    internal class OutObfuscatedAssembly : Cmdlet
    {
        #region Protected Methods

        protected override void ProcessRecord()
        {
            var parameters = new ConfuserParameters
            {
                Logger = new ProxyLogger(this)
            };

            ConfuserEngine.Run(parameters).Wait();
        }

        #endregion Protected Methods
    }
}
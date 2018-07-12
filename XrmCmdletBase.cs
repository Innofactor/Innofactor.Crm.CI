// Heavily inspired by Wael Hamze xrm-ci-framework
// https://github.com/WaelHamze/xrm-ci-framework/blob/master/CRM365/Xrm.Framework.CI/Xrm.Framework.CI.PowerShell.Cmdlets/XrmCommandBase.cs

namespace Cinteros.Crm.Utils.CI
{
    using Cinteros.Crm.Utils.Common;
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Tooling.Connector;
    using System.Management.Automation;
    using System.Net;

    public abstract class XrmCmdletBase : Cmdlet
    {
        #region Protected Fields

        protected IContainable container;

        #endregion Protected Fields

        #region Private Fields

        private int DefaultTime = 120;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// <para type="description">The connectionstring to the crm organization (see https://msdn.microsoft.com/en-us/library/mt608573.aspx ).</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string ConnectionString { get; set; }

        /// <summary>
        /// <para type="description">Timeout in seconds</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SetSecurityProtocol();
            WriteDebug("Connecting to CRM");
            WriteVerbose("Creating CrmServiceClient with: " + ConnectionString);
            var client = new CrmServiceClient(ConnectionString);
            if (client == null)
            {
                throw new PSArgumentException("Connection not established", "ConnectionString");
            }
            if (client.OrganizationServiceProxy == null)
            {
                throw new PSArgumentException("Connection not established. Last CRM error message:\n" + client.LastCrmError, "ConnectionString");
            }
            if (Timeout == 0)
            {
                client.OrganizationServiceProxy.Timeout = new System.TimeSpan(0, 0, DefaultTime);
            }
            else
            {
                client.OrganizationServiceProxy.Timeout = new System.TimeSpan(0, 0, Timeout);
            }
            container = new CintContainer(new CrmServiceProxy(client.OrganizationServiceProxy), CommandRuntime.ToString(), true);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (container != null)
            {
                (container as CintContainer)?.Dispose();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void SetSecurityProtocol()
        {
            WriteVerbose(string.Format("Current Security Protocol: {0}", ServicePointManager.SecurityProtocol));
            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls11))
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol ^ SecurityProtocolType.Tls11;
            }
            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12))
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol ^ SecurityProtocolType.Tls12;
            }
            WriteVerbose(string.Format("Modified Security Protocol: {0}", ServicePointManager.SecurityProtocol));
        }

        #endregion Private Methods
    }
}
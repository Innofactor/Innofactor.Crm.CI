// Heavily inspired by Wael Hamze xrm-ci-framework
// https://github.com/WaelHamze/xrm-ci-framework/blob/master/CRM365/Xrm.Framework.CI/Xrm.Framework.CI.PowerShell.Cmdlets/XrmCommandBase.cs

using Cinteros.Crm.Utils.Common;

namespace Cinteros.Crm.Utils.CI.Cmdlets.Structure
{
    using Microsoft.Xrm.Tooling.Connector;
    using System.Management.Automation;
    using System.Net;

    public abstract class XrmCommandBase : Cmdlet
    {
        #region Protected Fields

        protected CintContainer Container;

        #endregion Protected Fields

        #region Private Fields

        private int DefaultTime = 120;
        private CrmServiceClient ServiceClient;

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
            ServiceClient = new CrmServiceClient(ConnectionString);
            if (ServiceClient == null)
            {
                throw new PSArgumentException("Connection not established", "ConnectionString");
            }
            if (ServiceClient.OrganizationServiceProxy == null)
            {
                throw new PSArgumentException("Connection not established. Last CRM error message:\n" + ServiceClient.LastCrmError, "ConnectionString");
            }
            if (Timeout == 0)
            {
                ServiceClient.OrganizationServiceProxy.Timeout = new System.TimeSpan(0, 0, DefaultTime);
            }
            else
            {
                ServiceClient.OrganizationServiceProxy.Timeout = new System.TimeSpan(0, 0, Timeout);
            }
            Container = new CintContainer(new CrmServiceProxy(ServiceClient.OrganizationServiceProxy), CommandRuntime.ToString(), true);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (ServiceClient != null)
            {
                ServiceClient.Dispose();
            }
            if (Container != null)
            {
                Container.Dispose();
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
// Heavily inspired by Wael Hamze xrm-ci-framework
// https://github.com/WaelHamze/xrm-ci-framework/blob/master/CRM365/Xrm.Framework.CI/Xrm.Framework.CI.PowerShell.Cmdlets/XrmCommandBase.cs

namespace Innofactor.Crm.CI.Cmdlets
{
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Management.Automation;
    using System.Net;
    using System.Threading;

    public abstract class XrmCmdletBase : Cmdlet
    {
        #region Private Fields

        private readonly int DefaultTime = 120;
        private TimeSpan ConnectPolingInterval = TimeSpan.FromSeconds(15);
        private int ConnectRetryCount = 3;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// <para type="description">The connectionstring to the crm organization (see https://msdn.microsoft.com/en-us/library/mt608573.aspx ). For Service Principal (app registration) based connections see https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// <para type="description">Timeout in seconds</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Protected Properties

        internal CrmServiceClient Service
        {
            get;
            private set;
        }

        #endregion Protected Properties

        #region Protected Methods

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SetSecurityProtocol();
            WriteDebug("Connecting to CRM/Dataverse");
            WriteVerbose($"Attempting to connect with: {ConnectionString}");

            for (int i = 1; i <= ConnectRetryCount; i++)
            {
                WriteVerbose($"Connecting to CRM/Dataverse [attempt {i}]");
                Service = new CrmServiceClient(ConnectionString);

                if (Service != null && Service.IsReady)
                {
                    if (Timeout == 0)
                    {
                        CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 0, DefaultTime);
                    }
                    else
                    {
                        CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 0, Timeout);
                        WriteVerbose($"MaxConnectionTimeout set to {Timeout}");
                    }

                    return;
                }
                else
                {
                    if (i != ConnectRetryCount)
                    {
                        Thread.Sleep(ConnectPolingInterval);
                    }
                }
            }

            throw new Exception($"Couldn't connect to CRM/Dataverse instance after {ConnectRetryCount} attempts: {Service?.LastCrmError}");
        }

        protected override void EndProcessing() =>
            base.EndProcessing();

        #endregion Protected Methods

        #region Private Methods

        private void SetSecurityProtocol()
        {
            WriteVerbose($"Current Security Protocol: {ServicePointManager.SecurityProtocol}");
            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls11))
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol ^ SecurityProtocolType.Tls11;
            }
            if (!ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12))
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol ^ SecurityProtocolType.Tls12;
            }
            WriteVerbose($"Modified Security Protocol: {ServicePointManager.SecurityProtocol}");
        }

        #endregion Private Methods
    }
}
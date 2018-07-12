namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Cinteros.Crm.Utils.Common;
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Tooling.Connector;

    internal class ShuffleContainer : IContainable
    {
        #region Private Fields

        private readonly OrganizationServiceProxy service;

        #endregion Private Fields

        #region Internal Constructors

        internal ShuffleContainer(CrmServiceClient client)
        {
            service = client.OrganizationServiceProxy;
        }

        #endregion Internal Constructors

        #region Public Properties

        public ILoggable Logger => new ShuffleLogger();

        public IServicable Service => new CrmServiceProxy(service);

        #endregion Public Properties
    }
}
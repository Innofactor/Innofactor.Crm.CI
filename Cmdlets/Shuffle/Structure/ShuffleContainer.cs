namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Cinteros.Crm.Utils.Common;
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Sdk.Client;

    /// <summary>
    /// Implementation of IContainable for Shuffle
    /// </summary>
    internal class ShuffleContainer : IContainable
    {
        #region Private Fields

        private readonly OrganizationServiceProxy service;

        private readonly XrmCmdletBase cmdlet;

        #endregion Private Fields

        #region Internal Constructors

        internal ShuffleContainer(XrmCmdletBase cmdlet)
        {
            this.cmdlet = cmdlet;
            service = cmdlet.Service.OrganizationServiceProxy;
        }

        #endregion Internal Constructors

        #region Public Properties

        public ILoggable Logger => new ShuffleLogger(cmdlet);

        public IServicable Service => new CrmServiceProxy(service);

        #endregion Public Properties
    }
}
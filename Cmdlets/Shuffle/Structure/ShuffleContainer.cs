namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Innofactor.Xrm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Sdk;

    //using Cinteros.Crm.Utils.Common;
    //using Cinteros.Crm.Utils.Common.Interfaces;

    /// <summary>
    /// Implementation of IExecutionContainer for Shuffle
    /// </summary>
    internal class ShuffleContainer : IExecutionContainer //IContainable
    {
        #region Private Fields

        //private readonly IOrganizationService service;

        private readonly XrmCmdletBase cmdlet;

        #endregion Private Fields

        #region Internal Constructors

        internal ShuffleContainer(XrmCmdletBase cmdlet)
        {
            this.cmdlet = cmdlet;
            //service = cmdlet.Service;
        }

        #endregion Internal Constructors

        #region Public Properties

        //public ILoggable Logger => new ShuffleLogger(cmdlet);

        //public IServicable Service => new CrmServiceProxy(service);

        public dynamic Values
        {
            get;
        }

        ILoggable Logger => new ShuffleLogger(cmdlet);

        IOrganizationService IExecutionContainer.Service => cmdlet.Service;

        #endregion Public Properties
    }
}
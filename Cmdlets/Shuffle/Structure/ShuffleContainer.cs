namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Microsoft.Xrm.Sdk;
    using Xrm.Utils.Core.Common.Interfaces;

    /// <summary>
    /// Implementation of IExecutionContainer for Shuffle
    /// </summary>
    public class ShuffleContainer : IExecutionContainer //IContainable
    {
        #region Private Fields

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

        ILoggable IExecutionContainer.Logger => new ShuffleLogger(cmdlet);

        IOrganizationService IExecutionContainer.Service => cmdlet.Service;

        #endregion Public Properties
    }
}
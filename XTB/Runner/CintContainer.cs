namespace Innofactor.Crm.Shuffle.Runner
{
    using Innofactor.Xrm.Utils.Common.Interfaces;
    using Innofactor.Xrm.Utils.Common.Loggers;
    using Microsoft.Xrm.Sdk;
    using System;

    internal class CintContainer : IExecutionContainer
    {
        #region Private Fields

        private readonly Lazy<ILoggable> logger;

        private readonly Lazy<IOrganizationService> service;

        #endregion Private Fields

        public CintContainer(IOrganizationService service, string logpath)
        {
            this.service = new Lazy<IOrganizationService>(() => service);
            logger = new Lazy<ILoggable>(() => new FileLogger(string.Empty, logpath));
        }

        public dynamic Values
        {
            get;
        }

        public ILoggable Logger => logger.Value;

        public IOrganizationService Service => service.Value;
    }
}
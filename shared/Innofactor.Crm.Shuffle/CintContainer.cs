namespace Cinteros.Crm.Utils.Shuffle
{
    using Innofactor.Xrm.Utils.Common.Interfaces;
    using Innofactor.Xrm.Utils.Common.Loggers;
    using Microsoft.Xrm.Sdk;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class CintContainer : IExecutionContainer
    {
        #region Private Fields

        private readonly Lazy<ILoggable> logger;

        private readonly Lazy<IOrganizationService> service;

        #endregion Private Fields

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logpath">Folder path for logs</param>
        public CintContainer(IOrganizationService service, string logpath)
        {
            this.service = new Lazy<IOrganizationService>(() => service);
            logger = new Lazy<ILoggable>(() => new FileLogger(logpath));
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Values
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public ILoggable Logger => logger.Value;

        /// <summary>
        /// 
        /// </summary>
        public IOrganizationService Service => service.Value;
    }
}
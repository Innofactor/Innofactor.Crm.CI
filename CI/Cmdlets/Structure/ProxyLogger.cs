namespace Cinteros.Crm.Utils.CI.Cmdlets.Structure
{
    using Confuser.Core;
    using System;
    using System.Management.Automation;

    internal class ProxyLogger : ILogger
    {
        #region Private Fields

        private readonly Cmdlet cmdlet;

        #endregion Private Fields

        #region Public Constructors

        public ProxyLogger(Cmdlet cmdlet) =>
            this.cmdlet = cmdlet;

        #endregion Public Constructors

        #region Public Methods

        public void Debug(string msg) =>
            cmdlet.WriteDebug(msg);

        public void DebugFormat(string format, params object[] args) =>
            cmdlet.WriteDebug(string.Format(format, args));

        public void EndProgress()
        {
        }

        public void Error(string msg)
        {
        }

        public void ErrorException(string msg, Exception ex)
        {
        }

        public void ErrorFormat(string format, params object[] args)
        {
        }

        public void Finish(bool successful)
        {
        }

        public void Info(string msg) =>
            cmdlet.WriteInformation(msg, new string[] { });

        public void InfoFormat(string format, params object[] args) =>
            cmdlet.WriteInformation(string.Format(format, args), new string[] { });

        public void Progress(int progress, int overall)
        {
        }

        public void Warn(string msg)
            => cmdlet.WriteWarning(msg);

        public void WarnException(string msg, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args) =>
            cmdlet.WriteWarning(string.Format(format, args));

        #endregion Public Methods
    }
}
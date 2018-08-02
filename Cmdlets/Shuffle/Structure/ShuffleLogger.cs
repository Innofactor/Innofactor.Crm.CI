namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    /// <summary>
    /// Implementation of ILoggable for Shuffle
    /// </summary>
    internal class ShuffleLogger : ILoggable
    {
        #region Private Fields

        private readonly DateTime begin;
        private string name;
        private readonly XrmCmdletBase cmdlet;

        #endregion Private Fields

        #region Public Constructors

        public ShuffleLogger(XrmCmdletBase cmdlet)
        {
            begin = DateTime.Now;
            this.cmdlet = cmdlet;
        }

        #endregion Public Constructors

        #region Public Methods

        public void CloseLog() =>
            CloseLog(string.Empty);

        public void CloseLog(string closetext)
        {
            var now = DateTime.Now;
            var timeString = string.Format(
                "at {0}, {1}:{2:d2} elapsed.",
                now.ToShortTimeString(),
                (int)now.Subtract(begin).TotalMinutes,
                now.Subtract(begin).Seconds);
            if (!string.IsNullOrEmpty(closetext))
            {
                Console.Title = $"Shuffle - {closetext}";
            }

            cmdlet.WriteVerbose($"Finished {timeString}");
            //WriteLineWithColor(ConsoleColor.Green, "Finished " + timeString);
        }

        public void EndSection() =>
           cmdlet.WriteVerbose($" [END of {name}]");

        public void Log(string message) =>
           cmdlet.WriteDebug($" [INFO] {message}");

        public void Log(Exception ex) => cmdlet.WriteDebug(ex.Message);

        public void Log(string message, params object[] arg) =>
          cmdlet.WriteVerbose(" [INFO] " + string.Format(message, arg));

        public void LogIf(bool condition, string message)
        {
            if (condition)
            {
                Log(message);
            }
        }

        public void LogIf(bool condition, string message, params object[] arg) =>
            LogIf(condition, string.Format(message, arg));

        public string SaveEntity(IExecutionContext context, string filename, string info)
        {
            return default(string);
        }

        public string SaveEntity(Entity entity, string filename, string info)
        {
            return default(string);
        }

        public void SaveQX(IServicable service, QueryBase qry, string filename)
        {
        }

        public void StartSection(string name)
        {
            cmdlet.WriteVerbose($" [START of {name}]");
            this.name = name;
        }

        #endregion Public Methods
    }
}
namespace Innofactor.Crm.CI.Cmdlets.Structure
{
    using Cinteros.Crm.Utils.Common.Interfaces;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    internal class ShuffleLogger : ILoggable
    {
        #region Private Fields

        private readonly DateTime begin;
        private string name;

        #endregion Private Fields

        #region Public Constructors

        public ShuffleLogger()
        {
            begin = DateTime.Now;
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
                Console.Title = $"Suffle - {closetext}";
            }

            WriteLineWithColor(ConsoleColor.Green, "Finished " + timeString);
        }

        public void EndSection() =>
            WriteLineWithColor(ConsoleColor.Gray, $" [END of {name}]");

        public void Log(string message) =>
            WriteLineWithColor(ConsoleColor.White, " [INFO] " + message);

        public void Log(Exception ex) =>
            WriteLineWithColor(ConsoleColor.Red, "Exception: " + ex);

        public void Log(string message, params object[] arg) =>
            WriteLineWithColor(ConsoleColor.White, " [INFO] " + string.Format(message, arg));

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
            WriteLineWithColor(ConsoleColor.Gray, $" [START of {name}]");

            this.name = name;
        }

        #endregion Public Methods

        #region Private Methods

        private static void WriteLineWithColor(ConsoleColor color, string txt)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(txt);
            Console.ForegroundColor = original;
        }

        #endregion Private Methods
    }
}
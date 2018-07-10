namespace Cinteros.Crm.Utils.CI.Cmdlets.Structure
{
    using Confuser.Core;
    using System;

    internal class ConsoleLogger : ILogger
    {
        #region Private Fields

        private readonly DateTime begin;

        #endregion Private Fields

        #region Public Constructors

        public ConsoleLogger()
        {
            begin = DateTime.Now;
        }

        #endregion Public Constructors

        #region Public Properties

        public int ReturnValue { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void Debug(string msg)
        {
            WriteLineWithColor(ConsoleColor.Gray, "[DEBUG] " + msg);
        }

        public void DebugFormat(string format, params object[] args)
        {
            WriteLineWithColor(ConsoleColor.Gray, "[DEBUG] " + string.Format(format, args));
        }

        public void EndProgress()
        {
        }

        public void Error(string msg)
        {
            WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + msg);
        }

        public void ErrorException(string msg, Exception ex)
        {
            WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + msg);
            WriteLineWithColor(ConsoleColor.Red, "Exception: " + ex);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + string.Format(format, args));
        }

        public void Finish(bool successful)
        {
            var now = DateTime.Now;
            var timeString = string.Format(
                "at {0}, {1}:{2:d2} elapsed.",
                now.ToShortTimeString(),
                (int)now.Subtract(begin).TotalMinutes,
                now.Subtract(begin).Seconds);
            if (successful)
            {
                Console.Title = "ConfuserEx - Success";
                WriteLineWithColor(ConsoleColor.Green, "Finished " + timeString);
                ReturnValue = 0;
            }
            else
            {
                Console.Title = "ConfuserEx - Fail";
                WriteLineWithColor(ConsoleColor.Red, "Failed " + timeString);
                ReturnValue = 1;
            }
        }

        public void Info(string msg)
        {
            WriteLineWithColor(ConsoleColor.White, " [INFO] " + msg);
        }

        public void InfoFormat(string format, params object[] args)
        {
            WriteLineWithColor(ConsoleColor.White, " [INFO] " + string.Format(format, args));
        }

        public void Progress(int progress, int overall)
        {
        }

        public void Warn(string msg)
        {
            WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + msg);
        }

        public void WarnException(string msg, Exception ex)
        {
            WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + msg);
            WriteLineWithColor(ConsoleColor.Yellow, "Exception: " + ex);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + string.Format(format, args));
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
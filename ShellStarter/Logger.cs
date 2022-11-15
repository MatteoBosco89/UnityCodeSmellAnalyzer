using System;
using System.IO;
using System.Text;

namespace Starter
{
    /// <summary>
    /// Logger Class
    /// </summary>
    public static class Logger
    {
        private static string logFile = "ShellStarter.log";
        private static LogLevel logLevel;
        private static bool verbose = false;
        /// <summary>
        /// Make logger ThreadSafe. Once a Thread invoke Log operation
        /// it has to acquire this lock or wait for it.
        /// </summary>
        private static readonly object syncLock = new object();
        /// <summary>
        /// LogLevel <see cref="Enum"/> provides Trace, Debug, Information, Warning, Error, Critical and None
        /// </summary>
        public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical, None };
        public static string LogFile { get { return logFile; } set { logFile = value; } }
        public static LogLevel Level { get { return logLevel; } set { logLevel = value; } }
        public static bool Verbose { set { verbose = value; } }

        /// <summary>
        /// Initialize the Logger
        /// </summary>
        public static void Start()
        {
            if (logLevel == LogLevel.None) return;
            File.WriteAllText(logFile, "");
        }
        /// <summary>
        /// Updates the LogLevel
        /// </summary>
        /// <param name="level">int representing the Log Level</param>
        public static void SetLogLevel(int level)
        {
            if ((LogLevel)level >= LogLevel.None) logLevel = LogLevel.None;
            else logLevel = (LogLevel)level;
        }
        /// <summary>
        /// Logs the text provided with the level <see cref="LogLevel"/> constraint.
        /// Thread Safe.
        /// </summary>
        /// <param name="level">LogLevel Constraint</param>
        /// <param name="text">Text to Log</param>
        public static void Log(LogLevel level, string text)
        {
            if (logLevel == LogLevel.None) return;
            if (level < logLevel) return;
            lock (syncLock)
            {
                File.AppendAllText(logFile, DateTime.Now + " " + text + Environment.NewLine);
                if (verbose) Console.WriteLine(text);
            }
        }

    }
}




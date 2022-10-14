using System;
using System.IO;

namespace MetaSmellDetector
{
    public static class Logger
    {
        private static string logFile = "Analyzer.log";
        private static LogLevel logLevel;
        private static StreamWriter sw;
        private static bool verbose = false;
        public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical, None };
        public static string LogFile { get { return logFile; } set { logFile = value; } }
        public static LogLevel Level { get { return logLevel; } set { logLevel = value; } }
        public static bool Verbose { get { return verbose; } set { verbose = value; } }

        public static void Start()
        {
            if (logLevel == LogLevel.None) return;
            if (!File.Exists(logFile)) sw = new StreamWriter(File.Create(logFile));
            else sw = new StreamWriter(logFile);
        }

        public static void SetLogLevel(int level)
        {
            if ((LogLevel)level >= LogLevel.None) logLevel = LogLevel.None;
            else logLevel = (LogLevel)level;
        }

        public static void Log(LogLevel level, string text)
        {
            if (logLevel == LogLevel.None) return;
            if (sw == null) return;
            if (level >= logLevel)
            {
                sw.WriteLine(DateTime.Now + " " + text);
                sw.Flush();
            }
            if (verbose)
            {
                Console.WriteLine(text);
            }
        }
    }
}


using CommandLine;
using System;
using System.Text.RegularExpressions;

namespace CodeSmellFinder
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => SmellAnalyzer.Init(o));
            if (SmellAnalyzer.DataPath != null || SmellAnalyzer.Expose)
            {
                SmellAnalyzer.Analyze();
                Environment.Exit(0);
            }
        }
    }
    public class Options
    {
        [Option('e', "expose", SetName = "exp", Required = true, HelpText = "Exposes all possible smell names, Mutually exclusive with -d, --data")]
        public bool Expose { get; set; }
        [Option('d', "data", SetName = "dat", Required = true, HelpText = "json file name produced by the Code Analyzer, Mutually exclusive with -e, --expose")]
        public string DataPath { get; set; }
        [Option('s', "smell", Required = false, HelpText = "Searches for a single smell")]
        public string Smell { get; set; }
        [Option('f', "file", Required = false, HelpText = "Textual file with the list of smells to search for")]
        public string SmellPath { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Enables the status log on the console window")]
        public bool Verbose { get; set; }
        [Option('p', "project", Required = false, HelpText = "Saves the number of smells for the project in a .csv file")]
        public bool NumSmellForProject { get; set; }
        [Option('c', "category", Required = false, HelpText = "Saves the smells by category")]
        public bool SmellForFile { get; set; }
        [Option('r', "result", Required = false, HelpText = "Save results into a specified folder")]
        public string SaveDirectory { get; set; }
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)")]
        public int Logging { get; set; }
    }
}

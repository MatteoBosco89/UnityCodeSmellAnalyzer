using CommandLine;
using System;
using System.Collections.Generic;

namespace UnityDataAnalyzer
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args).WithParsed(o => UnityDataExtractor.Init(o));
            UnityDataExtractor.Analyze();
            Environment.Exit(0);
            
        }
    }
    public class Options
    {
        [Option('d', "dir",Required = true, HelpText = "Path to the Assets directory")]
        public string DataPath { get; set; }
        [Option('m', "nometa", Required = false, HelpText = "If specified, the tool does no load .meta files")]
        public bool NoMeta { get; set; }
        [Option('f', "fileExt", Required = false, HelpText = "File (default Extension.txt) containing the extensions to analyze")]
        public string ExtensionFile { get; set; }
        [Option('e', "ext", Required = false, HelpText = "List of extensions to search")]
        public IEnumerable<string> Extensions { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Enable the status log on the console window")]
        public bool Verbose { get; set; }
        [Option('r', "results", Required = false, HelpText = "Saves results to specified folder (default is the current directory)")]
        public string SaveDirectory { get; set; }
        [Option('n', "name", Required = false, HelpText = "Specify the project Name")]
        public string ProjectName { get; set; } 
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)")]
        public int Logging { get; set; }
    }
}

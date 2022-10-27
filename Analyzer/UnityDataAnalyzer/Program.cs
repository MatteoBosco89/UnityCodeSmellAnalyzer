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
        [Option('a', "assets",Required = true, HelpText = "Path to Assets directory")]
        public string DataPath { get; set; }
        [Option('n', "nometa", Required = false, HelpText = "Don't load .meta files")]
        public bool NoMeta { get; set; }
        [Option('f', "fileExt", Required = false, HelpText = "Extension.txt file")]
        public string ExtensionFile { get; set; }
        [Option('e', "ext", Required = false, HelpText = "List of extension to search")]
        public IEnumerable<string> Extensions { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Enable the status log on console window")]
        public bool Verbose { get; set; }
        [Option('s', "save", Required = false, HelpText = "Save results to specified folder")]
        public string SaveDirectory { get; set; }
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)")]
        public int Logging { get; set; }
    }
}

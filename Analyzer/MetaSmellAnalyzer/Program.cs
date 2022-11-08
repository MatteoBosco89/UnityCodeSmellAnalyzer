using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using CommandLine;
namespace MetaSmellDetector
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => MetaDetector.Init(o));
            MetaDetector.Analyze();
            Environment.Exit(0);
        }
    }

    public class Options
    {
        [Option('e', "expose", SetName = "exp", Required = true, HelpText = "Expose all possible smell name, Mutually exclusive with -d, --data")]
        public bool Expose { get; set; }
        [Option('d', "data", SetName = "dat", Required = true, HelpText = "MainData directory and MetaData directory path, Mutually exclusive with -e, --expose")]
        public IEnumerable<string> DataPath { get; set; }
        [Option('f', "file", Required = false, HelpText = "file.txt with list of smell to search")]
        public string SmellPath { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Enable the status log on console window")]
        public bool Verbose { get; set; }
        [Option('r', "results", Required = false, HelpText = "Save results to specified directory")]
        public string SaveDir { get; set; }
        [Option('c', "category", Required = false, HelpText = "Save results divided by smell category")]
        public bool SaveOnMultipleFile { get; set; }
        [Option('p', "project", Required = false, HelpText = "Save results to file csv for the project")]
        public bool SaveResultCsv { get; set; }
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)")]
        public int Logging { get; set; }

    }

}



      




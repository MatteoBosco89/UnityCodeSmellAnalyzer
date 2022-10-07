using CommandLine;
using System.Text.RegularExpressions;

namespace CodeSmellFinder
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => SmellAnalyzer.Init(o));
            if (SmellAnalyzer.DataPath == null && !SmellAnalyzer.Expose) return;
            SmellAnalyzer.Analyze();
        }
    }
    public class Options
    {
        [Option('e', "expose", SetName = "exp", Required = true, HelpText = "Expose all possible smell name, Mutually exclusive with -d, --data")]
        public bool Expose { get; set; }
        [Option('d', "data", SetName = "dat", Required = true, HelpText = "Dataset.json file, Mutually exclusive with -e, --expose")]
        public string DataPath { get; set; }
        [Option('s', "smell", Required = false, HelpText = "Search single smell")]
        public string Smell { get; set; }
        [Option('f', "file", Required = false, HelpText = "file.txt with list of smell to search")]
        public string SmellPath { get; set; }
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default)")]
        public int Logging { get; set; }
    }
}

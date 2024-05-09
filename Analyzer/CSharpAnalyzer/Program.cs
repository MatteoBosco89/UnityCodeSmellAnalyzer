using CommandLine;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Main
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => AnalyzerConfiguration.Init(o));
            if (AnalyzerConfiguration.ProjectPath == null) { Logger.Log(Logger.LogLevel.Critical, "Project cannot be reached"); Environment.Exit(1); }
            ProjectSchema projectSchema = new ProjectSchema();
            projectSchema.Analyze();
            Environment.Exit(0);
        }
    }
    /// <summary>
    /// Command Line Options Wrapper
    /// </summary>
    public class Options
    {
        [Option('p', "project", Required = true, HelpText = "Project directory.")]
        public string ProjectPath { get; set; }
        [Option('r', "results", Required = false, HelpText = "Directory where to store the results (CodeAnalysis.json file). If not provided, results are saved in the current directory.")]
        public string Results { get; set; }
        [Option('d', "directory", Required = false, HelpText = "Analyze the specified directory only. If not provided, the project directory is selected.")]
        public string Directory { get; set; }
        [Option('a', "assembly", Required = false, HelpText = "Additional assemblies directory (i.e., to analyze DLLs).")]
        public string AssemblyDir { get; set; }
        [Option('s', "statements", Required = false, HelpText = "Set output all statements.")]
        public bool Statements { get; set; }
        [Option('n', "name", Required = false, HelpText = "The project name.")]
        public string ProjectName { get; set; }
        [Option('c', "config", Required = false, HelpText = "Configuration File.")]
        public string ConfigFile { get; set; }
        [Option('l', "log", Required = false, Default = 1, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default).")]
        public int Logging { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Displays the log on the standard output.")]
        public bool Verbose { get; set; }
    }


}

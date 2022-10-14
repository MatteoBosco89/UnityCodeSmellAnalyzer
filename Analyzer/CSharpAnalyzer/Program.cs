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
            if (AnalyzerConfiguration.ProjectPath == null) Environment.Exit(1);
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
        [Option('p', "project", Required = true, HelpText = "Project Directory.")]
        public string ProjectPath { get; set; }
        [Option('d', "directory", Required = false, HelpText = "Analyze only the directory provided. If not provided the Project Directory is selected.")]
        public string Directory { get; set; }
        [Option('a', "assembly", Required = false, HelpText = "Additional Assemblies Directory.")]
        public string AssemblyDir { get; set; }
        [Option('s', "statements", Required = false, HelpText = "Set output all Statements.")]
        public bool Statements { get; set; }
        [Option('n', "name", Required = false, HelpText = "The project name.")]
        public string ProjectName { get; set; }
        [Option('c', "config", Required = false, HelpText = "Configuration File.")]
        public string ConfigFile { get; set; }
        [Option('l', "log", Required = false, HelpText = "Log Level: Trace 0 Debug 1 Information 2 Warning 3 Error 4 Critical 5 None 6 (Debug is Default).")]
        public int Logging { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Display Log on the standard output.")]
        public bool Verbose { get; set; }
    }


}

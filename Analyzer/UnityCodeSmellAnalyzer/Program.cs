using CommandLine;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Main
    /// </summary>
    internal class Program
    {  
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => AnalyzerConfiguration.Init(o));
            if (AnalyzerConfiguration.ProjectPath == null) return;
            ProjectSchema projectSchema = new ProjectSchema();
            projectSchema.Analyze();
        }
    }
    /// <summary>
    /// Command Line Options Wrapper
    /// </summary>
    public class Options
    {
        [Option('p', "project", Required = true, HelpText = "Project Directory.")]
        public string ProjectPath { get; set; }
        [Option('a', "assembly", Required = false, HelpText = "Additional Assemblies Directory.")]
        public string AssemblyDir { get; set; }
        [Option('s', "statements", Required = false, HelpText = "Set output all Statements.")]
        public bool Statements { get; set; }
        [Option('n', "name", Required = false, HelpText = "The project name.")]
        public string ProjectName { get; set; }
        [Option('c', "config", Required = false, HelpText = "Configuration File.")]
        public string ConfigFile { get; set; }
    }
}

using System.Threading;
using CommandLine;
using StarterModel;

namespace Starter
{

    /// <summary>
    /// Main
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            UnityCodeSmellAnalyzer ucsa = new UnityCodeSmellAnalyzer();
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => ucsa.Init(o));
        }
    }


    /// <summary>
    /// Starter
    /// </summary>
    public class UnityCodeSmellAnalyzer
    {

        protected List<string> repos = new List<string>();
        protected ThreadHandler codeAnalysis;
        protected ThreadHandler dataAnalysis;
        List<string> codeAnalysisProc = new List<string> { "CSharpAnalyzer/CSharpAnalyzer.exe", "CodeSmellAnalysis/CodeSmellAnalysis.exe" };
        List<string> dataAnalysisProc = new List<string> { "UnityDataNalyzer/UnityDataNalyzer.exe", "MetaSmellAnalyzer/MetaSmellAnalyzer.exe" };


        public UnityCodeSmellAnalyzer()
        {
            Logger.SetLogLevel(1);
            Logger.Start();
        }

        public void Init(Options o)
        {
            string[] directories = Directory.GetDirectories(o.Directory);
            foreach (string d in directories) repos.Add(d);
            Logger.Log(Logger.LogLevel.Debug, "Found " + repos.Count + " Directories");
            foreach (string d in repos)
            {
                StartThread(d);
                //while(!codeAnalysis.Finished || !dataAnalysis.Finished) { }
                
            }
        }

        public void WriteOutput(string s)
        {
            Logger.Log(Logger.LogLevel.Debug, s);
        }

        public void StartThread(string repo)
        {
            //codeAnalysis = new ThreadHandler(codeAnalysisProc, CodeAnalysisCommands, this);
            //dataAnalysis = new ThreadHandler(dataAnalysisProc, dataAnalysisComm, this);
        }

        protected string ProjectName(string path)
        {
            string[] rep = path.Split(Path.DirectorySeparatorChar);
            return rep.ElementAt(rep.Length - 1);
        }
 
        protected List<string> CodeAnalysisCommands(string repo)
        {
            List<string> commands = new List<string>();
            string path = Path.GetFullPath(repo);
            string name = ProjectName(repo);
            commands.Add("-n " + name + " -p " + path + " -r ../Results/CodeSmell -v");
            commands.Add("-d ../Results/CodeSmell/results.json -r ../Results/CodeSmell -c -v");
            return commands;
        }

        protected List<string> DataAnalysisCommands(string repo)
        {
            List<string> commands = new List<string>();

            return commands;
        }

        // -d path/to/result.json -v -c divisi per categoria -r path/to/resultFolder

    }

    /// <summary>
    /// Command Line Options Wrapper
    /// </summary>
    public class Options
    {
        [Option('d', "directory", Required = true, HelpText = "Repos Directory")]
        public string Directory { get; set; }
    }

}
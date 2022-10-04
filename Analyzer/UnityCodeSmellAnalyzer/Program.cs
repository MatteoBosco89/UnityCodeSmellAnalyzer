using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace UnityCodeSmellAnalyzer
{
    internal class Program
    {

        protected static List<string> fileList = new List<string>();
        protected static List<SyntaxTreeWrapper> compilationUnits = new List<SyntaxTreeWrapper>();
        protected static List<MetadataReference> assemblies = new List<MetadataReference>();
        protected static List<CompilationUnit> project = new List<CompilationUnit>();
        protected static Dictionary<string, string> commands = new Dictionary<string, string>();
        protected static string jsonString;


        protected static void ShowHelp()
        {
            Console.WriteLine("C# Code Analyzer based on Roslyn API for Unity3d");
            Console.WriteLine(string.Format("{0,-30} {1,-30}", "OPTION", "FUNCTION"));
            foreach (KeyValuePair<string, string> kv in commands) Console.WriteLine(string.Format("{0,-30} {1,-30}", kv.Key, kv.Value));
        }
        

        public static void Main()
        {
            Console.WriteLine();
            LoadCommands();
            List<string> args = Environment.GetCommandLineArgs().ToList();

            if(args.Contains("-help"))
            {
                ShowHelp();
                return;
            }
            if (!args.Contains("-project"))
            {
                Console.WriteLine("-project command is mandatory\n");
                ShowHelp();
                return;
            }
            if (args.Contains("-project"))
            {
                int index = args.IndexOf("-project");
                AnalyzerConfiguration.ProjectPath = args.ElementAt(index + 1);
            }
            if (args.Contains("-statements"))
            {
                AnalyzerConfiguration.StatementVerbose = true;
            }

            AnalyzerConfiguration.Init();

            LoadAssemblyList();
            LoadFileList();
            Analyze();
        }

        protected static void LoadAssemblyList()
        {
            Console.Write("Loading Assemblies...");
            assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            try
            {
                foreach(string file in AnalyzerConfiguration.Assemblies) assemblies.Add(MetadataReference.CreateFromFile(file));
                Console.WriteLine("Loaded!");
            }
            catch (Exception)
            {
                Console.WriteLine("Directory Not Found!");
            }
        }

        protected static void LoadFileList()
        {
            Console.Write("Loading Project...");
            try
            {
                string[] dir = { ".cs" };
                List<string> files = Directory.GetFiles(AnalyzerConfiguration.ProjectPath, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    fileList.Add(f);
                }
                Console.WriteLine("Loaded!");
            }
            catch (Exception)
            {
                Console.WriteLine("Directory not found!");
            }
        }

        protected static void Analyze()
        {
            if (fileList.Count <= 0) return;
            Console.WriteLine("Analysis Started");
            foreach (string file in fileList)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                tree = tree.WithFilePath(file);
                compilationUnits.Add(new SyntaxTreeWrapper(tree, file));
            }

            CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees: GetCU(), references: assemblies);

            
            AnalyzerConfiguration.Compilation = compilation;

            foreach (SyntaxTreeWrapper syntaxTree in compilationUnits)
            {
                SyntaxTree tree = syntaxTree.tree;
                string fileName = syntaxTree.fileName;
                string name = Path.GetFileNameWithoutExtension(fileName);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                SemanticModel model = compilation.GetSemanticModel(tree);
                CompilationUnit cu = new CompilationUnit(name, fileName);
                Console.WriteLine("Analyzing " + cu.Name);
                cu.LoadInformations(root, model);
                project.Add(cu);
            }

            ToJson();
            ToFile(jsonString);
        }

        protected static IEnumerable<SyntaxTree> GetCU()
        {
            List<SyntaxTree> s = new List<SyntaxTree>();
            foreach (SyntaxTreeWrapper stw in compilationUnits) s.Add(stw.tree);
            return s;
        }

        protected static void ToJson()
        {
            jsonString = JsonConvert.SerializeObject(project, Formatting.Indented);
        }

        internal class SyntaxTreeWrapper
        {
            public SyntaxTree tree;
            public string fileName;
            public SyntaxTreeWrapper(SyntaxTree tree, string fileName)
            {
                this.tree = tree;
                this.fileName = fileName;   
            }
        }

        protected static void ToFile(string toWrite)
        {
            Console.Write("Saving Results...");
            File.WriteAllText("results.json", toWrite);
            Console.Write("Done!\n");
        }

        protected static void LoadCommands()
        {            
            commands.Add("-project", "Project directory (use \"path/to/directory\" if there's spaces in the path)");
            commands.Add("-statements", "Get all statements");
            commands.Add("-help", "Shows this message");
        }

    }
}

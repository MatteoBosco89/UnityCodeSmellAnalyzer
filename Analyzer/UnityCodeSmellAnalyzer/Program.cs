using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CommandLine;



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
        

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => AnalyzerConfiguration.Init(o));

            if (AnalyzerConfiguration.ProjectPath == null) return;

            LoadAssemblyList();
            LoadFileList();
            Analyze();
        }

        protected static void LoadAssemblyList()
        {
            Console.WriteLine();
            Console.WriteLine("Loading Assemblies...");
            assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            try
            {
                foreach (string file in AnalyzerConfiguration.Assemblies) assemblies.Add(MetadataReference.CreateFromFile(file));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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


    }

    public class Options
    {

        [Option('p', "project", Required = true, HelpText = "Project Directory.")]
        public string Project { get; set; }
        [Option('a', "assembly", Required = false, HelpText = "Additional Assemblies Directory.")]
        public string AssemblyDir { get; set; }
        [Option('s', "statements", Required = false, HelpText = "Set output all Statements.")]
        public bool Statements { get; set; }
        
    }

}

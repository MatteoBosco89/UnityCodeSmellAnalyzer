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

namespace UnityCodeSmellAnalyzer
{
    internal class Program
    {

        protected static List<string> fileList = new List<string>();
        protected static List<SyntaxTreeWrapper> compilationUnits = new List<SyntaxTreeWrapper>();
        protected static List<MetadataReference> assemblies = new List<MetadataReference>();
        protected static List<CompilationUnit> project = new List<CompilationUnit>();
        protected static string jsonString;

        public static void Main(string[] args)
        {

            // mandatory -d option
            if (args.Length <= 0)
            {
                Console.WriteLine("Project directory path is mandatory, -d [directory/path]");
                return;
            }

            string command = args[0];
            string directory = "";

            if (command.Equals("-h")) { Console.WriteLine("-d [directory/path]\n-h [shows this message]"); return; }

            if (command.Equals("-d")) { directory = args[1]; }

            LoadAssemblyList();
            LoadFileList(directory);
            Analyze();
        }

        protected static void LoadAssemblyList()
        {
            Console.Write("Loading Assemblies...");
            assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            try
            {
                string[] lines = File.ReadAllLines("Assemblies.conf");
                string[] dir = { ".dll" };
                foreach (string line in lines)
                {
                    List<string> files = Directory.GetFiles(line, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                    foreach (string f in files)
                    {
                        assemblies.Add(MetadataReference.CreateFromFile(f));
                    }
                    
                }
                Console.WriteLine("Loaded!");
            }
            catch (Exception)
            {
                Console.WriteLine("Directory Not Found!");
            }
        }

        protected static void LoadFileList(string directory)
        {
            Console.Write("Loading Project...");
            try
            {
                string[] dir = { ".cs" };
                List<string> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
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
            Console.WriteLine("Analyzing...");
            foreach (string file in fileList)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                string fileName = Path.GetFileNameWithoutExtension(file);
                compilationUnits.Add(new SyntaxTreeWrapper(tree, fileName));
            }

            CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees: GetCU(), references: assemblies );
            


            foreach (SyntaxTreeWrapper syntaxTree in compilationUnits)
            {
                SyntaxTree tree = syntaxTree.tree;
                string fileName = syntaxTree.fileName;
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                SemanticModel model = compilation.GetSemanticModel(tree);
                CompilationUnit cu = new CompilationUnit(fileName);
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
}

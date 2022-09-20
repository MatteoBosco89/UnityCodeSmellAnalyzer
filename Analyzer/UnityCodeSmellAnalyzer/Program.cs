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
using System.Text;
using System.Threading.Tasks;

namespace UnityCodeSmellAnalyzer
{
    internal class Program
    {

        protected static List<string> fileList = new List<string>();
        protected static List<SyntaxTree> compilationUnits = new List<SyntaxTree>();
        static readonly MetadataReference Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        public static void Main(string[] args)
        {
            // mandatory -d option
            if (args.Length <= 0)
            {
                Console.WriteLine("Directory path is mandatory, -d [directory/path]");
                return;
            }

            string command = args[0];
            string directory = "";

            if (command.Equals("-d")) { directory = args[1]; }

            LoadFileList(directory);
            Analyze();
        }


        protected static void LoadFileList(string directory)
        {
            try
            {
                string[] dir = { ".cs" };
                List<string> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    fileList.Add(f);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Directory not found!");
            }
        }

        protected static void Analyze()
        {
            foreach (string file in fileList)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                compilationUnits.Add(tree);
            }

            foreach(SyntaxTree cu in compilationUnits)
            {
                CompilationUnitSyntax root = cu.GetCompilationUnitRoot();
                CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees: GetCU(), references: new[] { Mscorlib });
                SemanticModel model = compilation.GetSemanticModel(cu);
            }
        }

        protected static IEnumerable<SyntaxTree> GetCU()
        {
            return compilationUnits;
        }

    }
}

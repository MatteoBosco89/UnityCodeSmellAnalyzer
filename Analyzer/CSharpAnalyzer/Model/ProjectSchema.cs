using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class Representing the entire project analyzed. Contains trivial informations and the list of Compilation Units.
    /// Informations gathered: Project Name, Project Directory, Project Language, List of Compilation Units. 
    /// Responsible of the creation of the Semantic Model from the Assmeblies Linked
    /// </summary>
    [Serializable]
    public class ProjectSchema
    {
        protected List<CompilationUnit> project = new List<CompilationUnit>();
        protected List<string> fileList = new List<string>();
        protected List<SyntaxTreeWrapper> compilationUnits = new List<SyntaxTreeWrapper>();
        protected static List<MetadataReference> assemblies = new List<MetadataReference>();
        protected string jsonString;
        protected string projectName;
        protected string projectLanguage;
        protected string projectDir;
        protected string projectLanguageVersion;
        protected string configurationFile;
        protected string resultsFile = "results.json";
        protected int projectLoc = 0;

        public string ProjectName { get { return projectName; } }
        public string ProjectDirectory { get { return projectDir; } }
        public int LOC { get { return projectLoc; } }
        public string ConfigurationFile { get { return configurationFile; } }
        public string ProjectLanguage { get { return projectLanguage; } }
        public string ProjectLanguageVersion { get { return projectLanguageVersion; } }
        public List<CompilationUnit> Project { get { return project; } }

        public ProjectSchema() { }


        /// <summary>
        /// Start the Analysis, create the CSharpCompilation in order to Assemble all the dll's needed. 
        /// Gather the Syntax Trees of all the Compilation Units of the project from the disk. 
        /// The only public method of the class. Returns if the List of Compilation Units is Empty.
        /// </summary>
        public void Analyze()
        {
            if (!Directory.Exists(AnalyzerConfiguration.ProjectPath)) { Console.WriteLine("Project not found"); return; }
            LoadAssemblyList();
            LoadFileList();
            if (fileList.Count <= 0) return;
            LoadSyntax();
            CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees: GetCU(), references: assemblies);
            AnalyzerConfiguration.Compilation = compilation;
            if (AnalyzerConfiguration.Compilation == null) { Console.WriteLine("Analysis Failed!"); return; }
            projectLanguage = AnalyzerConfiguration.Compilation.Language;
            projectDir = AnalyzerConfiguration.ProjectPath;
            projectName = AnalyzerConfiguration.ProjectName;
            configurationFile = AnalyzerConfiguration.ConfigFile;
            projectLanguageVersion = AnalyzerConfiguration.Compilation.LanguageVersion.ToString();


            foreach (SyntaxTreeWrapper syntaxTree in compilationUnits)
            {
                AnalyzeCompilation(syntaxTree);
            }
            ToJson();
            ToFile(jsonString, resultsFile);
        }

        /// <summary>
        /// Loads all the assemblies files
        /// </summary>
        protected void LoadAssemblyList()
        {
            Logger.Log(Logger.LogLevel.Debug, "Analysis Started...");
            assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            try
            {
                foreach (string file in AnalyzerConfiguration.Assemblies) assemblies.Add(MetadataReference.CreateFromFile(file));
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.Critical, "Errors occurred: " + e);
            }
        }

        /// <summary>
        /// Loads all the Syntax Trees of the Compilation Units of the Project from disk (.cs).
        /// </summary>
        protected void LoadFileList()
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading Project...");
            try
            {
                string[] dir = { ".cs" };
                List<string> files = Directory.GetFiles(AnalyzerConfiguration.ProjectPath, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    fileList.Add(f);
                }
                Logger.Log(Logger.LogLevel.Debug, "Loaded!");
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.Critical, "Errors occurred: " + e);
            }
        }

        /// <summary>
        /// Analyze the Single Compilation Unit
        /// </summary>
        /// <param name="syntaxTree">Object representing the Compilation Unit</param>
        protected void AnalyzeCompilation(SyntaxTreeWrapper syntaxTree)
        {
            SyntaxTree tree = syntaxTree.tree;
            string fileName = syntaxTree.fileName;
            string name = Path.GetFileNameWithoutExtension(fileName);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            SemanticModel model = AnalyzerConfiguration.Compilation.GetSemanticModel(tree);
            CompilationUnit cu = new CompilationUnit(name, fileName);
            Logger.Log(Logger.LogLevel.Debug, "Analyzing " + cu.Name);
            cu.LoadInformations(root, model);
            LoadLoc(root, model);
            project.Add(cu);
        }

        /// <summary>
        /// Calculate the LOC of entire project
        /// </summary>
        /// <param name="root">The Compilation Unit</param>
        /// <param name="model">The model</param>
        protected void LoadLoc(SyntaxNode root, SemanticModel model)
        {
            projectLoc += root.GetLocation().GetLineSpan().EndLinePosition.Line - root.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        /// <summary>
        /// Create the SyntaxTree object in order to get additional informations.
        /// </summary>
        protected void LoadSyntax()
        {
            foreach (string file in fileList)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                tree = tree.WithFilePath(file);
                compilationUnits.Add(new SyntaxTreeWrapper(tree, file));
            }
        }
        /// <summary>
        /// Returns the SyntaxTree List in IEnumerable format
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<SyntaxTree> GetCU()
        {
            List<SyntaxTree> s = new List<SyntaxTree>();
            foreach (SyntaxTreeWrapper stw in compilationUnits) s.Add(stw.tree);
            return s;
        }
        /// <summary>
        /// Create the Json string for the results.
        /// </summary>
        protected void ToJson()
        {
            jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        /// <summary>
        /// Save the Json string to file.
        /// </summary>
        /// <param name="toWrite">String To Write in results file</param>
        /// <param name="file">The file</param>
        protected void ToFile(string toWrite, string file)
        {
            Logger.Log(Logger.LogLevel.Debug, "Saving Results in " + file);
            File.WriteAllText(file, toWrite);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }


        /// <summary>
        /// Class representing the SyntaxTree plus additional informations needed for every Compilation Unit.
        /// </summary>
        protected class SyntaxTreeWrapper
        {
            public SyntaxTree tree;
            public string fileName;
            public SyntaxTreeWrapper(SyntaxTree tree, string fileName)
            {
                this.tree = tree;
                this.fileName = fileName;
            }
        }

    }
}


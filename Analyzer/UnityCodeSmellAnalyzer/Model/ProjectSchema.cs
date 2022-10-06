﻿using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using static UnityCodeSmellAnalyzer.Program;
using System.Collections.Generic;
using System.IO;
using UnityCodeSmellAnalyzer;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityCodeSmellAnalyzer
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

        public string ProjectName { get { return projectName; } }
        public string ProjectDirectory { get { return projectDir; } }
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
            LoadAssemblyList();
            LoadFileList();
            if (fileList.Count <= 0) return;
            Console.WriteLine("Analysis Started");
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
            Console.WriteLine();
            Console.WriteLine("Loading Assemblies...");
            assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            try
            {
                foreach (string file in AnalyzerConfiguration.Assemblies) assemblies.Add(MetadataReference.CreateFromFile(file));
            }
            catch (Exception e)
            {
                Console.WriteLine("Errors occurred: " + e);
            }
        }

        /// <summary>
        /// Loads all the Syntax Trees of the Compilation Units of the Project from disk (.cs).
        /// </summary>
        protected void LoadFileList()
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
            catch (Exception e)
            {
                Console.WriteLine("Errors occurred: " + e);
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
            Console.WriteLine("Analyzing " + cu.Name);
            cu.LoadInformations(root, model);
            project.Add(cu);
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
            Console.Write("Saving Results...");
            File.WriteAllText(file, toWrite);
            Console.Write("Done!\n");
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


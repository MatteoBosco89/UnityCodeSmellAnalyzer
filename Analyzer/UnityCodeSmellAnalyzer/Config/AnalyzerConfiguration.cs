using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Utility Class representing configurations of the Analyzer such as:
    /// Statements, Assemblies Path, Project Path, The CSharpCompilation, Miscellaneous
    /// </summary>
    public static class AnalyzerConfiguration
    {
        
        private static string projectPath = null;
        private static CSharpCompilation compilation;
        private static ConfigModel configurations;
        private static bool statementsVerbose = false;

        public static List<string> Assemblies { get { return configurations.Assemblies; } }
        
        public static bool StatementVerbose { get { return statementsVerbose; } set { statementsVerbose = value; } }
        public static CSharpCompilation Compilation
        {
            get { return compilation; }
            set { compilation = value; }
        }
        
        public static string ProjectPath
        {
            get { return projectPath; }
            set { projectPath = value; }
        }

        public static void Init(Options opt)
        {
            string f = File.ReadAllText("Config.json");
            configurations = JsonConvert.DeserializeObject<ConfigModel>(f);
            projectPath = opt.Project;
            configurations.ProjectPath = opt.Project;
            statementsVerbose = opt.Statements;
            configurations.AdditionalAssembly = opt.AssemblyDir;
        }

        

        [Serializable]
        private class ConfigModel
        {
            protected List<string> assemblyFiles = new List<string>();
            protected List<string> assemblyDirectories = new List<string>();
            protected string assemblyDirAdditional;
            protected string projectPath;
            protected List<string> assemblyList = new List<string>();

            [JsonIgnore]
            public List<string> Assemblies { get { return CreateAssemblies(); } }
            [JsonIgnore]
            public string ProjectPath { set { projectPath = value; } }
            [JsonIgnore]
            public string AdditionalAssembly { set { assemblyDirAdditional = value; } }
            public List<string> AssemblyFiles { get { return assemblyFiles; } set { assemblyFiles = value; } }
            public List<string> AssemblyDirectories { get { return assemblyDirectories; } set { assemblyDirectories = value; } }


            public ConfigModel() { }

            protected List<string> CreateAssemblies()
            {
                LoadProjectAssemblies();
                LoadConfigAssemblyDirectory();
                LoadConfigAssemblyFiles();
                LoadAdditionalAssemblyDirectory();

                return assemblyList;
            }
            
            protected void LoadProjectAssemblies()
            {
                string[] dir = { ".dll" };
                Console.Write("Loading Project Assemblies...");
                List<string> projectAssemblies = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith))?.ToList();
                if (projectAssemblies.Count > 0)
                {
                    assemblyList.AddRange(projectAssemblies);
                    Console.WriteLine("Done");
                }else Console.WriteLine("Nothing Found");
            }
            protected void LoadConfigAssemblyFiles()
            {
                if (assemblyFiles.Count <= 0) return;
                Console.Write("Loading Config Assemblies Files...");
                assemblyList.AddRange(assemblyFiles);
            }
            protected void LoadConfigAssemblyDirectory()
            {
                string[] dir = { ".dll" };
                Console.Write("Loading Config Assemblies Directories...");
                List<string> assemblyDir = new List<string>();
                foreach (var directory in assemblyDirectories)
                {
                    List<string> tempList = new List<string>();
                    tempList = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                    if(tempList.Any()) assemblyDir.AddRange(tempList);
                }
                if (assemblyDir.Count <= 0) Console.WriteLine("Nothing Found");
                else { assemblyList.AddRange(assemblyDir); Console.WriteLine("Done"); }
            }
            protected void LoadAdditionalAssemblyDirectory()
            {
                if (assemblyDirAdditional == null) return;
                string[] dir = { ".dll" };
                Console.Write("Loading Additional Assemblies Directory...");
                List<string> projectAssemblies = Directory.GetFiles(assemblyDirAdditional, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                if (projectAssemblies.Count > 0)
                {
                    assemblyList.AddRange(projectAssemblies);
                    Console.WriteLine("Done");
                }
                else Console.WriteLine("Nothing Found");
            }
        }

    }
}


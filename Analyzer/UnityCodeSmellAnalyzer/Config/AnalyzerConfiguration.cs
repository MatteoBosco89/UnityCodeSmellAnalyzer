using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static void Init()
        {
            string f = File.ReadAllText("Config.json");
            configurations = JsonConvert.DeserializeObject<ConfigModel>(f);
        }

        

        [Serializable]
        private class ConfigModel
        {
            protected List<string> assemblyFiles = new List<string>();
            protected List<string> assemblyDirectories = new List<string>();

            [JsonIgnore]
            public List<string> Assemblies { get { return CreateAssemblies(); } }

            public List<string> AssemblyFiles { get { return assemblyFiles; } set { assemblyFiles = value; } }
            public List<string> AssemblyDirectories { get { return assemblyDirectories; } set { assemblyDirectories = value; } }

            public ConfigModel() { }

            protected List<string> CreateAssemblies()
            {
                List<string> files = new List<string>();
                string[] dir = { ".dll" };
                foreach (var directory in assemblyDirectories)
                {
                    files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                }
                files.AddRange(assemblyFiles);
                return files;
            }
            

        }

    }
}


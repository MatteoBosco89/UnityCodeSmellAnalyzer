using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CSharpAnalyzer
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
        private static string projectName = "C# Project";
        private static string configFile = "Config.json";
        private static string directoryPath;
        private static int logLevel = 1;
        private static bool verbose = false;

        public static List<string> Assemblies { get { return configurations.Assemblies; } }
        public static string DirectoryPath { get { return directoryPath; } }
        public static bool StatementVerbose { get { return statementsVerbose; } set { statementsVerbose = value; } }
        public static CSharpCompilation Compilation { get { return compilation; } set { compilation = value; } }
        public static string ProjectPath { get { return projectPath; } set { projectPath = value; } }
        public static string ProjectName { get { return projectName; } set { projectName = value; } }
        public static string ConfigFile { get { return configFile; } set { configFile = value; } }
        public static bool Verbose { get { return verbose; } }

        /// <summary>
        /// Init the Analyzer Configuration
        /// </summary>
        /// <param name="opt">Command Line Options</param>
        public static void Init(Options opt)
        {
            logLevel = opt.Logging;
            Logger.SetLogLevel(logLevel);
            Logger.Start();
            LoadCommands(opt);
            InitConfig(opt);
            
        }

        /// <summary>
        /// Initialize the ConfigModel object
        /// </summary>
        /// <param name="opt">Command Line Options</param>
        private static void InitConfig(Options opt)
        {
            try
            {
                string f = File.ReadAllText(configFile);
                Logger.Log(Logger.LogLevel.Critical, "Config File Found!");
                configurations = JsonConvert.DeserializeObject<ConfigModel>(f);
            }
            catch (Exception) 
            { 
                Logger.Log(Logger.LogLevel.Critical, "Config File Not Found!");
                configurations = new ConfigModel();
            }
            configurations.ProjectPath = opt.ProjectPath;
            configurations.AdditionalAssembly = opt.AssemblyDir;
            Logger.Log(Logger.LogLevel.Critical, "Additional Assemblies: " + (opt.AssemblyDir ?? "None"));
        }

        /// <summary>
        /// Loads all the commands passed
        /// </summary>
        /// <param name="opt">Command Line Options</param>
        private static void LoadCommands(Options opt)
        {
            if (opt.ProjectName != null) projectName = opt.ProjectName;
            Logger.Log(Logger.LogLevel.Critical, "Project: " + projectName);
            if (opt.Directory != null) directoryPath = opt.ProjectPath + opt.Directory;
            else directoryPath = opt.ProjectPath;
            Logger.Log(Logger.LogLevel.Critical, "Directory Analyzed: " + directoryPath);
            if (opt.ConfigFile != null) configFile = opt.ConfigFile;
            Logger.Log(Logger.LogLevel.Critical, "Config File: " + configFile);
            statementsVerbose = opt.Statements;
            Logger.Log(Logger.LogLevel.Critical, "Statements: " + statementsVerbose);
            verbose = opt.Verbose;
            Logger.Log(Logger.LogLevel.Critical, "Verbose: " + verbose);
            projectPath = opt.ProjectPath;
            Logger.Log(Logger.LogLevel.Critical, "Project Path: " + projectPath);
            Logger.Log(Logger.LogLevel.Critical, "Log Level: " + (Logger.LogLevel)logLevel);
        }

        /// <summary>
        /// Inner Class representing the Configuration Model. Responsible for the Configuration Loading and 
        /// Project Assembly Loading.
        /// </summary>
        [Serializable]
        internal class ConfigModel
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
                Logger.Log(Logger.LogLevel.Debug, "Loading Project Assemblies...");
                List<string> projectAssemblies = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith))?.ToList();
                if (projectAssemblies.Count > 0)
                {
                    assemblyList.AddRange(projectAssemblies);
                    Logger.Log(Logger.LogLevel.Debug, "Done!");
                }else Logger.Log(Logger.LogLevel.Error, "Nothing Found!");
            }
            protected void LoadConfigAssemblyFiles()
            {
                if (assemblyFiles.Count <= 0) return;
                Logger.Log(Logger.LogLevel.Debug, "Loading Config Assemblies Files...");
                assemblyList.AddRange(assemblyFiles);
            }
            protected void LoadConfigAssemblyDirectory()
            {
                string[] dir = { ".dll" };
                Logger.Log(Logger.LogLevel.Debug, "Loading Config Assemblies Directories...");
                List<string> assemblyDir = new List<string>();
                foreach (var directory in assemblyDirectories)
                {
                    List<string> tempList = new List<string>();
                    tempList = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                    if(tempList.Any()) assemblyDir.AddRange(tempList);
                }
                if (assemblyDir.Count <= 0) Logger.Log(Logger.LogLevel.Error, "Nothing Found!");
                else { assemblyList.AddRange(assemblyDir); Logger.Log(Logger.LogLevel.Debug, "Done!"); }
            }
            protected void LoadAdditionalAssemblyDirectory()
            {
                if (assemblyDirAdditional == null) return;
                string[] dir = { ".dll" };
                Logger.Log(Logger.LogLevel.Debug, "Loading Additional Assemblies Directory...");
                List<string> projectAssemblies = Directory.GetFiles(assemblyDirAdditional, "*.*", SearchOption.AllDirectories).Where(f => dir.Any(f.ToLower().EndsWith)).ToList();
                if (projectAssemblies.Count > 0)
                {
                    assemblyList.AddRange(projectAssemblies);
                    Logger.Log(Logger.LogLevel.Debug, "Done!");
                }
                else Logger.Log(Logger.LogLevel.Error, "Nothing Found!");
            }
        }

    }
}


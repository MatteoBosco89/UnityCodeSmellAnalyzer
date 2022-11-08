using CSharpAnalyzer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityAnalyzer;

namespace UnityDataAnalyzer
{
    /// <summary>
    /// The class representing the unity data extractor object
    /// </summary>
    public class UnityDataExtractor
    {
        protected static List<string> unityMainFileList = new List<string>();
        protected static List<string> unityMetaFileList = new List<string>();
        protected static string[] DIR = { ".unity", ".controller", ".prefab", ".mat", ".anim", ".flare", ".assets" };
        protected static string[] META = { ".meta" };
        protected static List<UnityData> mainData = new List<UnityData>();
        protected static List<UnityData> metaData = new List<UnityData>();
        protected static string RESULT_DIR = "UnityDataResults";
        protected static string META_DIR = "metaResults";
        protected static string MAIN_DIR = "mainResults";
        protected static string MAIN_FILE = "mainData.json";
        protected static string META_FILE = "metaData.json";
        protected static bool meta = true;
        protected static string save_dir = "";
        protected static string directory = null;
        protected static string file_extensions = null;
        protected static int logLevel = 1;
        protected static string projectName = "UnityProject";
        protected static Dictionary<string, int> numMainFiles = new Dictionary<string, int>();

        /// <summary>
        /// Load the configuration of the program from the given option from command line
        /// </summary>
        /// <param name="opt">The Options object containing the parameter given to the program</param>
        public static void Init(Options opt)
        {
            if (opt.DataPath != null)directory = opt.DataPath;
            if (opt.NoMeta) meta = false;
            if (opt.Extensions.Count() > 0) LoadExtensions(opt.Extensions);
            if (opt.ExtensionFile != null) file_extensions = opt.ExtensionFile;
            if (opt.SaveDirectory != null) save_dir = opt.SaveDirectory;
            CreateSaveDirectory();
            if (opt.ProjectName != null) projectName = opt.ProjectName;
            if (opt.Verbose) Logger.Verbose = true;
            Logger.SetLogLevel(logLevel);
            if (save_dir == "") save_dir = Directory.GetCurrentDirectory();
            Logger.LogFile = Path.Combine(save_dir, "UnityExtractor.Log");
            Logger.Start();
        }

        protected static void CreateSaveDirectory()
        {
            if (save_dir == "") return;
            Console.WriteLine(Path.DirectorySeparatorChar);
            if (save_dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                save_dir = save_dir.Remove(save_dir.Length - 1);
            }
            if (Directory.Exists(save_dir)) return;
            else Directory.CreateDirectory(save_dir);
        }
        /// <summary>
        /// Load the list of file inside the given project folder
        /// </summary>
        /// <returns>True if the operation is succesfull, false otherwise</returns>
        protected static bool LoadFileList()
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching MetaData file inside project folder...");
            try
            {
                Logger.Log(Logger.LogLevel.Debug, "Searching main file...");
                List<string> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => DIR.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    unityMainFileList.Add(f);
                }
                Logger.Log(Logger.LogLevel.Debug, "Done!");
                Logger.Log(Logger.LogLevel.Debug, "Searching meta file...");
                files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => META.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    unityMetaFileList.Add(f);
                }
                Logger.Log(Logger.LogLevel.Debug, "Done!");
            }
            catch (Exception)
            {
                Logger.Log(Logger.LogLevel.Debug, "Directory not found!");
                return false;
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            return true;
        }
        /// <summary>
        /// Start the analisys process
        /// </summary>
        public static void Analyze()
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading extensions...");
            if (!LoadFileList()) return;
            if (file_extensions != null)
            {
                if (!LoadExtensionsFile()) return;
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            Logger.Log(Logger.LogLevel.Debug, "Creating Result Directories");
            CreateResultDirectory(MAIN_DIR);
            if (meta) CreateResultDirectory(META_DIR);
            Logger.Log(Logger.LogLevel.Debug, "Start analisys...");
            //load all prefab/unity/controller/mat files
            //load all metafile
            Logger.Log(Logger.LogLevel.Debug, "Analyzing Main MetaData...");
            UnityData pi = new UnityData("ProjectInfo", "InfoFile", "info");
            SaveSingleFile(pi, MAIN_DIR);
            foreach (string file in unityMainFileList)
            {
                if (unityMetaFileList.Contains(file + ".meta"))
                {
                    Logger.Log(Logger.LogLevel.Debug, $"Analyzing File: {file}");
                    UnityData d = new UnityData(file, file + ".meta");
                    Logger.Log(Logger.LogLevel.Debug, $"Data Extracted!");
                    unityMetaFileList.Remove(file + ".meta");
                    SaveSingleFile(d, MAIN_DIR);
                    //mainData.Add(d);
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            if (meta)
            {
                Logger.Log(Logger.LogLevel.Debug, "Analyzing Meta files...");
                foreach (string file in unityMetaFileList)
                {
                    Logger.Log(Logger.LogLevel.Debug, $"Analyzing File: {file}");
                    UnityData d = new UnityData(file);
                    Logger.Log(Logger.LogLevel.Debug, $"Data Extracted!");
                    SaveSingleFile(d, META_DIR);
                    //metaData.Add(d);
                }
                Logger.Log(Logger.LogLevel.Debug, $"Done!!");
            }
            SaveNumFileAnalyzed();
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }
        /// <summary>
        /// Save the results inside a file
        /// </summary>
        public static void SaveResults()
        {
            Logger.Log(Logger.LogLevel.Debug, "Saving Results...");
            string main_dir = Directory.GetCurrentDirectory();
            if (save_dir == "") main_dir = Path.Combine(main_dir, MAIN_DIR);
            else main_dir = Path.Combine(save_dir, MAIN_DIR);
            DirectoryInfo d = new DirectoryInfo(main_dir);
            if (d.Exists) d.Delete(true);
            Directory.CreateDirectory(main_dir);
            foreach (var data in mainData)
            {
                JObject res = data.ToJsonObject();
                res.Add("ProjectName", projectName);
                res.Add("ProjectPath", directory);
                string fileName = res["guid"].ToString() + ".json";
                Logger.Log(Logger.LogLevel.Debug, "Saving File: " + fileName);
                string s = Path.Combine(main_dir, fileName);
                File.WriteAllText(s, res.ToString());
            }
            if (meta)
            {
                string meta_dir = Directory.GetCurrentDirectory();
                if (save_dir == "") meta_dir = Path.Combine(meta_dir, META_DIR);
                else meta_dir = Path.Combine(save_dir, META_DIR);
                d = new DirectoryInfo(meta_dir);
                if (d.Exists) d.Delete(true);
                Directory.CreateDirectory(meta_dir);
                foreach (var data in metaData)
                {
                    JObject res = data.ToJsonObject();
                    res.Add("ProjectName", projectName);
                    res.Add("ProjectPath", directory);
                    string fileName = res["guid"].ToString() + ".json";
                    Logger.Log(Logger.LogLevel.Debug, "Saving File: " + fileName);
                    string s = Path.Combine(meta_dir, fileName);
                    File.WriteAllText(s, res.ToString());
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }

        protected static void SaveSingleFile(UnityData data, string dir)
        {
            Logger.Log(Logger.LogLevel.Debug, "Saving Results...");
            string main_dir = Directory.GetCurrentDirectory();
            if (save_dir == "") main_dir = Path.Combine(main_dir, dir);
            else main_dir = Path.Combine(save_dir, dir);
            JObject res = data.ToJsonObject();
            res.Add("ProjectName", projectName);
            res.Add("ProjectPath", directory);
            string fileName = res["guid"].ToString() + ".json";
            Logger.Log(Logger.LogLevel.Debug, "Saving File: " + fileName);
            string s = Path.Combine(main_dir, fileName);
            File.WriteAllText(s, res.ToString());
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
        }

        protected static void CreateResultDirectory(string dir)
        {
            string main_dir = Directory.GetCurrentDirectory();
            if (save_dir == "") main_dir = Path.Combine(main_dir, dir);
            else main_dir = Path.Combine(save_dir, dir);
            DirectoryInfo d = new DirectoryInfo(main_dir);
            if (d.Exists) d.Delete(true);
            Directory.CreateDirectory(main_dir);
        }
       
        /// <summary>
        /// Load the file containing the list of file extension to analyze
        /// </summary>
        /// <returns>True if the operation is successfull, False otherwise</returns>
        protected static bool LoadExtensionsFile()
        {
            try
            {
                string[] lines = File.ReadAllLines(file_extensions);
                List<string> extensions = new List<string>();
                foreach (string line in lines)
                {
                    string s = line;
                    if (s[0] != '.') s = '.' + s;
                    extensions.Add(s);
                }
                DIR = extensions.ToArray();
            }
            catch (FileNotFoundException)
            {
                Logger.Log(Logger.LogLevel.Debug, "Extension file not found, using default extensions");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Load the extension given by command line
        /// </summary>
        /// <param name="elements">The List of string containig the name of the extensions to load</param>
        protected static void LoadExtensions(IEnumerable<string> elements)
        {
            List<string> extensions = new List<string>();
            for (int i = 0; i < elements.Count(); i++)
            {
                string s = elements.ElementAt(i);
                if (s[0] != '.') s = '.' + s;
                extensions.Add(s);
            }
            DIR = extensions.ToArray();
            Console.WriteLine(DIR[0]);
        }

        protected static void NumOfAnalyzedFiles()
        {
            foreach(string ext in DIR)
            {
                int num = 0;
                foreach(string file in unityMainFileList)
                {
                    if (file.Contains(ext)) num++;
                }
                numMainFiles[ext] = num;
            }
        }

        protected static void SaveNumFileAnalyzed()
        {
            NumOfAnalyzedFiles();
            string main_dir = Directory.GetCurrentDirectory();
            if (save_dir != "") main_dir = save_dir;
            string headers = "";
            string values = "";
            int i = 0;
            headers += "ProjectName;ProjectPath;";
            values += projectName + ";" + directory + ";";
            foreach(KeyValuePair<string, int> k in numMainFiles)
            {
                headers += k.Key + ";";
                values += k.Value.ToString()+";";
            }
            headers += ".meta";
            values += unityMetaFileList.Count.ToString();

            string csv = headers + "\n" + values;
            File.WriteAllText(Path.Combine(main_dir, "numMetaFiles.csv"), csv);
        }
    }
}
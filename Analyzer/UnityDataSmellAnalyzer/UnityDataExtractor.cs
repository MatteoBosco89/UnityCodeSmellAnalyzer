using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityAnalyzer;

namespace UnityDataSmellAnalyzer
{
    public class UnityDataExtractor
    {
        protected static List<string> unityMainFileList = new List<string>();
        protected static List<string> unityMetaFileList = new List<string>();
        protected static string[] DIR = { ".unity", ".controller", ".prefab", ".mat", ".anim", ".flare", ".assets" };
        protected static string[] META = { ".meta" };
        protected static List<UnityData> mainData = new List<UnityData>();
        protected static List<UnityData> metaData = new List<UnityData>();
        protected static string RESULT_DIR = "UnityDataResults";
        protected static string MAIN_FILE = "mainData.json";
        protected static string META_FILE = "metaData.json";
        protected static bool meta = true;
        protected static string directory = null;
        protected static string file_extensions = null;
        protected static int logLevel = 1;

        public static void Init(Options opt)
        {
            if(opt.DataPath != null)directory = opt.DataPath;
            if(opt.NoMeta) meta = false;
            if(opt.Extensions.Count() > 0) LoadExtensions(opt.Extensions);
            if(opt.ExtensionFile != null) file_extensions = opt.ExtensionFile;
        }
        protected static bool LoadFileList()
        {
            Console.WriteLine("Loading Unity File...");
            try
            {
                Console.Write("\tSearching main file...");
                List<string> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => DIR.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    unityMainFileList.Add(f);
                }
                Console.WriteLine("Done!");
                Console.Write("\tLoading meta file...");
                files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => META.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    unityMetaFileList.Add(f);
                }
                Console.WriteLine("Done!");
            }
            catch (Exception)
            {
                Console.WriteLine("Directory not found!");
                return false;
            }
            Console.WriteLine("Done!");
            return true;
        }

        public static void Analyze()
        {
            if(!LoadFileList())return;
            if(file_extensions != null)
            {
                if (!LoadExtensionsFile()) return;
            }
            Console.WriteLine("Analyzing...");
            //load all prefab/unity/controller/mat files
            //load all metafile
            Console.Write("\tAnalyzig main files...");
            foreach (string file in unityMainFileList)
            {
                if (unityMetaFileList.Contains(file + ".meta"))
                {
                    //Console.WriteLine(file);
                    UnityData d = new UnityData(file, file + ".meta");
                    unityMetaFileList.Remove(file + ".meta");
                    mainData.Add(d);
                }
            }
            Console.WriteLine("Done!");
            if (meta)
            {
                Console.Write("\tAnalyzig meta files...");
                foreach (string file in unityMetaFileList)
                {
                    UnityData d = new UnityData(file);
                    metaData.Add(d);
                }
                Console.WriteLine("Done!");
            }
            Console.WriteLine("Done!");
            SaveResults();
        }

        public static void SaveResults()
        {
            Console.WriteLine("Saving Results...");
            JArray ja = new JArray();
            foreach (UnityData d in mainData) ja.Add(d.ToJsonObject());
            Directory.CreateDirectory(RESULT_DIR);
            File.WriteAllText(RESULT_DIR + "\\" + MAIN_FILE, ja.ToString());
            Console.WriteLine("\tMain Results saved in " + Path.GetFullPath(MAIN_FILE));
            ja = new JArray();
            if (meta)
            {
                foreach (UnityData d in metaData) ja.Add(d.ToJsonObject());
                Directory.CreateDirectory(RESULT_DIR);
                File.WriteAllText(RESULT_DIR + "\\" + META_FILE, ja.ToString());
                Console.WriteLine("\tMeta Results saved in " + Path.GetFullPath(META_FILE));
            }
            Console.WriteLine("Done\n");

        }

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
                Console.WriteLine("Extensions file not found, using default extensions");
                return false;
            }
            return true;
        }

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
    }
}
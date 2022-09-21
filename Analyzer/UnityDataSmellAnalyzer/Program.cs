using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAnalyzer;

namespace UnityDataSmellAnalyzer
{
    internal class Program
    {
        protected static List<string> unityMainFileList = new List<string>();
        protected static List<string> unityMetaFileList = new List<string>();
        protected static string[] DIR = {".unity", ".controller", ".prefab", ".mat" };
        protected static string[] META = { ".meta" };
        protected static List<UnityData> mainData = new List<UnityData>();
        protected static List<UnityData> metaData = new List<UnityData>();
        protected static string RESULT_DIR = "UnityDataResults";
        protected static string MAIN_FILE = "mainData.json";
        protected static string META_FILE = "metaData.json";
        static void Main(string[] args)
        {
            // mandatory -d option
            if (args.Length <= 0)
            {
                Console.WriteLine("Directory path is mandatory, -d [directory/path]");
                return;
            }

            string command = args[0];
            string directory = "";

            if (command.Equals("-h")) { Console.WriteLine("-d [directory/path]\n-h [shows this message]"); return; }

            if (command.Equals("-d")) { directory = args[1]; }

            LoadFileList(directory);
            Analyze();
            SaveResults();
        }
        protected static void LoadFileList(string directory)
        {
            try
            {
                Console.WriteLine("Searching main file");
                List<string> files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => DIR.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                { 
                    unityMainFileList.Add(f);
                }
                Console.WriteLine("Searching main file");
                files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => META.Any(f.ToLower().EndsWith)).ToList();
                foreach (string f in files)
                {
                    unityMetaFileList.Add(f);
                }
                    
            }
            catch (Exception)
            {
                Console.WriteLine("Directory not found!");
            }
        }

        protected static void Analyze()
        {
            //load all prefab/unity/controller/mat files
            //load all metafile
            foreach (string file in unityMainFileList)
            {
                if (unityMetaFileList.Contains(file + ".meta"))
                {
                    Console.WriteLine(file);
                    UnityData d = new UnityData(file, file + ".meta");
                    unityMetaFileList.Remove(file + ".meta");
                    mainData.Add(d);
                }
            }

            foreach(string file in unityMetaFileList)
            {
                UnityData d = new UnityData(file);
                metaData.Add(d);
            }
        }

        public static void SaveResults()
        {
            JArray ja = new JArray();
            foreach (UnityData d in mainData) ja.Add(d.ToJsonObject());
            Directory.CreateDirectory(RESULT_DIR);
            File.WriteAllText(RESULT_DIR + "\\" + MAIN_FILE, ja.ToString());
            ja = new JArray();
            foreach (UnityData d in metaData) ja.Add(d.ToJsonObject());
            Directory.CreateDirectory(RESULT_DIR);
            File.WriteAllText(RESULT_DIR + "\\" + META_FILE, ja.ToString());
        }
    }
}

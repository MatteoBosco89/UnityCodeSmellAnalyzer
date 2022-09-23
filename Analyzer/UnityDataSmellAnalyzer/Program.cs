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
        protected static string[] DIR = {".unity", ".controller", ".prefab", ".mat", ".anim", ".flare", ".assets" };
        protected static string[] META = { ".meta" };
        protected static List<UnityData> mainData = new List<UnityData>();
        protected static List<UnityData> metaData = new List<UnityData>();
        protected static string RESULT_DIR = "UnityDataResults";
        protected static string MAIN_FILE = "mainData.json";
        protected static string META_FILE = "metaData.json";
        protected static bool found_directory = false;
        static void Main(string[] args)
        {
            // mandatory -d option
            if (args.Length <= 0)
            {
                Console.WriteLine("Assets path is mandatory, -d [directory/path]");
                return;
            }

            string command = args[0];
            string directory = "";

            if (command.Equals("-h")) { Console.WriteLine("-d [directory/path]\n-h [shows this message]"); return; }

            if (command.Equals("-d")) { directory = args[1]; }
            PrintLogo();
            LoadFileList(directory);
            if (found_directory)
            {
                Analyze();
                SaveResults();
            }
        }
        protected static void LoadFileList(string directory)
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
                found_directory = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Directory not found!");
                found_directory = false;
            }
            Console.WriteLine("Done!");
        }

        protected static void Analyze()
        {
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
            Console.Write("\tAnalyzig meta files...");
            foreach (string file in unityMetaFileList)
            {
                UnityData d = new UnityData(file);
                metaData.Add(d);
            }
            Console.WriteLine("Done!");
            Console.WriteLine("Done!");
        }

        public static void SaveResults()
        {
            Console.Write("Saving Results...");
            JArray ja = new JArray();
            foreach (UnityData d in mainData) ja.Add(d.ToJsonObject());
            Directory.CreateDirectory(RESULT_DIR);
            File.WriteAllText(RESULT_DIR + "\\" + MAIN_FILE, ja.ToString());
            ja = new JArray();
            foreach (UnityData d in metaData) ja.Add(d.ToJsonObject());
            Directory.CreateDirectory(RESULT_DIR);
            File.WriteAllText(RESULT_DIR + "\\" + META_FILE, ja.ToString());
            Console.WriteLine("Done");
        }

        public static void PrintLogo()
        {
            string logo = " ____ ___      .__  __                                   \r\n" +
                    "|    |   \\____ |__|/  |_ ___.__.                         \r\n" +
                    "|    |   /    \\|  \\   __<   |  |                         \r\n" +
                    "|    |  /   |  \\  ||  |  \\___  |                         \r\n" +
                    "|______/|___|  /__||__|  / ____|                         \r\n" +
                    "             \\/          \\/                              \r\n" +
                    "                   .____    .__        __                \r\n" +
                    "                   |    |   |__| _____/  |_  ___________ \r\n" +
                    "                   |    |   |  |/    \\   __\\/ __ \\_  __ \\\r\n" +
                    "                   |    |___|  |   |  \\  | \\  ___/|  | \\/\r\n" +
                    "                   |_______ \\__|___|  /__|  \\___  >__|   \r\n" +
                    "                           \\/       \\/          \\/       ";
            Console.WriteLine(logo);
            Console.WriteLine("\nBy Matteo Bosco, Pasquale Cavoto, Augusto Ungolo\n");
        }
    }
}

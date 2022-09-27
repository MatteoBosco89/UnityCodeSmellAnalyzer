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
        protected static Dictionary<string, string> commands = new Dictionary<string, string>();
        protected static bool meta = true;
        static void Main()
        {
            PrintLogo();
            LoadCommands();
            string directory = "";
            List<string> args = Environment.GetCommandLineArgs().ToList();
            // mandatory -d option
            if (!args.Contains("-dir"))
            {
                Console.WriteLine("-dir is mandatory");
                ShowHelp();
                return;
            }

            if (args.Contains("-help"))
            {
                ShowHelp();
            }
            if (args.Contains("-dir"))
            {
                int i = args.IndexOf("-dir");
                directory = args[i + 1];
            }
            if (args.Contains("-ext_f"))
            {
                int i = args.IndexOf("-ext_f");
                string file_extensions = args[i + 1];
                LoadExtensionsFile(file_extensions);
            }
            if (args.Contains("-ext"))
            {
                int i = args.IndexOf("-ext");
                LoadExtensions(args[i + 1]);
            }
            if (args.Contains("-no_meta"))
            {
                meta = false;
            }

            LoadFileList(directory);
            if (found_directory)
            {
                Analyze();
                SaveResults();
            }
            else Console.WriteLine("Error directory " + directory + "Not found");
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
        protected static void LoadCommands()
        {
            commands.Add("-dir", "Assets directory (use \"path/to/directory\" if there's spaces in the path)");
            commands.Add("-ext_f", "Extensions list (use \"path/to/directory\" if there's spaces in the path)");
            commands.Add("-ext", "list of file to read (use \"<extension1>-<extension2>-....\" to insert the extensions to read");
            commands.Add("-no_meta", "doesn't analyze .meta file");
            commands.Add("-help", "Shows this message");
        }

        protected static void LoadExtensionsFile(string file_extensions)
        {
            try
            {
                string[] lines = File.ReadAllLines(file_extensions);
                List<string> extensions = new List<string>();
                foreach(string line in lines)
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
            }
        }

        protected static void LoadExtensions(string line)
        {
            string[] exts = line.Split('-');
            List<string> extensions = new List<string>();
            if(exts.Length <= 0)
            {
                Console.WriteLine("No extensions passed, using default extensions");
                return;
            }
            for(int i = 0; i < exts.Length; i ++)
            {
                string s = exts[i];
                if (s[0] != '.') s = '.' + s;
                extensions.Add(s);
            }
            DIR = extensions.ToArray();
            Console.WriteLine(DIR[0]);

        }
        protected static void ShowHelp()
        {
            Console.WriteLine(string.Format("{0,-30} {1,-30}", "OPTION", "FUNCTION"));
            foreach (KeyValuePair<string, string> kv in commands) Console.WriteLine(string.Format("{0,-30} {1,-30}", kv.Key, kv.Value));
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
            Console.WriteLine("Unity meta-data file analyzer for project smell analysis\n");
        }
    }
}

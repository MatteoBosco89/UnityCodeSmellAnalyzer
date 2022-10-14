using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;


namespace MetaSmellDetector
{
    public static class MetaDetector
    {
        private static JObject mainData;
        private static JObject metaData;
        private static bool isDataLoaded = false;
        private static string smellFile="smell.txt";
        private static JArray smells = new JArray();
        private static int logLevel = 1;
        private static bool expose= false;

        public static void Init(Options opt)
        {
            if (opt.DataPath.Count() > 0) {
                Console.Write(opt.DataPath.Count());
                    LoadData(opt.DataPath); }
            if (opt.SmellPath != null) smellFile = opt.SmellPath;
            if (opt.Verbose) Logger.Verbose = true;
            if (opt.Expose) expose = true;
            Logger.SetLogLevel(logLevel);
            Logger.LogFile = "metaLinter.log";
            Logger.Start();
      
        }

        public static void LoadData(IEnumerable<string> paths)
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading DataSet...");
            try
            {
                mainData = JObject.Parse(File.ReadAllText(paths.ElementAt(0)));
                if (paths.Count() > 1) metaData = JObject.Parse(File.ReadAllText(paths.ElementAt(1)));
                isDataLoaded = true;
            }
            catch (FileNotFoundException)
            {
                Logger.Log(Logger.LogLevel.Debug, "DataSet Not Found ");
                isDataLoaded = false;
            }
            catch (JsonReaderException)
            {
                Logger.Log(Logger.LogLevel.Debug, "DataSet Not Jsonfile");
                isDataLoaded = false;
            }

            Logger.Log(Logger.LogLevel.Debug, "Done!");

        }

        public static void Analyze()
        {
            Logger.Log(Logger.LogLevel.Debug, "Start analisys...");
            if (expose)
            {
                ExposeSmellMethod();
                return;
            }

            if (!isDataLoaded)
            {
                return;
            }
            JArray data = mainData["ObjectsData"] as JArray;
            if (metaData != null)
            {
                JArray data1 = metaData["ObjectsData"] as JArray;
                data.Union(data1);
            }
            Console.WriteLine("Searching :");

            SearchSmell(data);
            if (!isDataLoaded)
            {
                return;
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            SaveResult();
        }

        public static void ExposeSmellMethod()
        {
            List<string> methods = MetaExtractor.SmellsMethods();
            string text = "";
            foreach(string s in methods)
            {
                text += s + "\n";
            }
            File.WriteAllText("smellsmethods.txt", text);
        }


        public static void SearchSmell(JArray data)
        {

            try
            {
                string[] lines = File.ReadAllLines(smellFile);
                foreach(string c in lines)
                {
                    JObject result = MetaExtractor.InvokeMethods(c, data);
                    if(result!= null)
                    {
                        smells.Add(result);
                    }
            
                }
            }
            catch (FileNotFoundException)
            {

                Logger.Log(Logger.LogLevel.Debug, "Smell File Not Found");
                isDataLoaded = false;
            }

        }

        public static void SaveResult()
        {
            JObject result = new JObject();
            result.Add("ProjectPath", mainData["ProjectPath"]);
            result.Add("SmellList", smells);
            File.WriteAllText("results.json", result.ToString());
            Logger.Log(Logger.LogLevel.Debug, "Save Completed");
        }
    }
}


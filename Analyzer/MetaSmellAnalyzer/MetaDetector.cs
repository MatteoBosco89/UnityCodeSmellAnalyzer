using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaSmellDetector
{
    public static class MetaDetector
    {
        private static JArray mainData = new JArray();
        private static JArray metaData = new JArray();
        private static bool isDataLoaded = false;
        private static string smellFile="smell.txt";
        private static JArray smells = new JArray();
        private static string[] DIR = { ".json" };
        private static int logLevel = 1;
        private static bool expose= false;
        private static IEnumerable<string> dataPath;

        public static void Init(Options opt)
        {
            if (opt.DataPath.Count() > 0) dataPath = opt.DataPath;
            if (opt.SmellPath != null) smellFile = opt.SmellPath;
            if (opt.Verbose) Logger.Verbose = true;
            if (opt.Expose) expose = true;
            Logger.SetLogLevel(logLevel);
            Logger.LogFile = "metaLinter.log";
            Logger.Start();
        }

        public static void LoadData()
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading DataSet...");
            try
            {
                Logger.Log(Logger.LogLevel.Debug, "Loading main data...");
                List<string> files = Directory.GetFiles(dataPath.ElementAt(0), "*.*", SearchOption.AllDirectories).Where(f => DIR.Any(f.ToLower().EndsWith)).ToList();
                foreach (string file in files)
                {
                    string text = File.ReadAllText(file);
                    try
                    {
                        JObject j = JObject.Parse(text);
                        mainData.Add(j);
                        Logger.Log(Logger.LogLevel.Debug, $"File: {file}");
                    }
                    catch (JsonReaderException)
                    {
                        Logger.Log(Logger.LogLevel.Debug, $"File: {file} is not json");
                    }                  
                }
                Logger.Log(Logger.LogLevel.Debug, "Main data loaded!");
                if (dataPath.Count() > 1)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Loading meta data...");
                    files = Directory.GetFiles(dataPath.ElementAt(1), "*.*", SearchOption.AllDirectories).Where(f => DIR.Any(f.ToLower().EndsWith)).ToList();
                    foreach (string file in files)
                    {
                        string text = File.ReadAllText(file);
                        try
                        {
                            metaData.Add(JObject.Parse(text));
                        }
                        catch (JsonReaderException)
                        {
                            Logger.Log(Logger.LogLevel.Debug, $"File: {file} is not json");
                        }
                    }
                    Logger.Log(Logger.LogLevel.Debug, "Meta data loaded!");
                }
                isDataLoaded = true;
                Logger.Log(Logger.LogLevel.Debug, "Dataset Loaded!");
            }
            catch (FileNotFoundException)
            {
                Logger.Log(Logger.LogLevel.Debug, "DataSet Not Found");
                isDataLoaded = false;
            }
           
        }

        public static void Analyze()
        {
            Logger.Log(Logger.LogLevel.Debug, "Start analisys...");
            if (expose)
            {
                ExposeSmellMethod();
                return;
            }
            LoadData();
            if (!isDataLoaded)
            {
                return;
            }
            JArray data = mainData;
            if (metaData != null)
            {
                data.Union(metaData);
            }
            SearchSmell(data);
            Logger.Log(Logger.LogLevel.Debug, "Analysis Done!");
            SaveResult();
        }

        public static void ExposeSmellMethod()
        {
            Logger.Log(Logger.LogLevel.Debug, "Exposing names of smells methods...");
            List<string> methods = MetaExtractor.SmellsMethods();
            string text = "";
            foreach(string s in methods)
            {
                text += s + "\n";
            }
            File.WriteAllText("smellsmethods.txt", text);
            Logger.Log(Logger.LogLevel.Debug, "Saved results in smellsmethods.txt");
        }


        public static void SearchSmell(JArray data)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Search completed!");
        }

        public static void SaveResult()
        {
            Logger.Log(Logger.LogLevel.Debug, "Saving results...");
            JObject result = new JObject();
            result.Add("ProjectPath", mainData.First()["ProjectPath"]);
            result.Add("SmellList", smells);
            File.WriteAllText("results.json", result.ToString());
            Logger.Log(Logger.LogLevel.Debug, "Results saved in results.json");
        }
    }
}


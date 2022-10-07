using CSharpAnalyzer;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeSmellFinder
{
    public class SmellAnalyzer
    {
        private static string dataPath = null;
        private static string smellFile = null;
        private static string resultFile = "Results.json";
        private static JObject data = null; 
        private static JArray results = new JArray();
        private static int logLevel = 1;
        private static List<string> smellsToAnalyze = new List<string>();
        private static bool expose = false;

        public static string DataPath { get { return dataPath; } set { dataPath = value; } }
        public static string SmellFile { get { return smellFile; } set { smellFile = value; } }

        public static bool Expose { get { return expose; } set { expose = value; } }
        public static void Init(Options option)
        {
            if(option.DataPath != null)dataPath = option.DataPath;
            if(option.SmellPath != null)smellFile = option.SmellPath;
            if(option.Smell != null) smellsToAnalyze.Add(option.Smell);
            if(option.Expose) expose = true;
            Logger.SetLogLevel(logLevel);
            Logger.LogFile = "Linter.log";
            Logger.Start();
        }

        public static void Analyze()
        {
            if (expose)
            {
                ExposeSmellMethod();
                return;
            }
            Logger.Log(Logger.LogLevel.Debug, "Start analisys...");
            LoadData();
            LoadSmells();
            if (smellsToAnalyze.Count < 0)
            {
                Logger.Log(Logger.LogLevel.Debug, "No smells to anlyze, SmellFile.txt empty");
                return;
            }
            if(data == null)
            {
                Logger.Log(Logger.LogLevel.Debug, "No Json Object to analyze");
                return;
            }
            SearchSmell();
            SaveResults();
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }

        public static void LoadData()
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading Dataset...");
            try
            {
                string s = File.ReadAllText(dataPath);
                data = JObject.Parse(s);
            }
            catch (FileNotFoundException)
            {
                Logger.Log(Logger.LogLevel.Debug, "Dataset.json not found");
            }
            catch (JsonReaderException)
            {
                Logger.Log(Logger.LogLevel.Debug, "Dataset.json doesn't contains a Json Object");
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }
        public static void LoadSmells()
        {
            Logger.Log(Logger.LogLevel.Debug, "Loading smell list...");
            if(smellFile == null && smellsToAnalyze.Count <= 0)
            {
                ExposeSmellMethod();
                return;
            }
            try
            {
                string[] smells = File.ReadAllLines(smellFile);
                foreach(string smell in smells)
                {
                    smellsToAnalyze.Add(smell);
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Log(Logger.LogLevel.Debug, "SmellFile.txt not found");
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }

        public static void SearchSmell()
        {
            Logger.Log(Logger.LogLevel.Debug, "Starting smell search...");
            JArray project = data["Project"] as JArray;
            results = SmellDetector.DetectAllSmellsFromList(project, smellsToAnalyze);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }

        public static void SaveResults()
        {
            Logger.Log(Logger.LogLevel.Debug, $"Saving results to {resultFile}...");
            File.WriteAllText(resultFile, results.ToString());
            Logger.Log(Logger.LogLevel.Debug, "Done!");
        }

        public static void ExposeSmellMethod()
        {
            smellsToAnalyze = SmellDetector.ExposeAllMethods();
            if (!expose) return;
            string save = "";
            foreach(string s in smellsToAnalyze)
            {
                save += s + "\n";
            }
            File.WriteAllText("SmellFile.txt", save);
        }
    }
}


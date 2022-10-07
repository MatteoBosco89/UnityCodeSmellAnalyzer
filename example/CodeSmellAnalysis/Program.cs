using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeSmellFinder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText("C:\\Users\\pasqu\\Documents\\Università\\Tesi\\UnityCodeSmellAnalyzer\\Analyzer\\UnityCodeSmellAnalyzer\\bin\\Debug\\net472\\results.json");
            JObject o = JObject.Parse(text);
            JArray data = o["Project"] as JArray;
            JArray results = SmellDetector.DetectAllSmells(data);
            File.WriteAllText("Smells.json", results.ToString());
        }
    }
}

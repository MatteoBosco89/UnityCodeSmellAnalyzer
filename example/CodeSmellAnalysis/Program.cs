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
            JArray data = JArray.Parse(text);
            List<string> types = Utility.GetAllType(data, new List<string> { "Classes", "Interfaces" }, "Name");
            JArray results = new JArray();
            results.Add(SmellDetector.StaticCoupling(data, types, new List<string> { "SerializeField" }));
            results.Add(SmellDetector.DependenciesBetweenObjects(data, types, new List<string> { "GetComponent" }));
            results.Add(SmellDetector.ImproperCollider(data, new List<string> { "UnityEngine.MeshCollider" }, new List<string> { "GetComponent" }));
            results.Add(SmellDetector.WeakTemporization(data, new List<string> { "FixedUpdate" }, new List<string> { "Assignment", "Definition" }, new List<string> { "Time.time" }));
            results.Add(SmellDetector.LackOfSeparationOfConcern(data, new List<string> { "UnityEngine." }));
            results.Add(SmellDetector.SingletonPattern(data));
            results.Add(SmellDetector.InstantiateDestroy(data));
            results.Add(SmellDetector.FindMethods(data));
            results.Add(SmellDetector.PoorStateDesign(data));
            results.Add(SmellDetector.VelocityChange(data));
            results.Add(SmellDetector.CountinuouslyCheckingPositionRotation(data));
            File.WriteAllText("Smells.json", results.ToString());
        }
    }
}

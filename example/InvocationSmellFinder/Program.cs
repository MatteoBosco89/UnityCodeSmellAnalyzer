using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvocationSmellFinder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText("C:\\Users\\pasqu\\Documents\\Università\\Tesi\\UnityCodeSmellAnalyzer\\Analyzer\\UnityCodeSmellAnalyzer\\bin\\Debug\\net472\\results.json");
            JArray data = JArray.Parse(text);
            InvocationSmell.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Instantiate", "Destroy" });
            InvocationSmell.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Find", "FindGameObjectWithTag", "FindGameObjectsWithTag", "FindObjectOfType" });
        }
    }
}

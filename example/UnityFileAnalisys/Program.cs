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
using UnityAnalyzer;

namespace UnityFileProva
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UnityData d = new UnityData("Files\\Futuristic.fbx.meta");
            d.SaveDataToJsonFile(@"Files\\Prova.json");
        }
    }
}

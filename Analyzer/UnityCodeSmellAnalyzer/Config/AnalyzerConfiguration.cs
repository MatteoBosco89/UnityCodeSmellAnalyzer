using Microsoft.CodeAnalysis.CSharp;
using System;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Utility Class representing configurations of the Analyzer such as:
    /// Statements, Assemblies Path, Project Path, The CSharpCompilation, Miscellaneous
    /// </summary>
    public static class AnalyzerConfiguration
    {
        private static bool statementsVerbose = false;
        private static string assembliesPath = "Assemblies.conf";
        private static string projectPath = null;
        private static CSharpCompilation compilation;

        public static CSharpCompilation Compilation
        {
            get { return compilation; }
            set { compilation = value; }
        }

        public static bool StatementVerbose
        {
            get { return statementsVerbose; }
            set { statementsVerbose = value; }
        }

        public static string ProjectPath
        {
            get { return projectPath; }
            set { projectPath = value; }
        }

        public static string AssembliesPath
        {
            get { return assembliesPath; }
            set { assembliesPath = value; }
        }

    }
}


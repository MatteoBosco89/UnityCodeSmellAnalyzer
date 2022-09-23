﻿using System;

namespace UnityCodeSmellAnalyzer
{
    public static class AnalyzerConfiguration
    {
        private static bool statementsVerbose = false;
        private static string assembliesPath = "Assemblies.conf";
        private static string projectPath = null;

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


using Gui;
using System;
using System.Collections.Generic;

namespace GuiModel
{
    public static class Program
    {
        private enum Utility { CSharpAnalyzer, UnityDataAnalyzer, CodeSmellAnalysis, MetaSmellAnalysis }
        private static List<ThreadHandler> threadHandlers = new List<ThreadHandler>();
        private static Dictionary<string, string> parameters = new Dictionary<string, string>();
        private static UnityCodeSmellAnalyzer main;

        public static void AddParam(string name, string? value)
        {
            parameters[name] = value;
        }

        public static void Init(UnityCodeSmellAnalyzer window)
        {
            main = window;
            StartGathering();
        }

        private static void StartGathering()
        {
            threadHandlers.Add(new ThreadHandler("CSharpAnalyzer\\CSharpAnalyzer.exe", CreateCommand(Utility.CSharpAnalyzer), "CSharpAnalyzer", main));
        }
        private static void StartAnalysis()
        {

        }
        
        private static string CreateCommand(Utility utility)
        {
            string command = "";
            switch (utility)
            {
                case Utility.CSharpAnalyzer:
                    command += "--verbose ";
                    command += " --project " + "\"" + parameters["projectFolder"] + "\"";
                    command += " --name " + parameters["projectName"];
                    if (parameters["directory"] != "WholeProject") command += " --directory " + "\"\\" + parameters["directory"] + "\"";
                    command += " --log " + parameters["logLevel"];
                    break;
                case Utility.UnityDataAnalyzer:
                    break;
                case Utility.CodeSmellAnalysis:
                    break;
                case Utility.MetaSmellAnalysis:
                    break;
                default:
                    break;
            }
            return command;
        }


    }
}


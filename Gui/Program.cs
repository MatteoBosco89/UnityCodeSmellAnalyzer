using Gui;
using System;
using System.Collections.Generic;

namespace GuiModel
{
    /// <summary>
    /// Program Logic
    /// </summary>
    public static class Program
    {
        private enum Utility { CSharpAnalyzer, UnityDataAnalyzer, CodeSmellAnalysis, MetaSmellAnalysis }
        private static List<ThreadHandler> threadHandlers = new List<ThreadHandler>();
        private static Dictionary<string, string> parameters = new Dictionary<string, string>();
        private static UnityCodeSmellAnalyzer main;
        /// <summary>
        /// Adds a parameter in the parameters <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="name">Param Name</param>
        /// <param name="value">Param Value</param>
        public static void AddParam(string name, string? value)
        {
            parameters[name] = value;
        }
        /// <summary>
        /// Starts processes.
        /// </summary>
        /// <param name="window">Main Gui</param>
        public static void Init(UnityCodeSmellAnalyzer window)
        {
            main = window;
            StartAnalysis();
        }
        /// <summary>
        /// Starts the Analysis
        /// </summary>
        private static void StartAnalysis()
        {
            threadHandlers.Add(new ThreadHandler("CSharpAnalyzer\\CSharpAnalyzer.exe", CreateCommand(Utility.CSharpAnalyzer), "CSharpAnalyzer", main));
            threadHandlers.Add(new ThreadHandler("UnityDataAnalyzer\\UnityDataAnalyzer.exe", CreateCommand(Utility.UnityDataAnalyzer), "UnityDataAnalyzer", main));
        }
        /// <summary>
        /// Starts Smell Detecion
        /// </summary>
        private static void StartDetection()
        {

        }
        /// <summary>
        /// Decodes parameters.
        /// </summary>
        /// <param name="utility">Process to launch</param>
        /// <returns></returns>
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
                    command += "--verbose ";
                    string directory = parameters["projectFolder"];
                    if (parameters["directory"] != "WholeProject") directory += "\"\\" + parameters["directory"] + "\"";
                    command += " --assets " + "\"" + directory + "\"";
                    if (parameters["nometa"] == "1") command += " --nometa";
                    if (parameters.ContainsKey("ext")) command += " --ext " + "\"" + parameters["ext"] + "\"";
                    command += " --log " + parameters["logLevel"];
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


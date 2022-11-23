using Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GuiModel
{
    /// <summary>
    /// Program Logic
    /// </summary>
    public static class Program
    {
        private enum Utility { Code, Data }
        private static ThreadHandler codeAnalysis;
        private static ThreadHandler dataAnalysis;
        private static Thread controlThread;
        private static Dictionary<string, object> parameters = new Dictionary<string, object>();
        private static UnityCodeSmellAnalyzer main;

        /// <summary>
        /// Adds a parameter in the parameters <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="name">Param Name</param>
        /// <param name="value">Param Value</param>
        public static void AddParam(string name, object value)
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
            codeAnalysis = new ThreadHandler(CodeAnalysisProcesses(), CreateCommand(Utility.Code), "Results" + Path.DirectorySeparatorChar + ProjectName() + Path.DirectorySeparatorChar + "CodeSmell" + Path.DirectorySeparatorChar, main, "CODE");
            dataAnalysis = new ThreadHandler(DataAnalysisProcesses(), CreateCommand(Utility.Data), "Results" + Path.DirectorySeparatorChar + ProjectName() + Path.DirectorySeparatorChar + "DataSmell" + Path.DirectorySeparatorChar, main, "DATA");
            controlThread = new Thread(() => StartControl());
            controlThread.Start();
        }

        private static void StartControl()
        {
            while (!codeAnalysis.Finished || !dataAnalysis.Finished) { }
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " ------------------------------------------------- "));
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " Analysis Terminated on Project " + ProjectName()));
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " Analysis Output can be found in the Results Folder and " + ProjectName() + " SubFolder"));
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " ------------------------------------------------- "));
            main.Dispatcher.Invoke(() => main.StartEnable(true));
        }

        private static string ProjectName()
        {
            return (string)parameters["projectName"];
        }
        private static List<string> CodeAnalysisProcesses()
        {
            return new List<string> { "CSharpAnalyzer/CSharpAnalyzer.exe", "CodeSmellAnalysis/CodeSmellAnalysis.exe" };
        }
        private static List<string> DataAnalysisProcesses()
        {
            return new List<string> { "UnityDataAnalyzer/UnityDataAnalyzer.exe", "MetaSmellAnalyzer/MetaSmellAnalyzer.exe" };
        }
        /// <summary>
        /// Decodes parameters.
        /// </summary>
        /// <param name="utility">Process to launch</param>
        /// <returns></returns>
        private static List<string> CreateCommand(Utility utility)
        {
            List<string> commands = new List<string>();
            string command1 = "";
            string command2 = "";
            switch (utility)
            {
                case Utility.Code:
                    command1 += "--verbose";
                    command1 += " --project " + "\"" + (string)parameters["projectFolder"] + "\"";
                    command1 += " --name " + (string)parameters["projectName"];
                    command1 += " -r ../Results/" + (string)parameters["projectName"] + "/CodeSmell";
                    if ((string)parameters["directory"] != "WholeProject") command1 += " --directory " + "\"\\" + (string)parameters["directory"] + "\"";
                    command1 += " --log " + (string)parameters["logLevel"];
                    commands.Add(command1);
                    command2 += "-d ../Results/" + parameters["projectName"] + "/CodeSmell/CodeAnalysis.json -r ../Results/" + (string)parameters["projectName"] + "/CodeSmell -v";
                    if ((bool)parameters["codeCsv"]) command2 += " -p";
                    if ((bool)parameters["codeCat"]) command2 += " -c";
                    commands.Add(command2);
                    break;
                case Utility.Data:
                    command1 += "--verbose";
                    command1 += " -d ../Results/" + (string)parameters["projectName"] + "/DataSmell";
                    string directory = (string)parameters["projectFolder"];
                    if ((string)parameters["directory"] != "WholeProject") directory += "\"\\" + (string)parameters["directory"] + "\"";
                    command1 += " --asset " + "\"" + directory + "\"";
                    if ((bool)parameters["nometa"]) command1 += " --nometa";
                    if (parameters.ContainsKey("ext")) command1 += " --ext " + "\"" + (string)parameters["ext"] + "\"";
                    command1 += " --log " + (string)parameters["logLevel"];
                    commands.Add(command1);
                    command2 += "-d ../Results/" + (string)parameters["projectName"] + "/DataSmell -r ../Results/" + (string)parameters["projectName"] + "/DataSmell -v";
                    if ((bool)parameters["dataCsv"]) command2 += " -p";
                    if ((bool)parameters["dataCat"]) command2 += " -c";
                    commands.Add(command2);
                    break;
                default:
                    break;
            }
            return commands;
        }


    }
}


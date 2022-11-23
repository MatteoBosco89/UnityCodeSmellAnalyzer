using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Gui;

namespace GuiModel
{
    /// <summary>
    /// Wrapper Class for Thread. Handles one single thread.
    /// The thread needs to handle a ProcessHandler for Process - Gui communication.
    /// </summary>
    public class ThreadHandler
    {
        protected List<ProcessHandler> processList = new List<ProcessHandler>();
        protected Thread thread;
        protected ProcessHandler currentProcess;
        protected string currentProcessName;
        protected string currentWorkingDirectory;
        protected UnityCodeSmellAnalyzer main;
        protected bool finished = false;
        protected string type;

        public bool Finished { get { return finished; } }
        public ProcessHandler CurrentProcess { get { return currentProcess; } }
        public Thread Thread { get { return thread; } }
        public string Name { get { return currentProcessName; } }
        public ThreadHandler(List<string> processes, List<string> commands, string currentWorkingDir, UnityCodeSmellAnalyzer window, string type)
        {
            this.type = type;
            currentWorkingDirectory = currentWorkingDir;
            main = window;
            var pc = processes.Zip(commands, (p, c) => new { Proc = p, Comm = c });
            foreach (var i in pc) processList.Add(new ProcessHandler(i.Proc, i.Comm));
            thread = new Thread(() => StartThread());
            thread.Start();
        }
        /// <summary>
        /// Create the process with given data. Catches the Output Stream from ErrorOutput and StandardOutput.
        /// </summary>
        public void CreateProcess()
        {
            currentProcess = processList.ElementAt(0);
            processList.RemoveAt(0);
            currentProcess.CreateProcess();
            currentProcessName = currentProcess.Process.ProcessName;
            SendMessage("Begin Analysis");
            currentProcess.Process.Exited += new EventHandler(OnExit);
            currentProcess.Process.BeginOutputReadLine();
            currentProcess.Process.BeginErrorReadLine();
            currentProcess.Process.WaitForExit();
        }
        /// <summary>
        /// Starts the Thread and calls the <see cref="CreateProcess"/> Method.
        /// </summary>
        public void StartThread()
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " Started"));
            CreateProcess();
        }
        /// <summary>
        /// Event Listener, catches the Exit event of the process.
        /// </summary>
        /// <param name="sender">The process</param>
        /// <param name="args">Data Arguments</param>
        public void OnExit(object? sender, EventArgs args)
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " Exited"));
            if (processList.Count > 0)
            {
                if (processList.ElementAt(0).Name == "CodeSmellAnalysis/CodeSmellAnalysis.exe")
                {
                    bool found = false;
                    FileInfo file = new FileInfo(currentWorkingDirectory + "/CodeAnalysis.json");
                    while (!found)
                    {
                        Path.GetFullPath(currentWorkingDirectory);
                        if (File.Exists(currentWorkingDirectory + "/CodeAnalysis.json"))
                        {
                            if (!IsFileLocked(file)) found = true;
                        }
                    }
                }
                CreateProcess();
            }
            else
            {
                finished = true;
                thread.Interrupt();
            }
        }
        /// <summary>
        /// Event Listener, catches the Data Redirected from the process.
        /// </summary>
        /// <param name="sender">The Process</param>
        /// <param name="e">The Redirected Data</param>
        public void SendMessage(string message)
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " " + message));
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException) { return true; }

            return false;
        }

    }
}


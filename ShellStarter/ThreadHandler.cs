using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Starter;

namespace StarterModel
{
    /// <summary>
    /// Wrapper Class for Thread. Handles one single thread.
    /// The thread needs to handle a ProcessHandler for Process - Gui communication.
    /// </summary>
    public class ThreadHandler
    { 
        protected Thread thread;
        protected List<ProcessHandler> processList = new List<ProcessHandler>();
        protected bool finished = false;
        protected ProcessHandler currentProcess;
        protected string currentProcessName;
        protected string currentWorkingDirectory;

        public ProcessHandler CurrentProcess { get { return currentProcess; } }
        public Thread Thread { get { return thread; } }
        public string Name { get { return currentProcessName; } }
        public bool Finished { get { return finished; } }
        public ThreadHandler(List<string> processes, List<string> commands, string currentWorkingDirectory)
        {
            this.currentWorkingDirectory = currentWorkingDirectory;
            var pc = processes.Zip(commands, (p, c) => new { Proc = p, Comm = c });
            foreach(var i in pc) processList.Add(new ProcessHandler(i.Proc, i.Comm));
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
            UnityCodeSmellAnalyzer.WriteOutput(DateTime.Now + " " + Name + " Started");
            CreateProcess();
        }
        /// <summary>
        /// Event Listener, catches the Exit event of the process.
        /// </summary>
        /// <param name="sender">The process</param>
        /// <param name="args">Data Arguments</param>
        public void OnExit(object sender, EventArgs args)
        {
            try
            {
                UnityCodeSmellAnalyzer.WriteOutput(DateTime.Now + " " + Name + " Log can be found in " + currentWorkingDirectory);
                UnityCodeSmellAnalyzer.WriteOutput(DateTime.Now + " " + Name + " Exited. Process PID " + currentProcess.Process.Id);
            }catch(Exception) { }

            if (processList.Count > 0)
            {
                if(processList.ElementAt(0).Name == "CodeSmellAnalyzer.exe")
                {
                    bool found = false;
                    FileInfo file = new FileInfo(currentWorkingDirectory + "/CodeAnalysis.json");
                    while (!found)
                    {
                        Path.GetFullPath(currentWorkingDirectory);
                        if (File.Exists(currentWorkingDirectory + "/CodeAnalysis.json"))
                        {
                            if(!IsFileLocked(file)) found = true;
                        }
                    }
                }
                CreateProcess();
            }
            else
            {
                finished = true;
                thread.Abort();
            }
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



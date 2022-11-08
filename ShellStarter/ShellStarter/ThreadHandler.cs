using System;
using System.Diagnostics;
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
        protected UnityCodeSmellAnalyzer main;
        protected List<ProcessHandler> processList = new List<ProcessHandler>();
        protected bool finished = false;
        protected ProcessHandler currentProcess;
        protected string currentProcessName;

        public ProcessHandler CurrentProcess { get { return currentProcess; } }
        public Thread Thread { get { return thread; } }
        public string Name { get { return currentProcessName; } }
        public bool Finished { get { return finished; } }
        public ThreadHandler(List<string> processes, List<string> commands, UnityCodeSmellAnalyzer window)
        {
            main = window;
            var pc = processes.Zip(commands, (p, c) => new { Proc = p, Comm = c });
            foreach(var i in pc) processList.Add(new ProcessHandler(i.Proc, i.Comm));
            thread = new Thread(() => StartThread());
            CreateProcess();
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
            currentProcess.Process.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
            currentProcess.Process.ErrorDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
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
            main.WriteOutput(DateTime.Now + " " + Name + " Started");            
        }
        /// <summary>
        /// Event Listener, catches the Exit event of the process.
        /// </summary>
        /// <param name="sender">The process</param>
        /// <param name="args">Data Arguments</param>
        public void OnExit(object? sender, EventArgs args)
        {
            main.WriteOutput(DateTime.Now + " " + Name + " Exited");
            if (processList.Count > 0) CreateProcess();
            else finished = true;
        }
        /// <summary>
        /// Event Listener, catches the Data Redirected from the process.
        /// </summary>
        /// <param name="sender">The Process</param>
        /// <param name="e">The Redirected Data</param>
        public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            main.WriteOutput(DateTime.Now + " " + Name + " " + e.Data);
        }

    }
}



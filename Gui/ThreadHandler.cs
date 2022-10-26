using System;
using System.Diagnostics;
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
        protected ProcessHandler processHandler;
        protected Thread thread;
        protected string processName;
        protected UnityCodeSmellAnalyzer main;

        public ProcessHandler ProcessHandler { get { return processHandler; } }
        public Thread Thread { get { return thread; } }
        public string Name { get { return processName; } }
        public ThreadHandler(string process, string commands, string name, UnityCodeSmellAnalyzer window)
        {
            main = window;
            processName = name;
            processHandler = new ProcessHandler(process, commands);
            thread = new Thread(() => StartThread());
            thread.Start();
        }
        /// <summary>
        /// Create the process with given data. Catches the Output Stream from ErrorOutput and StandardOutput.
        /// </summary>
        public void CreateProcess()
        {
            processHandler.CreateProcess();
            processHandler.Process.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
            processHandler.Process.ErrorDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
            processHandler.Process.Exited += new EventHandler(OnExit);
            processHandler.Process.BeginOutputReadLine();
            processHandler.Process.BeginErrorReadLine();
            processHandler.Process.WaitForExit();
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
        }
        /// <summary>
        /// Event Listener, catches the Data Redirected from the process.
        /// </summary>
        /// <param name="sender">The Process</param>
        /// <param name="e">The Redirected Data</param>
        public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " " + e.Data));
        }

    }
}


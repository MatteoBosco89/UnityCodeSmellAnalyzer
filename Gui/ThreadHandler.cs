using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Gui;

namespace GuiModel
{
    public class ThreadHandler
    {
        protected ProcessHandler processHandler;
        protected Thread thread;
        protected string processName;
        protected UnityCodeSmellAnalyzer main;

        public ProcessHandler ProcessHandler { get { return processHandler; } }
        public Thread Thread { get { return thread; } }
        public string Name { get { return processName; } }

        public ThreadHandler(string process, string commands, string name, UnityCodeSmellAnalyzer main)
        {
            this.main = main;
            processName = name;
            processHandler = new ProcessHandler(process, commands);
            thread = new Thread(() => StartThread());
            thread.Start();
        }

        public void CreateProcess()
        {
            processHandler.CreateProcess();
            processHandler.Process.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
            processHandler.Process.Exited += new EventHandler(OnExit);
            processHandler.Process.BeginOutputReadLine();
            processHandler.Process.WaitForExit();
        }

        public void StartThread()
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " Started"));
            CreateProcess();
        }

        public void OnExit(object? sender, EventArgs args)
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " Exited"));
        }
        public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            main.Dispatcher.Invoke(() => main.WriteOutput(DateTime.Now + " " + Name + " " + e.Data));
        }

    }
}


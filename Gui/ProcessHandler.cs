using System;
using System.Diagnostics;
using System.IO;

namespace GuiModel
{
    /// <summary>
    /// Wrapper Class for the Process, handles all the process configurations and Life Cycle.
    /// </summary>
    public class ProcessHandler
    {
        protected string name;
        protected string arguments;
        protected bool redirectOutput = true;
        protected bool shellExecute = false;
        protected bool createNoWindow = true;
        protected Process process;


        public Process Process { get { return process; } }
        public string Name { get { return name; } }

        public ProcessHandler(string name, string arguments, bool redirectOutput, bool shellExecute, bool createNoWindow)
        {
            this.name = name;
            this.arguments = arguments;
            this.redirectOutput = redirectOutput;
            this.shellExecute = shellExecute;
            this.createNoWindow = createNoWindow;
            process = new Process();
        }

        public ProcessHandler(string name, string arguments)
        {
            this.name = name;
            this.arguments = arguments;
            process = new Process();
        }
        /// <summary>
        /// Create the process with the given configurations.
        /// </summary>
        public void CreateProcess()
        {
            process.StartInfo.FileName = name;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = redirectOutput;
            process.StartInfo.RedirectStandardError = redirectOutput;
            process.StartInfo.CreateNoWindow = createNoWindow;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(name);
            process.StartInfo.UseShellExecute = shellExecute;
            process.EnableRaisingEvents = true;
            process.Start();
        }

    }
}


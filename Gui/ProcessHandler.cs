using System;
using System.Diagnostics;

public class ProcessHandler
{
	protected string name;
	protected string arguments;
	protected bool redirectOutput = true;
	protected bool shellExecute = false;
	protected bool createNoWindow = true;
	protected Process process;

	public Process Process { get { return process; } }

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

	public void CreateProcess()
	{
		process.StartInfo.FileName = name;
        process.StartInfo.Arguments = arguments;
		process.StartInfo.RedirectStandardOutput = redirectOutput;
		process.StartInfo.CreateNoWindow = createNoWindow;
		process.StartInfo.UseShellExecute = shellExecute;
		process.Start();
    }

}

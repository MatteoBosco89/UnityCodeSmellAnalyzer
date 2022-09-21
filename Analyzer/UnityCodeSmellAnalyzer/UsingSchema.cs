using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class UsingSchema
	{
		protected string name;
		protected int line;

		public string Name { get { return name; } }
		public int Line { get { return line; } }

		public UsingSchema(string name, int line)
		{
			this.name = name;
			this.line = line;
		}
	}
}

using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class NamespaceSchema : CompilationUnit
	{
		protected int line;

		public int Line { get { return line; } }

		public NamespaceSchema(string name, int line) : base(name)
		{
			this.line = line;
		}
	}
}

using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class NamespaceSchema : CompilationUnit
	{
		protected int line;

		public NamespaceSchema(string name, int line) : base(name)
		{
			this.line = line;
		}
	}
}

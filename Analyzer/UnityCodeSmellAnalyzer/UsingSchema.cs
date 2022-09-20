using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class UsingSchema
	{
		protected string name;
		protected int line;

		public UsingSchema(string name, int line)
		{
			this.name = name;
			this.line = line;
		}
	}
}

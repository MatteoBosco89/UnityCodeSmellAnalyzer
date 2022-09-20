using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class ClassSchema
	{

		protected string name;
		protected string modifier;
		protected int line;
		protected string inheritance;
		protected List<PropertySchema> properties = new List<PropertySchema>();
		protected List<MethodSchema> methods = new List<MethodSchema>();
		protected List<FieldSchema> fields = new List<FieldSchema>();

		public ClassSchema(string name, string modifier, int line, string inheritance)
		{
			this.name = name;
			this.modifier = modifier;
			this.line = line;
			this.inheritance = inheritance;
		}

		public void AddProperty(PropertySchema p)
		{
			properties.Add(p);
		}

		public void AddMethod(MethodSchema m)
		{
			methods.Add(m);
		}

		public void AddField(FieldSchema f)
		{
			fields.Add(f);
		}

	}
}

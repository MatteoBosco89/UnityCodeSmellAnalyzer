using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class ClassSchema
	{

		protected string name;
		protected string modifier;
		protected int line;
		protected string inheritance;
		protected List<string> usedInterfaces;
		protected List<PropertySchema> properties = new List<PropertySchema>();
		protected List<MethodSchema> methods = new List<MethodSchema>();
		protected List<FieldSchema> fields = new List<FieldSchema>();


		public string Name { get { return name; } }
		public string Modifier { get { return modifier; } }
		public string Inheritance { get { return inheritance; } }
		public int Line { get { return line; } }
		public List<string> Interfaces { get { return usedInterfaces; } }
		public List<PropertySchema> Properties { get { return properties; } }
		public List<MethodSchema> Methods { get { return methods; } }
		public List<FieldSchema> Fields { get { return fields; } }


		public ClassSchema(string name, string modifier, int line, string inheritance, List<string> usedInterfaces)
		{
			this.name = name;
			this.modifier = modifier;
			this.line = line;
			this.inheritance = inheritance;
			this.usedInterfaces = usedInterfaces;
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

		public void AddInterface(string i)
		{
			usedInterfaces.Add(i);
		}

		public void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;
			List<MethodDeclarationSyntax> mdsl = (from meth in theClass.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();


			foreach(var m in mdsl)
			{
				MethodSchema method = new MethodSchema(m.Identifier.ToString(), m.GetLocation().GetLineSpan().StartLinePosition.Line);
				method.LoadInformations(m, model);
				AddMethod(method);
			}
		
		}

	}
}

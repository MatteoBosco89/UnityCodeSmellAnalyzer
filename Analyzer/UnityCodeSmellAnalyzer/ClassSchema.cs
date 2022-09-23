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
		protected int line;
		protected string inheritance;
		protected List<string> modifiers = new List<string>();
		protected List<string> usedInterfaces;
		protected List<PropertySchema> properties = new List<PropertySchema>();
		protected List<MethodSchema> methods = new List<MethodSchema>();
		protected List<FieldSchema> fields = new List<FieldSchema>();


		public string Name { get { return name; } }
		public List<string> Modifiers { get { return modifiers; } }
		public string Inheritance { get { return inheritance; } }
		public int Line { get { return line; } }
		public List<string> Interfaces { get { return usedInterfaces; } }
		public List<PropertySchema> Properties { get { return properties; } }
		public List<MethodSchema> Methods { get { return methods; } }
		public List<FieldSchema> Fields { get { return fields; } }


		public ClassSchema(string name, int line, string inheritance, List<string> usedInterfaces)
		{
			this.name = name;
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

		public void AddModifier(string m)
		{
			modifiers.Add(m);
		}

		public void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;
			List<MethodDeclarationSyntax> mdsl = (from meth in theClass.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();
			List<FieldDeclarationSyntax> fdsl = (from fiel in theClass.DescendantNodes().OfType<FieldDeclarationSyntax>() select fiel).ToList();
			List<PropertyDeclarationSyntax> pdsl = (from prop in theClass.DescendantNodes().OfType<PropertyDeclarationSyntax>() select prop).ToList();

			foreach(var mod in theClass.Modifiers)
			{
				AddModifier(mod.ToString());
			}

			foreach(var m in mdsl)
			{
				
				MethodSchema method = new MethodSchema(m.Identifier.ToString(), m.GetLocation().GetLineSpan().StartLinePosition.Line, m.ReturnType.ToString());
				method.LoadInformations(m, model);
				AddMethod(method);
			}
			foreach(var f in fdsl)
			{
				string type = f.Declaration.Type.ToString();
				foreach(var d in f.Declaration.Variables)
				{
					string initializer = null;
                    if (d.Initializer != null) initializer = d.Initializer.Value.ToString();
                    FieldSchema field = new FieldSchema(d.Identifier.ToString(), type, initializer, d.GetLocation().GetLineSpan().StartLinePosition.Line);
					field.LoadInformations(f, model);
					AddField(field);
                }
			}
			foreach(var p in pdsl)
			{
				PropertySchema property = new PropertySchema(p.Identifier.ToString(), p.Type.ToString(), p.GetLocation().GetLineSpan().StartLinePosition.Line);
				property.LoadInformations(p, model);
				AddProperty(property);
			}

		
		}

	}
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class ClassSchema : SyntaxSchema
	{

		protected string name;
		protected string inheritance;
		protected List<string> modifiers = new List<string>();
		protected List<string> usedInterfaces = new List<string>();
		protected List<PropertySchema> properties = new List<PropertySchema>();
		protected List<MethodSchema> methods = new List<MethodSchema>();
		protected List<FieldSchema> fields = new List<FieldSchema>();


		public string Name { get { return name; } }
		public List<string> Modifiers { get { return modifiers; } }
		public string Inheritance { get { return inheritance; } }
		public List<string> Interfaces { get { return usedInterfaces; } }
		public List<PropertySchema> Properties { get { return properties; } }
		public List<MethodSchema> Methods { get { return methods; } }
		public List<FieldSchema> Fields { get { return fields; } }


		public ClassSchema() { }

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

		public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;
            ITypeSymbol its = model.GetDeclaredSymbol(root) as ITypeSymbol;
            inheritance = its.BaseType.Name;
            List<INamedTypeSymbol> interf = its.Interfaces.ToList();
            foreach (INamedTypeSymbol t in interf) AddInterface(t.Name);
			name = theClass.Identifier.ToString();
			line = theClass.GetLocation().GetLineSpan().StartLinePosition.Line;
			

            List<MethodDeclarationSyntax> mdsl = (from meth in theClass.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();
			List<FieldDeclarationSyntax> fdsl = (from fiel in theClass.DescendantNodes().OfType<FieldDeclarationSyntax>() select fiel).ToList();
			List<PropertyDeclarationSyntax> pdsl = (from prop in theClass.DescendantNodes().OfType<PropertyDeclarationSyntax>() select prop).ToList();

			foreach(var mod in theClass.Modifiers)
			{
				AddModifier(mod.ToString());
			}
			foreach(var m in mdsl)
			{
                MethodSchema method = new MethodSchema();
				method.LoadInformations(m, model);
				AddMethod(method);
			}
			foreach(var f in fdsl)
			{
				foreach(VariableDeclaratorSyntax v in f.Declaration.Variables)
				{
                    FieldSchema field = new FieldSchema();
                    field.LoadInformations(f, v, model);
                    AddField(field);
                }
                
            }
			foreach(var p in pdsl)
			{
				PropertySchema property = new PropertySchema();
				property.LoadInformations(p, model);
				AddProperty(property);
			}

		
		}

	}
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
	/// <summary>
	/// Class representing the Class Declaration. 
	/// Informations gathered: Name, Inheritance, Fields, Methods, Properties, Modifiers, Interfaces, Attributes, LOC
	/// </summary>
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
		protected List<string> attributes = new List<string>();


		public string Name { get { return name; } }
		public List<string> Modifiers { get { return modifiers; } }
		public string Inheritance { get { return inheritance; } }
		public List<string> Interfaces { get { return usedInterfaces; } }
		public List<PropertySchema> Properties { get { return properties; } }
		public List<MethodSchema> Methods { get { return methods; } }
		public List<FieldSchema> Fields { get { return fields; } }
		public List<string> Attributes { get { return attributes; } }


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
        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

		/// <summary>
		/// Loads Methods of the Class
		/// </summary>
		/// <param name="mdsl">List of all methods found</param>
		/// <param name="model">The model</param>
		protected void LoadMethods(List<MethodDeclarationSyntax> mdsl, SemanticModel model)
		{
            foreach (var m in mdsl)
            {
                MethodSchema method = new MethodSchema();
                method.LoadInformations(m, model);
                AddMethod(method);
            }
        }
		/// <summary>
		/// Loads Fields of the Class
		/// </summary>
		/// <param name="fdsl">List of fields found</param>
		/// <param name="model">The model</param>
		protected void LoadFields(List<FieldDeclarationSyntax> fdsl, SemanticModel model)
		{
            foreach (var f in fdsl)
            {
                foreach (VariableDeclaratorSyntax v in f.Declaration.Variables)
                {
                    FieldSchema field = new FieldSchema();
                    field.LoadInformations(f, v, model);
                    AddField(field);
                }
            }
        }
		/// <summary>
		/// Loads Properties of the Class
		/// </summary>
		/// <param name="pdsl">List of properties found</param>
		/// <param name="model">The model</param>
		protected void LoadProperties(List<PropertyDeclarationSyntax> pdsl, SemanticModel model)
		{
            foreach (var p in pdsl)
            {
                PropertySchema property = new PropertySchema();
                property.LoadInformations(p, model);
                AddProperty(property);
            }
        }
		/// <summary>
		/// Loads Attributes of the Class
		/// </summary>
		/// <param name="theClass">The Class Declared</param>
		protected void LoadAttributes(ClassDeclarationSyntax theClass)
		{
            foreach (var attr in theClass.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }
		/// <summary>
		/// Loads Modifiers of the Class
		/// </summary>
		/// <param name="theClass">The Class Declared</param>
		protected void LoadModifiers(ClassDeclarationSyntax theClass)
		{
            foreach (var mod in theClass.Modifiers) AddModifier(mod.ToString());
        }


        public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;

			LoadBasicInformations(root, model);

            List<MethodDeclarationSyntax> mdsl = (from meth in theClass.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();
			List<FieldDeclarationSyntax> fdsl = (from fiel in theClass.DescendantNodes().OfType<FieldDeclarationSyntax>() select fiel).ToList();
			List<PropertyDeclarationSyntax> pdsl = (from prop in theClass.DescendantNodes().OfType<PropertyDeclarationSyntax>() select prop).ToList();

			LoadMethods(mdsl, model);
			LoadFields(fdsl, model);
			LoadProperties(pdsl, model);
			LoadAttributes(theClass);
			LoadModifiers(theClass);
		}

		public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
		{
            ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;
            ITypeSymbol its = model.GetDeclaredSymbol(root) as ITypeSymbol;
            inheritance = its.BaseType.Name;
            List<INamedTypeSymbol> interf = its.Interfaces.ToList();
            foreach (INamedTypeSymbol t in interf) AddInterface(t.Name);
            name = theClass.Identifier.ToString();
            line = theClass.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
	}
}

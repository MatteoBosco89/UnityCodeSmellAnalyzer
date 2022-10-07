using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace CSharpAnalyzer
{
	/// <summary>
	/// Class representing the Class Declaration. 
	/// Informations gathered: Name, Inheritance, Fields, Methods, Properties, Modifiers, Interfaces, Attributes, LOC
	/// </summary>
	[Serializable]
	public class ClassSchema : SyntaxSchema
	{

		protected string name;
		protected string fullName;
		protected string inheritance;
		protected string fullInheritanceName;
		protected List<string> classConstraints = new List<string>();
		protected List<string> modifiers = new List<string>();
		protected List<string> usedInterfaces = new List<string>();
		protected List<PropertySchema> properties = new List<PropertySchema>();
		protected List<MethodSchema> methods = new List<MethodSchema>();
		protected List<FieldSchema> fields = new List<FieldSchema>();
		protected List<ClassSchema> innerClasses = new List<ClassSchema>();
		protected List<ConstructorSchema> constructors = new List<ConstructorSchema>();
		protected List<string> attributes = new List<string>();
		protected List<string> constraints = new List<string>();


		public string Name { get { return name; } }
		public string FullName { get { return fullName; } }
		public string FullInheritanceName { get { return fullInheritanceName; } }
		public List<ClassSchema> InnerClasses { get { return innerClasses; } }
		public List<string> ClassConstraints { get { return classConstraints; } }
		public List<string> Modifiers { get { return modifiers; } }
		public string Inheritance { get { return inheritance; } }
		public List<string> Interfaces { get { return usedInterfaces; } }
		public List<PropertySchema> Properties { get { return properties; } }
		public List<MethodSchema> Methods { get { return methods; } }
		public List<FieldSchema> Fields { get { return fields; } }
		public List<ConstructorSchema> Constructors { get { return constructors; } }
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
        public void AddConstructor(ConstructorSchema c)
        {
			constructors.Add(c);
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
		public void AddConstraint(string c)
		{
			constraints.Add(c);
		}
		public void AddInnerClass(ClassSchema c)
		{
			innerClasses.Add(c);
		}

		/// <summary>
		/// Loads Methods of the Class
		/// </summary>
		/// <param name="mdsl">List of all methods found</param>
		/// <param name="model">The model</param>
		protected void LoadMethods(SyntaxNode root, List<MethodDeclarationSyntax> mdsl, SemanticModel model)
		{
            foreach (var m in mdsl)
            {
                if (SyntaxWalker.SearchParent(m, SyntaxWalker.ClassAncestors) == root)
				{
                    MethodSchema method = new MethodSchema();
                    method.LoadInformations(m, model);
                    AddMethod(method);
                }
                    
            }
        }
		/// <summary>
		/// Loads Fields of the Class
		/// </summary>
		/// <param name="fdsl">List of fields found</param>
		/// <param name="model">The model</param>
		protected void LoadFields(SyntaxNode root, List<FieldDeclarationSyntax> fdsl, SemanticModel model)
		{
            foreach (var f in fdsl)
            {
                foreach (VariableDeclaratorSyntax v in f.Declaration.Variables)
                {
					if (SyntaxWalker.SearchParent(f, SyntaxWalker.ClassAncestors) == root)
					{
                        FieldSchema field = new FieldSchema();
                        field.LoadInformations(f, v, model);
                        AddField(field);
                    }
                    
                }
            }
        }
		/// <summary>
		/// Loads Properties of the Class
		/// </summary>
		/// <param name="pdsl">List of properties found</param>
		/// <param name="model">The model</param>
		protected void LoadProperties(SyntaxNode root, List<PropertyDeclarationSyntax> pdsl, SemanticModel model)
		{
            foreach (var p in pdsl)
            {
				if (SyntaxWalker.SearchParent(p, SyntaxWalker.ClassAncestors) == root)
				{
					PropertySchema property = new PropertySchema();
					property.LoadInformations(p, model);
					AddProperty(property);
				}
            }
        }
		/// <summary>
		/// Loads Class Constructors
		/// </summary>
		/// <param name="cdsl">List of Constructors</param>
		/// <param name="model">The model</param>
        protected void LoadConstructors(SyntaxNode root, List<ConstructorDeclarationSyntax> cdsl, SemanticModel model)
        {
            foreach (var c in cdsl)
            {
				if (SyntaxWalker.SearchParent(c, SyntaxWalker.ClassAncestors) == root)
				{
					ConstructorSchema constructor = new ConstructorSchema();
                    ITypeSymbol its = model.GetDeclaredSymbol(root) as ITypeSymbol;
                    constructor.ClassRef = its.ToString();
					constructor.ReturnType = (root as ClassDeclarationSyntax).Identifier.ToString();
                    constructor.LoadInformations(c, model);
					AddConstructor(constructor);
				}
            }
        }
		/// <summary>
		/// Loads all Inner Classes inside the Class
		/// </summary>
		/// <param name="root">The CLass Declaration</param>
		/// <param name="cdsl">List of Inner Classes</param>
		/// <param name="model">The model</param>
		protected void LoadInnerClasses(SyntaxNode root, List<ClassDeclarationSyntax> cdsl, SemanticModel model)
		{
			foreach (var c in cdsl)
			{
				if (SyntaxWalker.SearchParent(c, SyntaxWalker.ClassAncestors) == root)
				{
					ClassSchema innerClass = new ClassSchema();
                    innerClass.LoadInformations(c, model);
					AddInnerClass(innerClass);
				}
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
		/// <summary>
		/// Loads Constraints of the Class ( i.e. public class Class1<T> where T : new() )
		/// </summary>
		/// <param name="theClass">The Class Declared</param>
		protected void LoadConstraints(ClassDeclarationSyntax theClass)
		{
            foreach (var cc in theClass.ConstraintClauses)
            {
                foreach (var c in cc.Constraints)
                {
					AddConstraint(c.ToString());
                }
            }
        }
		
        public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;

			LoadBasicInformations(root, model);

            List<MethodDeclarationSyntax> mdsl = (from meth in theClass.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();
			List<FieldDeclarationSyntax> fdsl = (from fiel in theClass.DescendantNodes().OfType<FieldDeclarationSyntax>() select fiel).ToList();
			List<PropertyDeclarationSyntax> pdsl = (from prop in theClass.DescendantNodes().OfType<PropertyDeclarationSyntax>() select prop).ToList();
			List<ConstructorDeclarationSyntax> cdss = (from ci in theClass.DescendantNodes().OfType<ConstructorDeclarationSyntax>() select ci).ToList();
			List<ClassDeclarationSyntax> cdsl = (from ic in theClass.DescendantNodes().OfType<ClassDeclarationSyntax>() select ic).ToList();
            LoadMethods(theClass, mdsl, model);
			LoadFields(theClass, fdsl, model);
			LoadProperties(theClass, pdsl, model);
			LoadAttributes(theClass);
			LoadModifiers(theClass);
			LoadConstraints(theClass);
			LoadConstructors(theClass, cdss, model);
			LoadInnerClasses(theClass, cdsl, model);
        }

		public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
		{
            ClassDeclarationSyntax theClass = root as ClassDeclarationSyntax;
            ITypeSymbol its = model.GetDeclaredSymbol(root) as ITypeSymbol;
            inheritance = its.BaseType.ToString();
            List<INamedTypeSymbol> interf = its.Interfaces.ToList();
            foreach (INamedTypeSymbol t in interf) AddInterface(t.Name);
            name = theClass.Identifier.ToString();
			fullInheritanceName = its.BaseType.ToString();
			fullName = its.ToString();
            line = theClass.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
	}
}

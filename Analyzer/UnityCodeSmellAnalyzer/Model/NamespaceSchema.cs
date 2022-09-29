using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
	/// <summary>
	/// Class representing the Namespace Declaration. 
	/// Extending Compilation Unit. 
	/// Informations gathered: Name, Usings, Namespaces, Interfaces, Classes, Attributes, LOC
	/// </summary>
	[Serializable]
	public class NamespaceSchema : CompilationUnit
	{
		protected List<string> attributes = new List<string>();
		public List<string> Attributes { get { return attributes; } }
		public void AddAttribute(string a)
		{
			attributes.Add(a);
		}
		public NamespaceSchema() { }

		public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			NamespaceDeclarationSyntax namespaceDeclaration = root as NamespaceDeclarationSyntax;
			
			base.LoadInformations(namespaceDeclaration, model);
			name = namespaceDeclaration.Name.ToString(); 
			foreach(var attr in namespaceDeclaration.AttributeLists)
			{
				foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString()); 
			}
        }
	}
}

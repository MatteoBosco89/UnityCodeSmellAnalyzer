using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class NamespaceSchema : CompilationUnit
	{
		
		public NamespaceSchema() { }

		public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			NamespaceDeclarationSyntax namespaceDeclaration = root as NamespaceDeclarationSyntax;
			base.LoadInformations(namespaceDeclaration, model);
			name = namespaceDeclaration.Name.ToString(); 

        }
	}
}

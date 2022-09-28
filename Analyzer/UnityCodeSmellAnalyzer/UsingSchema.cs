using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace UnityCodeSmellAnalyzer
{
	[Serializable]
	public class UsingSchema : SyntaxSchema
	{
		protected string name;

		public string Name { get { return name; } }


		public UsingSchema() { }

		public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			UsingDirectiveSyntax u = root as UsingDirectiveSyntax;
			name = u.Name.ToString();
			line = u.GetLocation().GetLineSpan().StartLinePosition.Line;
		}
	}
}

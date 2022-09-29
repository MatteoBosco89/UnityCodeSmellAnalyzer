using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace UnityCodeSmellAnalyzer
{
	/// <summary>
	/// Class representing the Using Directive.
	/// Informations gathered: Statement, LOC
	/// </summary>
	[Serializable]
	public class UsingSchema : SyntaxSchema
	{
		protected string name;

		public string Name { get { return name; } }


		public UsingSchema() { }

		public override void LoadInformations(SyntaxNode root, SemanticModel model)
		{
			LoadBasicInformations(root, model);
		}

		public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
		{
            UsingDirectiveSyntax u = root as UsingDirectiveSyntax;
            name = u.Name.ToString();
            line = u.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
	}
}

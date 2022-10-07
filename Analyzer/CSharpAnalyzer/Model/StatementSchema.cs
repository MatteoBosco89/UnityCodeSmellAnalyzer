using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing the actual Statement. 
    /// Informations gathered: Statement, LOC
    /// </summary>
    [Serializable]
    public class StatementSchema : SyntaxSchema
    {
        protected string statement;

        public string Statement { get { return statement; } }


        public StatementSchema() { }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            StatementSyntax s = root as StatementSyntax;
            statement = s.ToString();
            line = s.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}


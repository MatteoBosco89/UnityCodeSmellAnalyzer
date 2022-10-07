using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing a Else Statement Block. Inherit CycleOrControlSchema.
    /// Informations gathered: Statement
    /// </summary>
    [Serializable]
    public class ElseSchema : CycleOrControlSchema
    {
        protected string elseStatement;
        public string ElseStatement { get { return elseStatement; } }
        [JsonIgnore]
        public override int Depth { get { return depth; } }
        [JsonIgnore]
        public override int Line { get { return line; } }

        public ElseSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ElseClauseSyntax e = root as ElseClauseSyntax;
            elseStatement = e.Statement.ToString();
            startLine = e.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = e.GetLocation().GetLineSpan().EndLinePosition.Line;
        }

        
    }
}


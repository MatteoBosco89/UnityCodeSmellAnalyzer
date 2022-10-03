using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;

namespace UnityCodeSmellAnalyzer
{
    public class FinallySchema : CycleOrControlSchema
    {
        public FinallySchema() { }
        [JsonIgnore]
        public override int Depth { get { return depth; } }
        [JsonIgnore]
        public override int Line { get { return line; } }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            FinallyClauseSyntax f = root as FinallyClauseSyntax;
            startLine = f.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = f.GetLocation().GetLineSpan().EndLinePosition.Line;
        }
    }

}

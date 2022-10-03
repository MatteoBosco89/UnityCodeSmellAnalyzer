using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class CatchSchema : CycleOrControlSchema
    {
        public CatchSchema() { }
        protected string declaration;
        protected ConditionSchema filter;
        [JsonIgnore]
        public override int Depth { get { return depth; } }
        [JsonIgnore]
        public override int Line { get { return line; } }
        public string Declaration { get { return declaration; } }
        public ConditionSchema Filter { get { return filter; } }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {

            CatchClauseSyntax c = root as CatchClauseSyntax;
            startLine = c.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = c.GetLocation().GetLineSpan().EndLinePosition.Line;
            declaration = c.Declaration?.ToString();
            if(c.Filter != null)
            {
                filter = new ConditionSchema();
                filter.LoadInformations(c.Filter.FilterExpression, model);
            } 
        }
    }
}

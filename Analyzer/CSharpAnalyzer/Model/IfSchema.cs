using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing a If Statement Block. Inherit CycleOrControlSchema.
    /// Informations gathered: Else, Condition
    /// </summary>
    [Serializable]
    public class IfSchema : CycleOrControlSchema
    {
        protected ElseSchema elseSchema;
        protected ConditionSchema condition;
        protected bool danglingElse = true;

        public ConditionSchema Condition { get { return condition; } }
        public ElseSchema Else { get { return elseSchema; } }
        public bool DanglingElse { get { return danglingElse; } }
        [JsonIgnore]
        public override int Line { get { return line; } }
        public IfSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            IfStatementSyntax i = root as IfStatementSyntax;
            startLine = i.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = i.GetLocation().GetLineSpan().EndLinePosition.Line;
            condition = new ConditionSchema();
            condition.LoadInformations(i.Condition, model);
            if(i.Else != null)
            {
                danglingElse = false;
                elseSchema = new ElseSchema();
                elseSchema.Depth = depth;
                elseSchema.LoadInformations(i.Else, model);
            }
        }
    }
}


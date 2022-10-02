using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing a While Statement Block. Inherit CycleOrControlSchema.
    /// Informations gathered: Condition
    /// </summary>
    [Serializable]
    public class WhileSchema : CycleOrControlSchema
    {

        protected ConditionSchema condition;

        public WhileSchema() { }

        public ConditionSchema Condition { get { return condition; } }
        [JsonIgnore]
        public override int Line { get; }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            WhileStatementSyntax w = root as WhileStatementSyntax;
            startLine = w.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = w.GetLocation().GetLineSpan().EndLinePosition.Line;
            condition = new ConditionSchema();
            condition.LoadInformations(w.Condition, model);
        }

    }
}



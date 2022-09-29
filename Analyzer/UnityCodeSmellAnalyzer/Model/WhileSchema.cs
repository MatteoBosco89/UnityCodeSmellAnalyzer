using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected string condition;

        public WhileSchema() { }

        public string Condition { get { return condition; } }
        [JsonIgnore]
        public override int Line { get; }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            List<WhileStatementSyntax> wbl = (from wh in root.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<InvocationExpressionSyntax> invocations = (from wh in root.DescendantNodes().OfType<InvocationExpressionSyntax>() select wh).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in root.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<AssignmentExpressionSyntax> aesl = (from ae in root.DescendantNodes().OfType<AssignmentExpressionSyntax>() select ae).ToList();
            List<ForEachStatementSyntax> fel = (from fe in root.DescendantNodes().OfType<ForEachStatementSyntax>() select fe).ToList();
            LoadBasicInformations(root, model);
            LoadWhileStatement(root, wbl, model);
            LoadForEachStatement(root, fel, model);
            LoadInvocations(root, invocations, model);
            LoadVariables(root, vdsl, aesl, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            WhileStatementSyntax w = root as WhileStatementSyntax;
            startLine = w.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = w.GetLocation().GetLineSpan().EndLinePosition.Line;
            condition = w.Condition.ToString();
            depth = 0;
        }

    }
}



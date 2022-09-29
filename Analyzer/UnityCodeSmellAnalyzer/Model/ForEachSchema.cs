using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ForEachSchema : CycleOrControlSchema
    {
        protected string iterator;
        protected string iteratorType;
        protected string iterable;
        protected string iterableType;
        protected bool expressionIsInvocation = false;
        protected string invocation;

        public string Iterator { get { return iterator; } }
        public string IteratorType { get { return iteratorType; } }
        public string Iterable { get { return iterable; } }
        public string IterableType { get { return iterableType; } }
        public string Invocation { get { return invocation; } }
        public bool ExpressionIsInvocation { get { return expressionIsInvocation; } }
        [JsonIgnore]
        public override int Line { get; }


        public ForEachSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ForEachStatementSyntax forEach = root as ForEachStatementSyntax;
            iterator = forEach.Identifier.ToString();
            iteratorType = forEach.Type.ToString();
            iterable = forEach.Expression.ToString();
            iterableType = model.GetTypeInfo(forEach.Expression).Type.ToString();
            if (model.GetSymbolInfo(forEach.Expression).Symbol is IMethodSymbol)
            {
                expressionIsInvocation = true;
                invocation = (model.GetSymbolInfo(forEach.Expression).Symbol as IMethodSymbol).ToString();
            }
            startLine = forEach.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = forEach.GetLocation().GetLineSpan().EndLinePosition.Line;
            depth = 0;
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            List<WhileStatementSyntax> wbl = (from wh in root.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<InvocationExpressionSyntax> invocations = (from wh in root.DescendantNodes().OfType<InvocationExpressionSyntax>() select wh).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in root.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<AssignmentExpressionSyntax> aesl = (from ae in root.DescendantNodes().OfType<AssignmentExpressionSyntax>() select ae).ToList();

            LoadBasicInformations(root, model);
            LoadInvocations(root, invocations, model);
            LoadVariables(root, vdsl, aesl, model);
        }
    }
}


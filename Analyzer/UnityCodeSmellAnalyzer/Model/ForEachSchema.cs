using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing a ForEach Statement Block. Inherit CycleOrControlSchema.
    /// Informations gathered: Iterator, Iterator Type, Iterable, Iterable Type, Esxpression Is Invocation, 
    /// Expressione Is Property, Invocation, Property
    /// </summary>
    [Serializable]
    public class ForEachSchema : CycleOrControlSchema
    {
        protected string iterator;
        protected string iteratorType;
        protected string iterable;
        protected string iterableType;
        protected bool expressionIsInvocation = false;
        protected bool expressionIsProperty = false;
        protected string invocation;
        protected string property;

        public string Iterator { get { return iterator; } }
        public string IteratorType { get { return iteratorType; } }
        public string Iterable { get { return iterable; } }
        public string IterableType { get { return iterableType; } }
        public string Invocation { get { return invocation; } }
        public string Property { get { return property; } }
        public bool ExpressionIsInvocation { get { return expressionIsInvocation; } }
        public bool ExpressionIsProperty { get { return expressionIsProperty; } }
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
            if (model.GetSymbolInfo(forEach.Expression).Symbol is IPropertySymbol)
            {
                expressionIsProperty = true;
                property = (model.GetSymbolInfo(forEach.Expression).Symbol as IPropertySymbol).ToString();
            }
            startLine = forEach.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = forEach.GetLocation().GetLineSpan().EndLinePosition.Line;
        }

    }
}


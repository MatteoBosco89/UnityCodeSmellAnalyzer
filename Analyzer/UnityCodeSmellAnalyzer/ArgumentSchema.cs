using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ArgumentSchema : SyntaxSchema
    {
        protected string argument;
        protected bool isInvocation = false;

        public ArgumentSchema() { }

        public string Argument { get { return argument; } }
        public bool IsInvocation { get { return isInvocation; } }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            ArgumentSyntax arg = root as ArgumentSyntax;
            if (model.GetSymbolInfo(arg.Expression).Symbol is IMethodSymbol) isInvocation = true;
            argument = arg.Expression.ToString();
        }

    }
}



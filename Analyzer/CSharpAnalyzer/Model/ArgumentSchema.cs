using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing the Argument of an Invocation. 
    /// Informations gathered: Argument Expression, Is Invocation, Is Literal, LOC
    /// </summary>
    [Serializable]
    public class ArgumentSchema : SyntaxSchema
    {
        
        protected string argument;
        protected bool isInvocation = false;
        protected bool isLiteral = false;

        public ArgumentSchema() { }

        [JsonIgnore]
        public override int Line { get { return line; } }
        public string Argument { get { return argument; } }
        public bool IsInvocation { get { return isInvocation; } }
        public bool IsLiteral { get { return isLiteral; } }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ArgumentSyntax arg = root as ArgumentSyntax;
            if (model.GetSymbolInfo(arg.Expression).Symbol is IMethodSymbol) isInvocation = true;
            if (model.GetSymbolInfo(arg.Expression).Symbol is ILiteralOperation) isLiteral = true;
            argument = arg.Expression.ToString();
            
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }

    }
}



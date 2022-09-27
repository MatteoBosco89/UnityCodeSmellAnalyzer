using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InvocationSchema
    {
        protected int line;
        protected string name;
        protected string fullName;
        protected string returnType;
        protected List<ArgumentSchema> arguments = new List<ArgumentSchema>();

        public int Line { get { return line; } }
        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public string ReturnType { get { return returnType; } }

        public List<ArgumentSchema> Arguments { get { return arguments; } }

        public InvocationSchema(int line, string name, string fullName, string returnType)
        {
            this.line = line;
            this.name = name;
            this.fullName = fullName;
            this.returnType = returnType;
        }

        public void AddArgument(ArgumentSchema a)
        {
            arguments.Add(a);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            InvocationExpressionSyntax invocation = root as InvocationExpressionSyntax;

            foreach (var arg in invocation.ArgumentList.Arguments)
            {
                bool isMeth = false;
                if (model.GetSymbolInfo(arg.Expression).Symbol is IMethodSymbol) isMeth = true;
                ArgumentSchema a = new ArgumentSchema(arg.Expression.ToString(), isMeth);
                AddArgument(a);
            }

        }

    }
}


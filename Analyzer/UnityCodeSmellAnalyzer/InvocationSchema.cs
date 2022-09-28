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
    public class InvocationSchema : SyntaxSchema
    {
        protected string name;
        protected string fullName;
        protected string returnType;
        protected string fileName;
        protected int definitionLine;
        protected string kind;
        protected string module;
        protected List<ArgumentSchema> arguments = new List<ArgumentSchema>();

        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public string ReturnType { get { return returnType; } }
        public string FileName { get { return fileName; } }
        public int DefinitionLine { get { return definitionLine + 1; } }
        public string Kind { get { return kind; } }
        public string Module { get { return module; } }
        public List<ArgumentSchema> Arguments { get { return arguments; } }

        public InvocationSchema() { }

        public void AddArgument(ArgumentSchema a)
        {
            arguments.Add(a);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            InvocationExpressionSyntax invocation = root as InvocationExpressionSyntax;

            line = invocation.GetLocation().GetLineSpan().StartLinePosition.Line;

            if (model.GetSymbolInfo(invocation).Symbol != null)
            {
                name = model.GetSymbolInfo(invocation).Symbol.MetadataName;
                fullName = model.GetSymbolInfo(invocation).Symbol.ToString();
                returnType = model.GetTypeInfo(invocation).Type.Name;
                kind = model.GetSymbolInfo(invocation).Symbol.Locations.First().Kind.ToString();
                definitionLine = model.GetSymbolInfo(invocation).Symbol.Locations.First().GetLineSpan().StartLinePosition.Line;
                module = model.GetSymbolInfo(invocation).Symbol.Locations.First().MetadataModule?.ToString();
                var meth = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                fileName = meth.Locations.First().SourceTree?.FilePath;
            }


            foreach (var arg in invocation.ArgumentList.Arguments)
            { 
                ArgumentSchema a = new ArgumentSchema();
                LoadInformations(arg, model);
                AddArgument(a);
            }

        }

    }
}


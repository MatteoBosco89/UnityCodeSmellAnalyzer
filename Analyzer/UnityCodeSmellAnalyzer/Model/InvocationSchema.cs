using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Invocation Expression of a method. 
    /// Informations gathered: Name, Full Name (Namespace.Class), Return Type, Path, Definition LOC, Location Kind, 
    /// Module (if any), Arguments, LOC
    /// </summary>
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
        /// <summary>
        /// Loads all Arguments of the invocation
        /// </summary>
        /// <param name="invocation">The Invocation Expression</param>
        /// <param name="model">The model</param>
        protected void LoadArguments(InvocationExpressionSyntax invocation, SemanticModel model)
        {
            foreach (var arg in invocation.ArgumentList.Arguments)
            {
                ArgumentSchema a = new ArgumentSchema();
                a.LoadInformations(arg, model);
                AddArgument(a);
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            InvocationExpressionSyntax invocation = root as InvocationExpressionSyntax;
            LoadBasicInformations(root, model);
            LoadArguments(invocation, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
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
        }
    }
}


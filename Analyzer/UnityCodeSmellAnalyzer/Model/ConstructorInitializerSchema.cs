using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing the Class Constructor Initializer. Inherit SyntaxSchema.
    /// Exists when the class inerith from another class
    /// Informations gathered: List of Arguments for initialization, Base or This Keyword
    /// </summary>
    [Serializable]
    public class ConstructorInitializerSchema : SyntaxSchema
    {
        protected List<ArgumentSchema> arguments = new List<ArgumentSchema>();
        protected string thisOrBase;

        public List<ArgumentSchema> Arguments { get { return arguments; } }
        public string ThisOrBase { get { return thisOrBase; } }

        public ConstructorInitializerSchema() { }

        protected void AddArgument(ArgumentSchema a)
        {
            arguments.Add(a);
        }

        /// <summary>
        /// Loads all Arguments of the constructor initializer from the parent class
        /// </summary>
        /// <param name="constructor">The Constructor Initializer</param>
        /// <param name="model">The model</param>
        protected void LoadArguments(ConstructorInitializerSyntax constructor, SemanticModel model)
        {
            foreach (var arg in constructor.ArgumentList.Arguments)
            {
                ArgumentSchema a = new ArgumentSchema();
                a.LoadInformations(arg, model);
                AddArgument(a);
            }
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ConstructorInitializerSyntax cis = root as ConstructorInitializerSyntax;
            thisOrBase = cis.ThisOrBaseKeyword.ToString();
            LoadArguments(cis, model);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }
    }
}


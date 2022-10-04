using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Field Declaration. 
    /// Information gathered: Name, Type, Initial Value, Modifiers, Attributes, LOC
    /// </summary>
    [Serializable]
    public class FieldSchema : SyntaxSchema
    {

        protected string name;
        protected string fullName;
        protected string type;
        protected List<string> modifiers = new List<string>();
        protected string assignment;
        protected List<string> attributes = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public string Assignment { get { return assignment; } }
        public List<string> Attributes { get { return attributes; } }
        public string FullName { get { return fullName; } }

        public FieldSchema() { }

        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }
        /// <summary>
        /// Specialized LoadInformations method.
        /// </summary>
        /// <param name="root">The Field Declaration</param>
        /// <param name="v">The Variable Declaration</param>
        /// <param name="model">The model</param>
        public void LoadInformations(SyntaxNode root, SyntaxNode v, SemanticModel model)
        {
            FieldDeclarationSyntax f = root as FieldDeclarationSyntax;
            VariableDeclaratorSyntax fds = v as VariableDeclaratorSyntax;

            name = fds.Identifier.ToString();
            fullName = model.GetDeclaredSymbol(fds).ToString();
            line = fds.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = f.Declaration.Type.ToString();
            if (fds.Initializer != null) assignment = fds.Initializer.Value.ToString();
            LoadModifiers(f);
            LoadAttributes(f);
        }
        /// <summary>
        /// Loads all Modifiers of the Field
        /// </summary>
        /// <param name="f">The Field Declaration</param>
        protected void LoadModifiers(FieldDeclarationSyntax f)
        {
            foreach (var m in f.Modifiers) AddModifier(m.ToString());
        }
        /// <summary>
        /// Loads all Attributes of the Field
        /// </summary>
        /// <param name="f">The Field Declaration</param>
        protected void LoadAttributes(FieldDeclarationSyntax f)
        {
            foreach (var attr in f.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.ToString());
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model) { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model) { }
    }
}


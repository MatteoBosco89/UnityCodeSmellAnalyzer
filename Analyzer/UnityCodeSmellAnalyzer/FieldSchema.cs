using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class FieldSchema : SyntaxSchema
    {

        protected string name;
        protected string type;
        protected List<string> modifiers = new List<string>();
        protected string assignment;
        protected List<string> attributes = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public string Assignment { get { return assignment; } }
        public List<string> Attributes { get { return attributes; } }

        public FieldSchema() { }

        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }

        public void LoadInformations(SyntaxNode root, SyntaxNode v, SemanticModel model)
        {
            FieldDeclarationSyntax f = root as FieldDeclarationSyntax;
            VariableDeclaratorSyntax fds = v as VariableDeclaratorSyntax;
            
            name = fds.Identifier.ToString();
            line = fds.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = f.Declaration.Type.ToString();
            if (fds.Initializer != null) assignment = fds.Initializer.Value.ToString();
            
            foreach (var m in f.Modifiers) AddModifier(m.ToString());
            foreach (var a in f.AttributeLists) AddAttribute(a.ToString());
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model) { }
    }
}


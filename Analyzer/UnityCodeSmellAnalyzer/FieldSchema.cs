using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class FieldSchema
    {

        protected string name;
        protected string type;
        protected List<string> modifiers = new List<string>();
        protected string assignment;
        protected int line;
        protected List<string> attributes = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public string Assignment { get { return assignment; } }
        public int Line { get { return line; } }
        public List<string> Attributes { get { return attributes; } }

        public FieldSchema(string name, string type, string assignment, int line)
        {
            this.name = name;
            this.type = type;
            this.assignment = assignment;
            this.line = line;
        }

        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            FieldDeclarationSyntax f = root as FieldDeclarationSyntax;
            foreach (var m in f.Modifiers) AddModifier(m.ToString());
            foreach (var a in f.AttributeLists) AddAttribute(a.ToString());
        }
    }
}


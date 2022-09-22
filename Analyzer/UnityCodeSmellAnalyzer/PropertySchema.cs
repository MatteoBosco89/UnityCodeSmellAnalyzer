using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class PropertySchema
    {
        protected string name;
        protected string type;
        protected List<string> modifiers;
        protected int line;

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public int Line { get { return line; } }

        public PropertySchema(string name, string type, int line)
        {
            this.name = name;
            this.type = type;
            this.line = line;
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            foreach(var mod in prop.Modifiers)
            {
                AddModifier(mod.ValueText);
            }
        }

    }
}


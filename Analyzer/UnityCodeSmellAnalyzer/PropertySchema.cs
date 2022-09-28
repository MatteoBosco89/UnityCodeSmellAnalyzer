using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class PropertySchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected List<string> modifiers = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }

        public PropertySchema() { }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            name = prop.Identifier.ToString();
            type = prop.Type.ToString();
            line = prop.GetLocation().GetLineSpan().StartLinePosition.Line;

            foreach (var mod in prop.Modifiers)
            {
                AddModifier(mod.ToString());
            }
        }

    }
}


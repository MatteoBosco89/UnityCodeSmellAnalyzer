using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Property Declaration. 
    /// Informations gathered: Name, Type, Modifiers, LOC
    /// </summary>
    [Serializable]
    public class PropertySchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected List<string> modifiers = new List<string>();
        protected List<string> attributes = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public List<string> Attributes { get { return attributes; } }

        public PropertySchema() { }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }
        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

        public void LoadAttribute(PropertyDeclarationSyntax prop)
        {
            foreach (var attr in prop.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }

        public void LoadModifier(PropertyDeclarationSyntax prop)
        {
            foreach (var mod in prop.Modifiers)
            {
                AddModifier(mod.ToString());
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            LoadBasicInformations(root, model);
            LoadAttribute(prop);
            LoadModifier(prop);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            name = prop.Identifier.ToString();
            type = prop.Type.ToString();
            line = prop.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}


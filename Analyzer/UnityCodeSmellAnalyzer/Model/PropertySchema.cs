using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

//// POSSONO ESSERCI VARIABILI E INVOCAZIONI

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Property Declaration. 
    /// Informations gathered: Name, Type, Modifiers, LOC
    /// </summary>
    /// TODO MIGLIORARE (VEDI LOAD INFORMATIONS)
    [Serializable]
    public class PropertySchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected bool isAutoProperty = false;
        protected bool initializerValue;
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
            // PRENDERE ANCHE GET E SET, VEDERE SE SONO VUOTI (ABSTRACT) O NO
            //Console.WriteLine(prop.Initializer?.Value);
            //Console.WriteLine(prop.ExplicitInterfaceSpecifier);
            //Console.WriteLine(prop.ExpressionBody);
            //Console.WriteLine(prop.AccessorList);
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


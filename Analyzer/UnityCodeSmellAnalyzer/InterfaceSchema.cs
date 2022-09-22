using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InterfaceSchema
    {
        protected List<MethodSchema> methods = new List<MethodSchema>();
        protected List<PropertySchema> properties = new List<PropertySchema>();
        protected List<string> attributes = new List<string>();
        protected string name;
        protected int line;
        protected List<string> modifiers = new List<string>();

        public string Name { get { return name; } }
        public int Line { get { return line; } }
        public List<string> Modifiers { get { return modifiers; } }
        public List<MethodSchema> Methods { get { return methods; } }
        public List<PropertySchema> Properties { get { return properties; } }
        public List<string> Attributes { get { return attributes; } }

        public InterfaceSchema(string name, int line)
        {
            this.name = name;
            this.line = line;
        }
        public void AddMethod(MethodSchema m)
        {
            methods.Add(m);
        }
        public void AddProperty(PropertySchema p)
        {
            properties.Add(p);
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
            InterfaceDeclarationSyntax r = root as InterfaceDeclarationSyntax;
            List<MethodDeclarationSyntax> mdsl = (from meth in r.DescendantNodes().OfType<MethodDeclarationSyntax>() select meth).ToList();
            List<PropertyDeclarationSyntax> pdsl = (from prop in r.DescendantNodes().OfType<PropertyDeclarationSyntax>() select prop).ToList();

            foreach(var attrList in r.AttributeLists)
            {
                foreach (var attr in attrList.Attributes) AddAttribute(attr.Name.ToString());
            }

            foreach(var mod in r.Modifiers)
            {
                AddModifier(mod.ValueText);
            }

            foreach(MethodDeclarationSyntax m in mdsl)
            {
                MethodSchema method = new MethodSchema(m.Identifier.ToString(), m.GetLocation().GetLineSpan().StartLinePosition.Line);
                method.LoadInformations(m, model);
                AddMethod(method);
            }

            foreach(PropertyDeclarationSyntax p in pdsl)
            {
                PropertySchema property = new PropertySchema(p.Identifier.ToString(), p.Type.ToString(), p.GetLocation().GetLineSpan().StartLinePosition.Line);
                property.LoadInformations(p, model);
                AddProperty(property);
            }

        }

    }
}



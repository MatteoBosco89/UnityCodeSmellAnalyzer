﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InterfaceSchema : SyntaxSchema
    {
        protected List<MethodSchema> methods = new List<MethodSchema>();
        protected List<PropertySchema> properties = new List<PropertySchema>();
        protected List<string> attributes = new List<string>();
        protected string name;
        protected List<string> modifiers = new List<string>();

        public string Name { get { return name; } }
        public List<string> Modifiers { get { return modifiers; } }
        public List<MethodSchema> Methods { get { return methods; } }
        public List<PropertySchema> Properties { get { return properties; } }
        public List<string> Attributes { get { return attributes; } }

        public InterfaceSchema() { }
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

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {

            InterfaceDeclarationSyntax r = root as InterfaceDeclarationSyntax;

            name = r.Identifier.ToString();
            line = r.GetLocation().GetLineSpan().StartLinePosition.Line;

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
                MethodSchema method = new MethodSchema();
                method.LoadInformations(m, model);
                AddMethod(method);
            }

            foreach(PropertyDeclarationSyntax p in pdsl)
            {
                PropertySchema property = new PropertySchema();
                property.LoadInformations(p, model);
                AddProperty(property);
            }

        }

    }
}



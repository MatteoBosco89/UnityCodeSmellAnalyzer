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
        protected bool isAutoProperty = true;
        protected bool initializer;
        protected bool hasExpressionBody = false;
        protected ConditionSchema expressionBody;
        protected ConditionSchema initializerValue;
        protected List<string> modifiers = new List<string>();
        protected List<string> attributes = new List<string>();
        protected List<AccessorSchema> accessors = new List<AccessorSchema>();

        public string Name { get { return name; } set { name = value; } }
        public string Type { get { return type; } }
        public List<string> Modifiers { get { return modifiers; } }
        public List<string> Attributes { get { return attributes; } }
        public bool IsAutoProperty { get { return isAutoProperty; } }
        public bool Initializer { get { return initializer; } }
        public ConditionSchema InitializerValue { get { return initializerValue; } }
        public List<AccessorSchema> Accessor { get { return accessors; } }
        public bool HasExpressionBody { get { return hasExpressionBody; } }
        public ConditionSchema ExpressionBody { get { return expressionBody; } }

        public PropertySchema() { }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }
        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }
        protected void AddAccessor(AccessorSchema a)
        {
            accessors.Add(a);
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

        /// <summary>
        /// Loads the Property's Accessors (get, set)
        /// </summary>
        /// <param name="root">The Property</param>
        /// <param name="model">The model</param>
        protected void LoadAccessors(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            foreach (var accessor in prop.AccessorList.Accessors)
            {
                AccessorSchema acc = new AccessorSchema();
                acc.LoadInformations(accessor, model);
                AddAccessor(acc);
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            LoadBasicInformations(root, model);
            LoadAttribute(prop);
            LoadModifier(prop);
            if (prop.ExpressionBody == null) LoadAccessors(prop, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            
            PropertyDeclarationSyntax prop = root as PropertyDeclarationSyntax;
            name = prop.Identifier.ToString();
            type = prop.Type.ToString();
            line = prop.GetLocation().GetLineSpan().StartLinePosition.Line;
            if(prop.Initializer?.Value != null)
            {
                initializer = true;
                initializerValue = new ConditionSchema();
                initializerValue.LoadInformations(prop.Initializer.Value, model);
            }
            if(prop.ExpressionBody != null)
            {
                hasExpressionBody = true;
                isAutoProperty = false;
                expressionBody = new ConditionSchema();
                expressionBody.LoadInformations(prop.ExpressionBody.Expression, model);
            }
            if (prop.AccessorList != null)
            {
                foreach (var accessors in prop.AccessorList.Accessors)
                {
                    if (accessors.Body != null || accessors.ExpressionBody != null) isAutoProperty = false;
                }
            }
            
        }
    }
}


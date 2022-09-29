using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Prameter of the Method Declaration. 
    /// Informations gathered: Name, Type, Default Value, Attributes, LOC
    /// </summary>
    [Serializable]
    public class ParameterSchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected string assignment;
        protected List<string> attributes = new List<string>();

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }
        public List<string> Attributes { get { return attributes; } }

        [JsonIgnore]
        public override int Line { get { return line; } }

        public ParameterSchema() { }

        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }
        /// <summary>
        /// Parameters attributes (SerializeField, ...)
        /// </summary>
        protected void LoadAttribute(ParameterSyntax param)
        {
            foreach (var attr in param.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ParameterSyntax param = root as ParameterSyntax;
            name = param.Identifier.ToString();
            type = param.Type.ToString();
            if (param.Default != null) assignment = param.Default.Value.ToString();
            line = param.GetLocation().GetLineSpan().StartLinePosition.Line;
            LoadAttribute(param);
        }
    }
}


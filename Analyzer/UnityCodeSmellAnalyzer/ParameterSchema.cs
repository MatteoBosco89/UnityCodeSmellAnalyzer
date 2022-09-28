using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ParameterSchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected string assignment;

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }


        public ParameterSchema() { }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            ParameterSyntax param = root as ParameterSyntax;
            name = param.Identifier.ToString();
            type = param.Type.ToString();
            if (param.Default != null) assignment = param.Default.Value.ToString();
            line = param.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}


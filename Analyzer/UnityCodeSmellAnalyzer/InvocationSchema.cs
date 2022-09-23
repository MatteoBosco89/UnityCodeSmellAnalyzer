using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InvocationSchema
    {
        protected int line;
        protected string name;
        protected string fullName;
        protected string returnType;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();

        public int Line { get { return line; } }
        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public string ReturnType { get { return returnType; } }

        public List<ParameterSchema> Parameters { get { return parameters; } }

        public InvocationSchema(int line, string name, string fullName, string returnType)
        {
            this.line = line;
            this.name = name;
            this.fullName = fullName;
            this.returnType = returnType;
        }

        public void AddParameter(ParameterSchema p)
        {
            parameters.Add(p);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            InvocationExpressionSyntax invocation = root as InvocationExpressionSyntax;

            var sym = (model.GetSymbolInfo(invocation).Symbol) as IMethodSymbol;
            if(sym != null && sym.Parameters != null && sym.Parameters.Length > 0)
            {
                foreach (var param in sym.Parameters)
                {
                    ParameterSchema p = new ParameterSchema(param.Name, param.Type.ToString(), (param.HasExplicitDefaultValue ? param.ExplicitDefaultValue.ToString() : null));
                    AddParameter(p);
                }
            }
            
        }

    }
}


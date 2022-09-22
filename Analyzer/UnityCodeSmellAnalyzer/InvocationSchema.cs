using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InvocationSchema
    {
        protected int line;
        protected string name;
        protected string fullName;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();

        public int Line { get { return line; } }
        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }

        public InvocationSchema(int line, string name, string fullName)
        {
            this.line = line;
            this.name = name;
            this.fullName = fullName;
        }

        public void AddParameter(ParameterSchema p)
        {
            parameters.Add(p);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            InvocationExpressionSyntax invocation = root as InvocationExpressionSyntax;
            /*
            var sym = model.GetTypeInfo(invocation);
            sym.Type;

            foreach (var param in invocation.ArgumentList.Arguments)
            {


                param.
                string def = null;
                if (param.Default != null) def = param.Default.Value.ToString();
                ParameterSchema parameter = new ParameterSchema(param.Identifier.ToString(), param.Type.ToString(), def);
                AddParameter(parameter);
            }*/
        }

    }
}


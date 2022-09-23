using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class MethodSchema
    {
        protected string name;
        protected List<string> modifiers = new List<string>();
        protected int line;
        protected string returnType;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();
        protected List<InvocationSchema> invocations = new List<InvocationSchema>();
        protected List<VariableSchema> variables = new List<VariableSchema>();
        protected List<StatementSchema> statements = new List<StatementSchema>();


        public string Name { get { return name; } }
        public List<string> Modifiers { get { return modifiers; } }
        public int Line { get { return line; } }
        public string ReturnType { get { return returnType; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<StatementSchema> Statements { get { return statements; } }

        public MethodSchema(string name, int line, string returnType)
        {
            this.name = name;
            this.line = line;
            this.returnType = returnType;
        }

        public void AddParameter(ParameterSchema p)
        {
            parameters.Add(p);
        }

        public void AddInvocation(InvocationSchema i)
        {
            invocations.Add(i);
        }

        public void AddVariable(VariableSchema v)
        {
            variables.Add(v);
        }

        public void AddStatement(StatementSchema s)
        {
            statements.Add(s);
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            MethodDeclarationSyntax method = root as MethodDeclarationSyntax;
            List<InvocationExpressionSyntax> idsl = (from invoc in method.DescendantNodes().OfType<InvocationExpressionSyntax>() select invoc).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in method.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<StatementSyntax> ssl = (from stat in method.DescendantNodes().OfType<StatementSyntax>() select stat).ToList();

            if (AnalyzerConfiguration.StatementVerbose)
            {
                foreach (var s in ssl)
                {
                    AddStatement(new StatementSchema(s.ToString(), s.GetLocation().GetLineSpan().StartLinePosition.Line));
                }
            }

            foreach (var param in method.ParameterList.Parameters)
            {
                string def = null;
                if (param.Default != null) def = param.Default.Value.ToString();
                ParameterSchema parameter = new ParameterSchema(param.Identifier.ToString(), param.Type.ToString(), def);
                AddParameter(parameter);
            }

            foreach (var i in idsl) {
                string name = null;
                string fullName = null;
                string returnType = null;
                if(model.GetSymbolInfo(i).Symbol != null)
                {
                    name = model.GetSymbolInfo(i).Symbol.MetadataName;
                    fullName = model.GetSymbolInfo(i).Symbol.ToString();
                    returnType = model.GetTypeInfo(i).Type.Name;
                }
                InvocationSchema invoc = new InvocationSchema(i.GetLocation().GetLineSpan().StartLinePosition.Line, name, fullName, returnType);
                invoc.LoadInformations(i, model);
                AddInvocation(invoc);
            }

            foreach(var v in vdsl)
            {
                foreach(var vv in v.Variables)
                {
                    string initializer = null;
                    if(vv.Initializer != null) initializer = vv.Initializer.Value.ToString();
                    VariableSchema variable = new VariableSchema(vv.Identifier.ToString(), v.Type.ToString(), initializer, vv.GetLocation().GetLineSpan().StartLinePosition.Line);
                    AddVariable(variable);
                }
                
            }

            foreach (var mod in method.Modifiers)
            {
                AddModifier(mod.ValueText);
            }

        }

    }
}


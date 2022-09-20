using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class MethodSchema
    {
        protected string name;
        protected string modifier;
        protected int line;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();
        protected List<InvocationSchema> invocations = new List<InvocationSchema>();
        protected List<VariableSchema> variables = new List<VariableSchema>();
        protected List<StatementSchema> statements = new List<StatementSchema>();


        public MethodSchema(string name, string modifier, int line)
        {
            this.name = name;
            this.modifier = modifier;
            this.line = line;
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

    }
}


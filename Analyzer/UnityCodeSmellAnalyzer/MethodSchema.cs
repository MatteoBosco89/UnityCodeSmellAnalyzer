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


        public string Name { get { return name; } }
        public string Modifier { get { return modifier; } }
        public int Line { get { return line; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<StatementSchema> Statements { get { return statements; } }

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


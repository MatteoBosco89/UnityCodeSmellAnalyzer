using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InvocationSchema
    {
        protected int line;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();

        public int Line { get { return line; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }

        public InvocationSchema(int line)
        {
            this.line = line;
        }

        public void AddParameter(ParameterSchema p)
        {
            parameters.Add(p);
        }
    }
}


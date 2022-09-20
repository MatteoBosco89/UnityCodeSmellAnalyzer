using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InvocationSchema
    {
        protected int line;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();
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


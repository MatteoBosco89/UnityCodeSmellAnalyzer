using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ParameterSchema
    {
        protected string name;
        protected string type;
        protected string assignment;

        public ParameterSchema(string name, string type, string assignment)
        {
            this.name = name;
            this.type = type;
            this.assignment = assignment;
        }
    }
}


using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class VariableSchema
    {
        protected string name;
        protected string type;
        protected string assignment;
        protected int line;

        public VariableSchema(string name, string type, string assignment, int line)
        {
            this.name = name;
            this.type = type;
            this.assignment = assignment;
            this.line = line;
        }
    }
}


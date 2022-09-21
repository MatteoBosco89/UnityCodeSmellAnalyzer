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

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }
        public int Line { get { return line; } }

        public VariableSchema(string name, string type, string assignment, int line)
        {
            this.name = name;
            this.type = type;
            this.assignment = assignment;
            this.line = line;
        }
    }
}


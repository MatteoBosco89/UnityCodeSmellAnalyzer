using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ParameterSchema
    {
        protected string name;
        protected string type;
        protected string assignment;

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }

        public ParameterSchema(string name, string type, string assignment)
        {
            this.name = name;
            this.type = type;
            this.assignment = assignment;
        }
    }
}


using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class FieldSchema
    {

        protected string name;
        protected string type;
        protected string modifier;
        protected string assignment;
        protected int line;
        protected List<string> attributes = new List<string>();

        public FieldSchema(string name, string type, string modifier, string assignment, int line)
        {
            this.name = name;
            this.type = type;
            this.modifier = modifier;
            this.assignment = assignment;
            this.line = line;
        }

        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }
    }
}


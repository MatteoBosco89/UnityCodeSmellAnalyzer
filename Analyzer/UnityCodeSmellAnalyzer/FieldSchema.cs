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

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Modifier { get { return modifier; } }
        public string Assignment { get { return assignment; } }
        public int Line { get { return line; } }
        public List<string> Attributes { get { return attributes; } }

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


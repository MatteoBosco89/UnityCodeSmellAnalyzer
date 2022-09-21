using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class PropertySchema
    {
        protected string name;
        protected string type;
        protected string modifier;
        protected int line;

        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Modifier { get { return modifier; } }
        public int Line { get { return line; } }

        public PropertySchema(string name, string type, string modifier, int line)
        {
            this.name = name;
            this.type = type;
            this.modifier = modifier;
            this.line = line;
        }
    }
}


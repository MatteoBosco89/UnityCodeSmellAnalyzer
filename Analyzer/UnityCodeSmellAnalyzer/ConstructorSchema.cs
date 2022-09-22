using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ConstructorSchema : MethodSchema
    {
        protected string classRef;

        public string ClassRef { get { return classRef; } }

        public ConstructorSchema(string name, int line) : base(name, line)
        {
            classRef = name;
        }
    }
}


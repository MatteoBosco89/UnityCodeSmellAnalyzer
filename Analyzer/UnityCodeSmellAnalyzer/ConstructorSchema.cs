using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ConstructorSchema : MethodSchema
    {
        protected string classRef;

        public ConstructorSchema(string name, string modifier, int line) : base(name, modifier, line)
        {
            classRef = name;
        }
    }
}


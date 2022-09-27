using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class ArgumentSchema
    {
        protected string argument;
        protected bool isInvocation;

        public ArgumentSchema(string argument, bool isInvocation)
        {
            this.argument = argument;
            this.isInvocation = isInvocation;
        }

        public string Argument { get { return argument; } }
        public bool IsInvocation { get { return isInvocation; } }   

    }
}



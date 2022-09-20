using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class StatementSchema
    {
        protected string statement;
        protected int line;
        public StatementSchema(string statement, int line)
        {
            this.statement = statement;
            this.line = line;
        }
    }
}


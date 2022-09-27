using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class CycleOrControlSchema
    {
        protected int line;
        protected int depth;

        public CycleOrControlSchema(int line, int depth)
        {
            this.line = line;
            this.depth = depth;
        }
        public int Line { get { return line; } }
        public int Depth { get { return depth; } }

        public virtual void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            
        }

    }
}


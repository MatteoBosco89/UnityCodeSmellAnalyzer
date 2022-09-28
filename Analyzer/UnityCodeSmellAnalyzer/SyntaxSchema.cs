using Microsoft.CodeAnalysis;
using System;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public abstract class SyntaxSchema
    {

        protected int line;

        public int Line { get { return line + 1; } }


        public abstract void LoadInformations(SyntaxNode root, SemanticModel model);
    }
}


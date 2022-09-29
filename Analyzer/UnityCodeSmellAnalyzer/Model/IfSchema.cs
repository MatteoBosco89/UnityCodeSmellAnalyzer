using Microsoft.CodeAnalysis;
using System;

namespace UnityCodeSmellAnalyzer
{
    public class IfSchema : CycleOrControlSchema
    {
        public IfSchema()
        {
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            throw new NotImplementedException();
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            throw new NotImplementedException();
        }
    }
}


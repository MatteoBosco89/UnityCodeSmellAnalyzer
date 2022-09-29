using Microsoft.CodeAnalysis;
using System;

namespace UnityCodeSmellAnalyzer
{
    public class SwitchCaseSchema : CycleOrControlSchema
    {
        public SwitchCaseSchema()
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


using Microsoft.CodeAnalysis;
using System;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Class Constructor. Inherit MethodSchema
    /// Informations gathered: Reference Class, Return Type, Name, LOC
    /// </summary>
    [Serializable]
    public class ConstructorSchema : MethodSchema
    {
        protected string classRef;

        public string ClassRef { get { return classRef; } }

        public ConstructorSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            base.LoadBasicInformations(root, model);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            base.LoadInformations(root, model);
        }
    }
}


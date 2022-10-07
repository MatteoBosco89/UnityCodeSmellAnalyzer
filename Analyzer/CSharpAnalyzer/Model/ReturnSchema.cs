using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing a Return Statement Block. Inherit SyntaxSchema.
    /// Informations gathered: Return Statement, Return Kind 
    /// </summary>
    [Serializable]
    public class ReturnSchema : SyntaxSchema
    {
        protected ConditionSchema returnStatement;
        protected string returnType = "void";


        public ConditionSchema ReturnStatement { get { return returnStatement; } }
        public string ReturnType { get { return returnType; } }

        public ReturnSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ReturnStatementSyntax ret = root as ReturnStatementSyntax;
            line = ret.GetLocation().GetLineSpan().StartLinePosition.Line;
            if(ret.Expression != null)
            {
                returnStatement = new ConditionSchema();
                returnStatement.LoadBasicInformations(ret.Expression, model);
                returnType = model.GetTypeInfo(ret.Expression).Type?.ToString();
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }
    }
}


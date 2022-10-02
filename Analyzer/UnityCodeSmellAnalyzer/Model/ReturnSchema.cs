using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing a Return Statement Block. Inherit SyntaxSchema.
    /// Informations gathered: Return Statement, Return Kind 
    /// </summary>
    /// TODO MIGLIORARE LISTA TOKEN DEL RETURN
    [Serializable]
    public class ReturnSchema : SyntaxSchema
    {
        protected string returnStatement = "return";
        protected string returnKind = "void";

        public string ReturnStatement { get { return returnStatement; } }
        public string ReturnKind { get { return returnKind; } }

        public ReturnSchema() { }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ReturnStatementSyntax ret = root as ReturnStatementSyntax;
            line = ret.GetLocation().GetLineSpan().StartLinePosition.Line;
            if(ret.Expression != null)
            {
                returnStatement = ret.Expression.ToString();
                returnKind = ret.Expression.Kind().ToString();
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
        }
    }
}


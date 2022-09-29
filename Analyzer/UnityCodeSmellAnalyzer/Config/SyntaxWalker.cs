using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    public static class SyntaxWalker
    {
        private static readonly List<Type> invocationAncestors = new List<Type> { typeof(MethodDeclarationSyntax), typeof(WhileStatementSyntax), typeof(ForEachStatementSyntax), typeof(ForStatementSyntax), typeof(IfStatementSyntax), typeof(ElseClauseSyntax), typeof(SwitchExpressionSyntax) };
        private static readonly List<Type> variablesAncestors = new List<Type> { typeof(MethodDeclarationSyntax), typeof(WhileStatementSyntax), typeof(ForEachStatementSyntax), typeof(ForStatementSyntax), typeof(IfStatementSyntax), typeof(ElseClauseSyntax), typeof(SwitchExpressionSyntax) };

        public static List<Type> InvocationAncestors { get { return invocationAncestors; } }
        public static List<Type> VariablesAncestors { get { return variablesAncestors; } }

        /// <summary>
        /// Search for the first parent of those included in the Type list
        /// </summary>
        /// <param name="syntaxNode">The SyntaxNode</param>
        /// <param name="types">List of types to search</param>
        /// <returns>The first parent found, can't be null</returns>
        public static SyntaxNode SearchParent(SyntaxNode syntaxNode, List<Type> types)
        {
            if (types.Contains(syntaxNode.Parent.GetType())) return syntaxNode.Parent;
            return SearchParent(syntaxNode.Parent, types);
        }
    }
}


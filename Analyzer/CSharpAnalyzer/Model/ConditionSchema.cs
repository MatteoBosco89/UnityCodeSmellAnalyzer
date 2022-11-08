using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing a Condition Statement Block. Inherit SyntaxSchema.
    /// Informations gathered: Condition Statement, Condition Type, List of Condition Tokens 
    /// </summary>
    [Serializable]
    public class ConditionSchema : SyntaxSchema
    {
        protected static readonly string[] operators = new string[] { "&&", "||", "!", "<", "<=", "<<", ">", ">=", ">>", "=", "==", "(", ")" };
        protected readonly string literal = "Literal";
        protected readonly string invocation = "InvocationExpression";
        protected readonly string property = "Property";
        protected string conditionStatement;
        protected string type;
        protected string conditionType;
        protected string conditionKind;
        protected List<string> conditionTokens = new List<string>();
        protected List<InvocationSchema> invocations = new List<InvocationSchema>();
        protected List<string> properties = new List<string>();
        protected List<string> fields = new List<string>();
        public List<string> Tokens { get { return conditionTokens; } }
        public List<string> Properties { get { return properties; } }
        public List<string> Fields { get { return fields; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public string Type { get { return type; } }
        public string Statement { get { return conditionStatement; } }
        public string ExpressionType { get { return conditionType; } }
        public string Kind { get { return conditionKind; } }
        [JsonIgnore]
        public override int Line { get { return line; } }
        public ConditionSchema() { }

        public void AddToken(string t)
        {
            conditionTokens.Add(t);
        }
        public void AddField(string s)
        {
            fields.Add(s);
        }
        public void AddProperty(string p)
        {
            properties.Add(p);
        }
        public void AddInvocation(InvocationSchema i)
        {
            invocations.Add(i);
        }
       

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            if (root == null) return;
            LoadBasicInformations(root, model);
            foreach (string t in Normalize(root.ToString()))
            {
                if(t.Trim() != "") AddToken(t.Trim());
            }
        }
        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            List<InvocationExpressionSyntax> inv = (from i in root.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>() select i).ToList();
            List<IMemberReferenceOperation> mrol = model.GetOperation(root).DescendantsAndSelf().OfType<IMemberReferenceOperation>().ToList();
            
            LoadInvocations(inv, model);
            LoadMemberAccess(mrol, model);
            
            conditionStatement = root.ToString();
            conditionType = root.Kind().ToString(); ;
            conditionKind = model.GetSymbolInfo(root).Symbol?.Kind.ToString();
            if (conditionKind == null) conditionKind = literal;
            type = model.GetTypeInfo(root).Type?.ToString();
            
        }
        /// <summary>
        /// Remove operands and create the token list
        /// </summary>
        /// <param name="condition">The condition string</param>
        /// <returns>The normalized condition string</returns>
        public List<string> Normalize(string condition)
        {
            foreach (var c in operators)
            {
                condition = condition.Replace(c, string.Empty);
            }
            return condition.Split(' ').ToList();
        }
        /// <summary>
        /// Loads all invocations
        /// </summary>
        /// <param name="inv">List of invocations</param>
        /// <param name="model">Model</param>
        protected void LoadInvocations(List<InvocationExpressionSyntax> inv, SemanticModel model)
        {
            foreach (InvocationExpressionSyntax item in inv)
            {
                InvocationSchema s = new InvocationSchema();
                s.LoadInformations(item, model);
                AddInvocation(s);
            }
        }
        /// <summary>
        /// Loads all member accesses (property or field)
        /// </summary>
        /// <param name="mrol">List of Members</param>
        /// <param name="model">The model</param>
        protected void LoadMemberAccess(List<IMemberReferenceOperation> mrol, SemanticModel model)
        {
            foreach (var i in mrol)
            {
                if (i.Member.Kind == SymbolKind.Property) AddProperty(i.Member.ToString());
                if (i.Member.Kind == SymbolKind.Field) AddField(i.Member.ToString());
            }
        }
        
    }
}


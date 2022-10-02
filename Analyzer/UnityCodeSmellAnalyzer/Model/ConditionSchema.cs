using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing a Condition Statement Block. Inherit SyntaxSchema.
    /// Informations gathered: Condition Statement, Condition Type, List of Condition Tokens 
    /// </summary>
    [Serializable]
    public class ConditionSchema : SyntaxSchema
    {
        protected static readonly string[] operators = new string[] { "&&", "||", "!", "<", "<=", "<<", ">", ">=", ">>", "=", "==" };
        protected readonly string literal = "Literal";
        protected string conditionStatement;
        protected string type;
        protected string conditionType;
        protected string conditionKind;
        protected List<string> conditionTokens = new List<string>();
        
        public List<string> ConditionTokens { get { return conditionTokens; } }
        public string Type { get { return type; } }
        public string ConditionStatement { get { return conditionStatement; } }
        public string ConditionType { get { return conditionType; } }
        public string ConditionKind { get { return conditionKind; } }
        [JsonIgnore]
        public override int Line { get { return line; } }
        public ConditionSchema() { }

        public void AddToken(string t)
        {
            conditionTokens.Add(t);
        }
        
        

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
            foreach (string t in Normalize(root.ToString()))
            {
                if(t.Trim() != "") AddToken(t.Trim());
            }
        }
        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            conditionStatement = root.ToString();
            conditionKind = model.GetSymbolInfo(root).Symbol?.Kind.ToString();
            if (conditionKind == null) conditionKind = literal;
            type = model.GetTypeInfo(root).Type.ToString();
            conditionType = root.Kind().ToString();
        }

        public List<string> Normalize(string condition)
        {
            foreach (var c in operators)
            {
                condition = condition.Replace(c, string.Empty);
            }
            return condition.Split(' ').ToList();
        }

    }
}


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class SwitchSchema : CycleOrControlSchema
    {
        protected ConditionSchema condition;
        protected List<CaseSchema> caseBranches = new List<CaseSchema>();
        public ConditionSchema Condition { get { return condition; } }
        public List<CaseSchema> CaseBranches { get { return caseBranches; } }
        [JsonIgnore]
        public override int Line { get { return line; } }
        public SwitchSchema() { }
        protected void AddCase(CaseSchema c)
        {
            caseBranches.Add(c);
        }

        /// <summary>
        /// Loads all the Case Branches from the Switch Structure
        /// </summary>
        /// <param name="switchStatement">Switch Statement</param>
        /// <param name="model">The model</param>
        protected void LoadCaseBranches(SwitchStatementSyntax switchStatement, SemanticModel model)
        {
            foreach (var section in switchStatement.Sections)
            {
                CaseSchema caseBranch = new CaseSchema();
                caseBranch.LoadInformations(section, model);
                AddCase(caseBranch);
            }
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            SwitchStatementSyntax switchStatement = root as SwitchStatementSyntax;
            condition = new ConditionSchema();
            condition.LoadInformations(switchStatement.Expression, model);
            startLine = switchStatement.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = switchStatement.GetLocation().GetLineSpan().EndLinePosition.Line;
            LoadCaseBranches(switchStatement, model);
        }

        
    }
}


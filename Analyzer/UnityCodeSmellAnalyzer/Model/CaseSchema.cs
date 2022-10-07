using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyzer
{
    [Serializable]
    public class CaseSchema : CycleOrControlSchema
    {
        public List<string> labels = new List<string>();
        public CaseSchema() { }

        [JsonIgnore]
        public override int Line { get { return line; } }
        [JsonIgnore]
        public override int Depth { get { return depth; } }

        protected void AddLabel(string l)
        {
            labels.Add(l);
        }

        /// <summary>
        /// Loads all the Labels of the Switch Section
        /// <param name="caseBranch">The Switch Section</param>
        /// </summary>
        protected void LoadLabels(SwitchSectionSyntax caseBranch)
        {
            foreach (var label in caseBranch.Labels) AddLabel(label.ToString());
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            SwitchSectionSyntax caseBranch = root as SwitchSectionSyntax;
            LoadLabels(caseBranch);
            startLine = caseBranch.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = caseBranch.GetLocation().GetLineSpan().EndLinePosition.Line;
        }

    }
}


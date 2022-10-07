using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Class representing a For Statement Block. Inherit CycleOrControlSchema.
    /// Informations gathered: Iterator, Iterator Type, Iterable, Iterable Type, Esxpression Is Invocation, 
    /// Expressione Is Property, Invocation, Property
    /// </summary>
    [Serializable]
    public class ForSchema : CycleOrControlSchema
    {
        protected ConditionSchema condition;
        protected List<string> declarations = new List<string>();
        protected List<string> initializers = new List<string>();
        protected List<string> incrementors = new List<string>();

        public ConditionSchema Condition { get { return condition; } }
        public List<string> Declarations { get { return declarations; } }
        public List<string> Initializers { get { return initializers; } }
        public List<string> Incrementors { get { return incrementors; } }

        [JsonIgnore]
        public override int Line { get { return line; } }
        public ForSchema() { }

        protected void AddDeclaration(string d)
        {
            declarations.Add(d);
        }
        protected void AddInitializer(string i)
        {
            initializers.Add(i);
        }
        protected void AddIncrementor(string i)
        {
            incrementors.Add(i);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ForStatementSyntax fs = root as ForStatementSyntax;
            condition = new ConditionSchema();
            condition.LoadInformations(fs.Condition, model);
            if(fs.Declaration != null) foreach (var v in fs.Declaration.Variables) AddDeclaration(v.Identifier.ToString());
            foreach (var i in fs.Initializers) AddInitializer(i.ToString());
            foreach (var i in fs.Incrementors) AddIncrementor(i.ToString());
            startLine = fs.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = fs.GetLocation().GetLineSpan().EndLinePosition.Line;
        }


    }
}


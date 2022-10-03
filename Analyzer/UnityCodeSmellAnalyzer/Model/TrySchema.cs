using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UnityCodeSmellAnalyzer
{
    public class TrySchema : CycleOrControlSchema
    {
        protected List<CatchSchema> catchBlocks = new List<CatchSchema>();
        protected FinallySchema finallyBlock;
        [JsonIgnore]
        public override int Line { get { return line; } }
        [JsonIgnore]
        public override int Depth {get { return depth; } }
        public List<CatchSchema> CatchBlocks { get { return catchBlocks; } }
        public FinallySchema FinallyBlock { get { return finallyBlock; } }
        public TrySchema() { }

        protected void AddCatch(CatchSchema ca)
        {
            catchBlocks.Add(ca);
        }

        /// <summary>
        /// Loads all the Catch Blocks referenced to this Try Statement
        /// </summary>
        /// <param name="root">The Try Block</param>
        /// <param name="model">The model</param>
        protected void LoadCatches(SyntaxNode root, SemanticModel model)
        {
            TryStatementSyntax t = root as TryStatementSyntax;
            foreach (var ca in t.Catches)
            {
                CatchSchema c = new CatchSchema();
                c.LoadInformations(ca, model);
                AddCatch(c);
            }  
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            TryStatementSyntax t = root as TryStatementSyntax;
            startLine = t.GetLocation().GetLineSpan().StartLinePosition.Line;
            endLine = t.GetLocation().GetLineSpan().EndLinePosition.Line;
            LoadCatches(t, model);   
            if(t.Finally != null)
            {
                finallyBlock = new FinallySchema();
                finallyBlock.LoadInformations(t.Finally, model);
            }
        }
    }
}




using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class WhileSchema : CycleOrControlSchema
    {

        protected string condition;
        protected List<WhileSchema> whileLoops = new List<WhileSchema>();

        public WhileSchema(int line, int depth, string condition) : base(line, depth)
        {
            this.condition = condition;
        }
        
        public string Condition { get { return condition; } }
        public List<WhileSchema> WhileLoops { get { return whileLoops; } }

        public void AddWhileLoop(WhileSchema w) { whileLoops.Add(w); }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            List<WhileStatementSyntax> wbl = (from wh in root.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();

            foreach (var w in wbl)
            {
                if(w.Parent == root)
                {
                    WhileSchema ws = new WhileSchema(w.GetLocation().GetLineSpan().StartLinePosition.Line, Depth + 1, w.Condition.ToString());
                    ws.LoadInformations(w, model);
                    AddWhileLoop(ws);
                }
               
            }
        }

    }
}



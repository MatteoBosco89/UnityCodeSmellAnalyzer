using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;


namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Abstract Class representing a Cycle or a Control Structure. Inherit SyntaxSchema. 
    /// Information gathered: Depth, Start Line, End Line, List of Invocations, List of Variables, 
    /// List of Cycles, List of Control Structures
    /// </summary>
    [Serializable]
    public abstract class CycleOrControlSchema : SyntaxSchema
    {
        protected int startLine;
        protected int depth;
        protected int endLine;
        protected List<InvocationSchema> invocations = new List<InvocationSchema>();
        protected List<VariableSchema> variables = new List<VariableSchema>();
        protected List<ReturnSchema> returns = new List<ReturnSchema>();
        protected List<CycleOrControlSchema> whileBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> forBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> ifBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> switchBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> forEachBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> elseBlocks = new List<CycleOrControlSchema>();
        protected void IncrementDepth() { depth += 1; }
        public virtual int Depth { get { return depth; } set { depth = value; } }
        public int EndLine { get { return endLine + 1; } }
        public int StartLine { get { return startLine + 1; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<CycleOrControlSchema> WhileBlocks { get { return whileBlocks; } }
        public List<CycleOrControlSchema> ForBlocks { get { return forBlocks; } }
        public List<CycleOrControlSchema> IfBlocks { get { return ifBlocks; } }
        public List<CycleOrControlSchema> SwitchBlocks { get { return switchBlocks; } }
        public List<CycleOrControlSchema> ForEachBlocks { get { return forEachBlocks; } }
        public List<CycleOrControlSchema> ElseBlocks { get { return elseBlocks; } }
        public List<ReturnSchema> Returns { get { return returns; } }
        public void AddWhile(CycleOrControlSchema w)
        {
            whileBlocks.Add(w);
        }
        public void AddFor(CycleOrControlSchema f)
        {
            forBlocks.Add(f);
        }
        public void AddForEach(CycleOrControlSchema fe)
        {
            forEachBlocks.Add(fe);
        }
        public void AddIf(CycleOrControlSchema i)
        {
            ifBlocks.Add(i);
        }
        public void AddElse(CycleOrControlSchema e)
        {
            elseBlocks.Add(e);
        }
        public void AddSwitch(CycleOrControlSchema sc)
        {
            switchBlocks.Add(sc);
        }
        public void AddInvocation(InvocationSchema i)
        {
            invocations.Add(i);
        }
        public void AddVariable(VariableSchema v)
        {
            variables.Add(v);
        }
        public void AddReturn(ReturnSchema r)
        {
            returns.Add(r);
        }
        
        /// <summary>
        /// Loads all the Invocation in the SyntaxBlock, parent and children invocations are not included
        /// </summary>
        /// <param name="root">The SyntaxBlock</param>
        /// <param name="invocations">List of Invocations</param>
        /// <param name="model">The model</param>
        public virtual void LoadInvocations(SyntaxNode root, List<InvocationExpressionSyntax> invocations, SemanticModel model)
        {
            foreach (var inv in invocations)
            {
                if(SyntaxWalker.SearchParent(inv, SyntaxWalker.InvocationAncestors) == root)
                {
                    InvocationSchema invocation = new InvocationSchema();
                    invocation.LoadInformations(inv, model);
                    AddInvocation(invocation);
                }
                    
            }
        }
        /// <summary>
        /// Loads all the Variables in the SyntaxBlock, parent and children variables are not included
        /// </summary>
        /// <param name="root">The SyntaxBlock</param>
        /// <param name="vdsl">List of Declarations</param>
        /// <param name="aesl">List of Assignments</param>
        /// <param name="model">The model</param>
        public virtual void LoadVariables(SyntaxNode root, List<VariableDeclarationSyntax> vdsl, List<AssignmentExpressionSyntax> aesl, SemanticModel model)
        {
            // Definition
            foreach (var v in vdsl)
            {
                foreach (var declarator in v.Variables)
                {
                    if (SyntaxWalker.SearchParent(declarator, SyntaxWalker.VariablesAncestors) == root)
                    {
                        VariableSchema variable = new VariableSchema();
                        variable.LoadDefinition(v, declarator, model);
                        AddVariable(variable);
                    }

                }
            }
            // Assignment
            foreach (var v in aesl)
            {
                if (SyntaxWalker.SearchParent(v, SyntaxWalker.VariablesAncestors) == root)
                {
                    VariableSchema variable = new VariableSchema();
                    variable.LoadAssignment(v, model);
                    AddVariable(variable);
                }
            }
            // Use
            foreach (var v in aesl)
            {
                if (SyntaxWalker.SearchParent(v, SyntaxWalker.VariablesAncestors) == root)
                {
                    VariableSchema variable = new VariableSchema();
                    variable.LoadUsage(v, model);
                    AddVariable(variable);
                }
            }
        }

        /// <summary>
        /// Loads direct children While Loops.
        /// </summary>
        /// <param name="root">Syntax Root</param>
        /// <param name="wbl">List of While Statements</param>
        /// <param name="model">The model</param>
        public void LoadWhileStatement(SyntaxNode root, List<WhileStatementSyntax> wbl, SemanticModel model)
        {
            foreach (var w in wbl)
            {
                if (SyntaxWalker.SearchParent(w, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    WhileSchema ws = new WhileSchema();
                    ws.Depth = depth;
                    ws.LoadInformations(w, model);
                    AddWhile(ws);
                }

            }
        }

        /// <summary>
        /// Loads direct children Foreach Statements.
        /// </summary>
        /// <param name="root">Syntax Root</param>
        /// <param name="fel">List of ForEach Statements</param>
        /// <param name="model">The model</param>
        public void LoadForEachStatement(SyntaxNode root, List<ForEachStatementSyntax> fel, SemanticModel model)
        {
            foreach (var f in fel)
            {
                if (SyntaxWalker.SearchParent(f, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    ForEachSchema fe = new ForEachSchema();
                    fe.Depth = depth;
                    fe.LoadInformations(f, model);
                    AddForEach(fe);
                }

            }
        }

        /// <summary>
        /// Loads If Control Structures inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="ifssl">List of If Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadIfStatement(SyntaxNode root, List<IfStatementSyntax> ifssl, SemanticModel model)
        {
            foreach (var ifss in ifssl)
            {
                if (SyntaxWalker.SearchParent(ifss, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    IfSchema i = new IfSchema();
                    i.Depth = depth;
                    i.LoadInformations(ifss, model);
                    AddIf(i);
                    //Console.WriteLine(ifss.Condition);
                    //Console.WriteLine(ifss.Else);
                }
            }
        }
        /// <summary>
        /// Loads For Loops inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="fssl">List of For Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadForStatement(SyntaxNode root, List<ForStatementSyntax> fssl, SemanticModel model)
        {
            foreach (var fss in fssl)
            {
                if (SyntaxWalker.SearchParent(fss, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    ForSchema fs = new ForSchema();
                    fs.Depth = depth;
                    fs.LoadInformations(fss, model);
                    AddFor(fs);
                }
            }
        }
        /// <summary>
        /// Loads Switch Control Structure inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="sssl">List of Switch-Case Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadSwitchStatement(SyntaxNode root, List<SwitchStatementSyntax> sssl, SemanticModel model)
        {
            foreach (var sss in sssl)
            {
                if (SyntaxWalker.SearchParent(sss, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    SwitchSchema s = new SwitchSchema();
                    s.Depth = depth;
                    s.LoadInformations(sss, model);
                    AddSwitch(s);
                }
            }
        }
        /// <summary>
        /// Loads direct children Return Statements.
        /// </summary>
        /// <param name="root">Syntax Root</param>
        /// <param name="rssl">List of Return Statements</param>
        /// <param name="model">The model</param>
        protected void LoadReturnStatements(SyntaxNode root, List<ReturnStatementSyntax> rssl, SemanticModel model)
        {
            foreach (var rss in rssl)
            {
                if (SyntaxWalker.SearchParent(rss, SyntaxWalker.ControlOrCycleAncestors) == root)
                {
                    ReturnSchema returnSchema = new ReturnSchema();
                    returnSchema.LoadInformations(rss, model);
                    AddReturn(returnSchema);
                }
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
            IncrementDepth();
            List<WhileStatementSyntax> wbl = (from wh in root.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<InvocationExpressionSyntax> invocations = (from wh in root.DescendantNodes().OfType<InvocationExpressionSyntax>() select wh).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in root.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<AssignmentExpressionSyntax> aesl = (from ae in root.DescendantNodes().OfType<AssignmentExpressionSyntax>() select ae).ToList();
            List<ForEachStatementSyntax> fel = (from fe in root.DescendantNodes().OfType<ForEachStatementSyntax>() select fe).ToList();
            List<ReturnStatementSyntax> rssl = (from rs in root.DescendantNodes().OfType<ReturnStatementSyntax>() select rs).ToList();
            List<IfStatementSyntax> issl = (from i in root.DescendantNodes().OfType<IfStatementSyntax>() select i).ToList();
            List<ForStatementSyntax> fssl = (from fr in root.DescendantNodes().OfType<ForStatementSyntax>() select fr).ToList();
            List<SwitchStatementSyntax> sssl = (from ss in root.DescendantNodes().OfType<SwitchStatementSyntax>() select ss).ToList();

            LoadWhileStatement(root, wbl, model);
            LoadForEachStatement(root, fel, model);
            LoadForStatement(root, fssl, model);
            LoadIfStatement(root, issl, model);
            LoadSwitchStatement(root, sssl, model);
            LoadInvocations(root, invocations, model);
            LoadVariables(root, vdsl, aesl, model);
            LoadReturnStatements(root, rssl, model);
            
        }

    }
}


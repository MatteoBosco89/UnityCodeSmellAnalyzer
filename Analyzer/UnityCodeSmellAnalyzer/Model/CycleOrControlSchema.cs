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
        protected List<CycleOrControlSchema> whileBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> forBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> ifBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> switchCaseBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> forEachBlocks = new List<CycleOrControlSchema>();
        protected List<CycleOrControlSchema> elseBlocks = new List<CycleOrControlSchema>();
        public int Depth { get { return depth; } set { depth = value + 1; } }
        public int EndLine { get { return endLine; } }
        public int StartLine { get { return startLine; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<CycleOrControlSchema> WhileBlocks { get { return whileBlocks; } }
        public List<CycleOrControlSchema> ForBlocks { get { return forBlocks; } }
        public List<CycleOrControlSchema> IfBlocks { get { return ifBlocks; } }
        public List<CycleOrControlSchema> SwitchCaseBlocks { get { return switchCaseBlocks; } }
        public List<CycleOrControlSchema> ForEachBlocks { get { return forEachBlocks; } }
        public List<CycleOrControlSchema> ElseBlocks { get { return elseBlocks; } }
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
        public void AddSwitchCase(CycleOrControlSchema sc)
        {
            switchCaseBlocks.Add(sc);
        }
        public void AddInvocation(InvocationSchema i)
        {
            invocations.Add(i);
        }
        public void AddVariable(VariableSchema v)
        {
            variables.Add(v);
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
                if ((w.Parent as BlockSyntax)?.Parent == root)
                {
                    WhileSchema ws = new WhileSchema();
                    ws.LoadInformations(w, model);
                    ws.Depth = depth;
                    AddWhile(ws);
                }

            }
        }

        /// <summary>
        /// Loads direct children Foreach Statements.
        /// </summary>
        /// <param name="root">Syntax Root</param>
        /// <param name="wbl">List of ForEach Statements</param>
        /// <param name="model">The model</param>
        public void LoadForEachStatement(SyntaxNode root, List<ForEachStatementSyntax> fel, SemanticModel model)
        {
            foreach (var f in fel)
            {
                if ((f.Parent as BlockSyntax)?.Parent == root)
                {
                    ForEachSchema fe = new ForEachSchema();
                    fe.LoadInformations(f, model);
                    fe.Depth = depth;
                    AddForEach(fe);
                }

            }
        }

    }
}


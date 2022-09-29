using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Method Declaration. 
    /// Information gathered: Name, FullName (Namespace.Class), Return Type, Modifiers, Parameters, Invocations, Variables, 
    /// Statements, Cycles, Control Structures, Attributes, LOC
    /// </summary>
    [Serializable]
    public class MethodSchema : SyntaxSchema
    {
        protected string name;
        protected List<string> modifiers = new List<string>();
        protected string returnType;
        protected string fullName;
        protected List<ParameterSchema> parameters = new List<ParameterSchema>();
        protected List<InvocationSchema> invocations = new List<InvocationSchema>();
        protected List<VariableSchema> variables = new List<VariableSchema>();
        protected List<StatementSchema> statements = new List<StatementSchema>();
        protected List<WhileSchema> whileBlocks = new List<WhileSchema>();
        protected List<ForEachSchema> forEachBlocks = new List<ForEachSchema>();
        protected List<string> attributes = new List<string>();


        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public List<string> Modifiers { get { return modifiers; } }
        public List<string> Attributes { get { return attributes; } }
        public string ReturnType { get { return returnType; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<StatementSchema> Statements { get { return statements; } }
        public List<WhileSchema> WhileBlocks { get { return whileBlocks; } }
        public List<ForEachSchema> ForEachBlocks { get { return forEachBlocks; } }


        public MethodSchema() { }

        public void AddParameter(ParameterSchema p)
        {
            parameters.Add(p);
        }

        public void AddInvocation(InvocationSchema i)
        {
            invocations.Add(i);
        }

        public void AddVariable(VariableSchema v)
        {
            variables.Add(v);
        }

        public void AddStatement(StatementSchema s)
        {
            statements.Add(s);
        }

        public void AddModifier(string m)
        {
            modifiers.Add(m);
        }
        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

        public void AddWhileLoop(WhileSchema w)
        {
            whileBlocks.Add(w);
        }
        public void AddForEach(ForEachSchema f)
        {
            forEachBlocks.Add(f);
        }
        /// <summary>
        /// Check for configuration to include the TextStatement
        /// (The real line of code)
        /// </summary>
        /// <param name="ssl">List of Statements</param>
        /// <param name="model">The model</param>
        protected void LoadStatements(List<StatementSyntax> ssl, SemanticModel model)
        {
            if (AnalyzerConfiguration.StatementVerbose)
            {
                foreach (var s in ssl)
                {
                    StatementSchema statement = new StatementSchema();
                    statement.LoadInformations(s, model);
                    AddStatement(statement);
                }
            }
        }
        /// <summary>
        /// Loads the method parameters
        /// </summary>
        /// <param name="method">The Method Declaration</param>
        /// <param name="model">The model</param>
        protected void LoadParameters(MethodDeclarationSyntax method, SemanticModel model)
        {
            foreach (var param in method.ParameterList.Parameters)
            {
                ParameterSchema parameter = new ParameterSchema();
                parameter.LoadInformations(param, model);
                AddParameter(parameter);
            }
        }
        /// <summary>
        /// Loads the invocations made from the method
        /// </summary>
        /// <param name="idsl">List of Invocations Expression</param>
        /// <param name="model">The model</param>
        protected void LoadInvocations(SyntaxNode root, List<InvocationExpressionSyntax> idsl, SemanticModel model)
        {
            
            foreach (var i in idsl)
            {
                if (SyntaxWalker.SearchParent(i, SyntaxWalker.InvocationAncestors) == root)
                {
                    InvocationSchema invoc = new InvocationSchema();
                    invoc.LoadInformations(i, model);
                    AddInvocation(invoc);
                }
                    
            }
        }
        /// <summary>
        /// Variables Snippet divided in
        /// Definition
        /// Assignment
        /// Use
        /// For data flow analysis purpose
        /// </summary>
        /// <param name="vdsl">List of Variable Declaration</param>
        /// <param name="aesl">List of Assignment Expression</param>
        /// <param name="model">The model</param>
        protected void LoadVariables(SyntaxNode root, List<VariableDeclarationSyntax> vdsl, List<AssignmentExpressionSyntax> aesl, SemanticModel model)
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
        /// Method modifiers (public, static, abstract, ...)
        /// </summary>
        /// <param name="method">The Method Declaration</param>
        protected void LoadModifiers(MethodDeclarationSyntax method)
        {
            foreach (var mod in method.Modifiers)
            {
                AddModifier(mod.ValueText);
            }
        }
        /// <summary>
        /// Method attributes (Command, ...)
        /// </summary>
        /// <param name="method">The Method Declaration</param>
        protected void LoadAttributes(MethodDeclarationSyntax method)
        {
            foreach (var attr in method.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }
        /// <summary>
        /// Loads the While Loops inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="wbl">List of While Statements</param>
        /// <param name="model">The model</param>
        protected void LoadWhileStatement(SyntaxNode root, List<WhileStatementSyntax> wbl, SemanticModel model)
        {
            foreach (var w in wbl)
            {
                if ((w.Parent as BlockSyntax)?.Parent == root)
                {
                    WhileSchema ws = new WhileSchema();
                    ws.LoadInformations(w, model);
                    AddWhileLoop(ws);
                }
            }
        }
        /// <summary>
        /// Loads ForEach Loops inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="fessl">List of ForEach Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadForeachStatement(SyntaxNode root, List<ForEachStatementSyntax> fessl, SemanticModel model)
        {
            foreach (var fess in fessl)
            {
                if((fess.Parent as BlockSyntax)?.Parent == root)
                {
                    ForEachSchema fes = new ForEachSchema();
                    fes.LoadInformations(fess, model);
                    AddForEach(fes);
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
                if((fss.Parent as BlockSyntax)?.Parent == root)
                {

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
                if((ifss.Parent as BlockSyntax)?.Parent == root)
                {
                    
                }
            }
        }
        /// <summary>
        /// Loads Else Control Structures inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="ifssl">List of Else Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadElseStatement(SyntaxNode root, List<ElseClauseSyntax> ecsl, SemanticModel model)
        {

        }
        /// <summary>
        /// Loads Switch-Case Control Structure inside the method with hierarchy
        /// </summary>
        /// <param name="root">The Syntax Root (direct parent)</param>
        /// <param name="sssl">List of Switch-Case Statements</param>
        /// <param name="model">The Model</param>
        protected void LoadSwitchCaseStatement(SyntaxNode root, List<SwitchStatementSyntax> sssl, SemanticModel model)
        {
            foreach (var sss in sssl)
            {
                if((sss.Parent as BlockSyntax).Parent == root)
                {

                }
            }
        }


        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            MethodDeclarationSyntax method = root as MethodDeclarationSyntax;
           
            LoadBasicInformations(root, model);

            List<InvocationExpressionSyntax> idsl = (from invoc in method.DescendantNodes().OfType<InvocationExpressionSyntax>() select invoc).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in method.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();   
            List<StatementSyntax> ssl = (from stat in method.DescendantNodes().OfType<StatementSyntax>() select stat).ToList();
            List<AssignmentExpressionSyntax> aesl = (from ae in method.DescendantNodes().OfType<AssignmentExpressionSyntax>() select ae).ToList();
            List<WhileStatementSyntax> wbl = (from wh in method.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<ForEachStatementSyntax> fessl = (from fe in method.DescendantNodes().OfType<ForEachStatementSyntax>() select fe).ToList();
            List<IfStatementSyntax> ifssl = (from iff in method.DescendantNodes().OfType<IfStatementSyntax>() select iff).ToList();
            List<ForStatementSyntax> fssl = (from fr in method.DescendantNodes().OfType<ForStatementSyntax>() select fr).ToList();
            List<SwitchStatementSyntax> sssl = (from ss in method.DescendantNodes().OfType<SwitchStatementSyntax>() select ss).ToList();
            List<ElseClauseSyntax> ecsl = (from es in method.DescendantNodes().OfType<ElseClauseSyntax>() select es).ToList();

            LoadInvocations(method, idsl, model);
            LoadVariables(method, vdsl, aesl, model);
            LoadStatements(ssl, model);
            LoadModifiers(method);
            LoadAttributes(method);
            LoadWhileStatement(method, wbl, model);
            LoadForeachStatement(method, fessl, model);
            LoadIfStatement(method, ifssl, model);
            LoadElseStatement(method, ecsl, model);
            LoadSwitchCaseStatement(method, sssl, model);
            LoadForStatement(method, fssl, model);
            
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            MethodDeclarationSyntax method = root as MethodDeclarationSyntax;
            name = method.Identifier.ToString();
            line = method.GetLocation().GetLineSpan().StartLinePosition.Line;
            returnType = method.ReturnType.ToString();
            fullName = model.GetDeclaredSymbol(method).ConstructedFrom.ToString();
        }
    }
}


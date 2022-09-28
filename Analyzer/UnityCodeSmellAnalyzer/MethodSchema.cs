using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
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
        protected List<WhileSchema> whileLoops = new List<WhileSchema>();


        public string Name { get { return name; } }
        public string FullName { get { return fullName; } }
        public List<string> Modifiers { get { return modifiers; } }
        public string ReturnType { get { return returnType; } }
        public List<ParameterSchema> Parameters { get { return parameters; } }
        public List<InvocationSchema> Invocations { get { return invocations; } }
        public List<VariableSchema> Variables { get { return variables; } }
        public List<StatementSchema> Statements { get { return statements; } }
        public List<WhileSchema> WhileLoops { get { return whileLoops; } }

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

        public void AddWhileLoop(WhileSchema w)
        {
            whileLoops.Add(w);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            MethodDeclarationSyntax method = root as MethodDeclarationSyntax;

            name = method.Identifier.ToString();
            line = method.GetLocation().GetLineSpan().StartLinePosition.Line;
            returnType = method.ReturnType.ToString();
            fullName = model.GetDeclaredSymbol(method).ConstructedFrom.ToString();

            List<InvocationExpressionSyntax> idsl = (from invoc in method.DescendantNodes().OfType<InvocationExpressionSyntax>() select invoc).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in method.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<StatementSyntax> ssl = (from stat in method.DescendantNodes().OfType<StatementSyntax>() select stat).ToList();
            List<WhileStatementSyntax> wbl = (from wh in method.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<ForEachStatementSyntax> fessl = (from fe in method.DescendantNodes().OfType<ForEachStatementSyntax>() select fe).ToList();
            List<IfStatementSyntax> ifssl = (from iff in method.DescendantNodes().OfType<IfStatementSyntax>() select iff).ToList();
            List<ForStatementSyntax> fssl = (from fr in method.DescendantNodes().OfType<ForStatementSyntax>() select fr).ToList();
            //List<>

            if (AnalyzerConfiguration.StatementVerbose)
            {
                foreach (var s in ssl)
                {
                    StatementSchema statement = new StatementSchema();
                    statement.LoadInformations(s, model);
                    AddStatement(statement);
                }
            }

            foreach (var param in method.ParameterList.Parameters)
            {
                ParameterSchema parameter = new ParameterSchema();
                parameter.LoadInformations(param, model);
                AddParameter(parameter);
            }

            foreach (var i in idsl) {
                InvocationSchema invoc = new InvocationSchema();
                invoc.LoadInformations(i, model);
                AddInvocation(invoc);
            }

            foreach(var v in vdsl)
            {
                foreach(var vv in v.Variables)
                {
                    string initializer = null;
                    if(vv.Initializer != null) initializer = vv.Initializer.Value.ToString();
                    VariableSchema variable = new VariableSchema(vv.Identifier.ToString(), v.Type.ToString(), initializer, vv.GetLocation().GetLineSpan().StartLinePosition.Line);
                    AddVariable(variable);
                }
                
            }

            foreach (var mod in method.Modifiers)
            {
                AddModifier(mod.ValueText);
            }

            foreach (var w in wbl)
            {
                if(w.Parent == root)
                {
                    WhileSchema ws = new WhileSchema(w.GetLocation().GetLineSpan().StartLinePosition.Line, 0, w.Condition.ToString());
                    ws.LoadInformations(w, model);
                    AddWhileLoop(ws);
                }
            }

            foreach (var f in fessl)
            {
                Console.WriteLine(f.ForEachKeyword);
                Console.WriteLine(f.Type);
                Console.WriteLine(f.Identifier);
                Console.WriteLine(f.InKeyword);
                Console.WriteLine(f.Expression);
                Console.WriteLine();
            }

            foreach(var iff in ifssl)
            {

            }

            foreach(var fr in fssl)
            {

            }

        }

    }
}


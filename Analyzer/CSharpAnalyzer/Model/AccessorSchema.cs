using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyzer
{
    public class AccessorSchema : MethodSchema
    {
        protected string accessorType;

        [JsonIgnore] public override string Name { get { return name; } }
        [JsonIgnore] public override string FullName { get { return fullName; } }

        public string AccessorType { get { return accessorType; } }
        public AccessorSchema() { }
       
        public void LoadAttribute(AccessorDeclarationSyntax acc)
        {
            foreach (var attr in acc.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }

        public void LoadModifier(AccessorDeclarationSyntax acc)
        {
            foreach (var mod in acc.Modifiers)
            {
                AddModifier(mod.ToString());
            }
        }
        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            AccessorDeclarationSyntax acc = root as AccessorDeclarationSyntax;
            LoadAttribute(acc);
            LoadModifier(acc);
            accessorType = acc.Keyword.ToString();
            line = acc.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            AccessorDeclarationSyntax acc = root as AccessorDeclarationSyntax;
            LoadBasicInformations(root, model);
            if(acc.Body != null)
            {
                isEmpty = false;
                hasBody = true;
                LoadData(root, acc, model);
            }
            if(acc.ExpressionBody != null)
            {
                hasBodyExpression = true;
                isEmpty = false;
                expressionBody = new ConditionSchema();
                expressionBody.LoadInformations(acc.ExpressionBody.Expression, model);
            }
            if (acc.Body == null && acc.ExpressionBody == null) isEmpty = true;
        }

        /// <summary>
        /// Loads all the data
        /// </summary>
        /// <param name="root">Accessor Declaration</param>
        /// <param name="body">Body of the Accessor </param>
        /// <param name="model">The model</param>
        protected void LoadData(SyntaxNode root, SyntaxNode body, SemanticModel model)
        {
            List<InvocationExpressionSyntax> idsl = (from invoc in body.DescendantNodes().OfType<InvocationExpressionSyntax>() select invoc).ToList();
            List<VariableDeclarationSyntax> vdsl = (from variab in body.DescendantNodes().OfType<VariableDeclarationSyntax>() select variab).ToList();
            List<StatementSyntax> ssl = (from stat in body.DescendantNodes().OfType<StatementSyntax>() select stat).ToList();
            List<AssignmentExpressionSyntax> aesl = (from ae in body.DescendantNodes().OfType<AssignmentExpressionSyntax>() select ae).ToList();
            List<WhileStatementSyntax> wbl = (from wh in body.DescendantNodes().OfType<WhileStatementSyntax>() select wh).ToList();
            List<ForEachStatementSyntax> fessl = (from fe in body.DescendantNodes().OfType<ForEachStatementSyntax>() select fe).ToList();
            List<IfStatementSyntax> ifssl = (from iff in body.DescendantNodes().OfType<IfStatementSyntax>() select iff).ToList();
            List<ForStatementSyntax> fssl = (from fr in body.DescendantNodes().OfType<ForStatementSyntax>() select fr).ToList();
            List<SwitchStatementSyntax> sssl = (from ss in body.DescendantNodes().OfType<SwitchStatementSyntax>() select ss).ToList();
            List<ReturnStatementSyntax> rssl = (from rs in body.DescendantNodes().OfType<ReturnStatementSyntax>() select rs).ToList();

            LoadInvocations(root, idsl, model);
            LoadVariables(root, vdsl, aesl, model);
            LoadStatements(ssl, model);
            LoadWhileStatement(root, wbl, model);
            LoadForeachStatement(root, fessl, model);
            LoadIfStatement(root, ifssl, model);
            LoadForStatement(root, fssl, model);
            LoadSwitchStatement(root, sssl, model);
            LoadReturnStatements(root, rssl, model);
        }


    }
}


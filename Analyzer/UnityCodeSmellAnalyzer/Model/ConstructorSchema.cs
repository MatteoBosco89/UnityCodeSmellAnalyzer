using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Class Constructor. Inherit MethodSchema
    /// Informations gathered: Reference Class, Is Empty, Return Type, Name, LOC
    /// </summary>
    [Serializable]
    public class ConstructorSchema : MethodSchema
    {
        protected string classRef;
        
        protected ConstructorInitializerSchema initializer;
        public string ClassRef { get { return classRef; } set { classRef = value; } }
        public override string ReturnType { get { return returnType; } set { returnType = value; } }
        

        public ConstructorSchema() { }

        /// <summary>
        /// Constructor modifiers (public, static, abstract, ...)
        /// </summary>
        /// <param name="cons">The Constructor Declaration</param>
        protected void LoadModifiers(ConstructorDeclarationSyntax cons)
        {
            foreach (var mod in cons.Modifiers)
            {
                AddModifier(mod.ValueText);
            }
        }
        /// <summary>
        /// Constructor attributes (Command, ...)
        /// </summary>
        /// <param name="cons">The Constructor Declaration</param>
        protected void LoadAttributes(ConstructorDeclarationSyntax cons)
        {
            foreach (var attr in cons.AttributeLists)
            {
                foreach (var a in attr.Attributes) AddAttribute(a.Name.ToString());
            }
        }
        /// <summary>
        /// Loads the Constructor parameters
        /// </summary>
        /// <param name="cons">The Constructor Declaration</param>
        /// <param name="model">The model</param>
        protected void LoadParameters(ConstructorDeclarationSyntax cons, SemanticModel model)
        {
            foreach (var param in cons.ParameterList.Parameters)
            {
                ParameterSchema parameter = new ParameterSchema();
                parameter.LoadInformations(param, model);
                AddParameter(parameter);
            }
        }

        /// <summary>
        /// Loads The Initializer if any 
        /// </summary>
        /// <param name="cons">The Class Initializer</param>
        /// <param name="model">The model</param>
        protected void LoadInitializer(ConstructorInitializerSyntax cons, SemanticModel model)
        {
            initializer = new ConstructorInitializerSchema();
            initializer.LoadInformations(cons, model);
        }

        /// <summary>
        /// Loads the Body of the Constructor
        /// </summary>
        /// <param name="root">The Body of the Constructor</param>
        /// <param name="model">The model</param>
        protected void LoadConstructorBody(SyntaxNode root, SemanticModel model)
        {
            LoadData(root, (root as ConstructorDeclarationSyntax).Body, model);
        }
        /// <summary>
        /// Loads the ExpressionBody of the Constructor
        /// </summary>
        /// <param name="root">ExpressionBody of the Construcotr</param>
        /// <param name="model">The model</param>
        protected void LoadConstructorExpressionBody(SyntaxNode root, SemanticModel model)
        {
            LoadData(root, (root as ConstructorDeclarationSyntax).ExpressionBody.Expression, model);
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);
            ConstructorDeclarationSyntax cons = root as ConstructorDeclarationSyntax;
            LoadAttributes(cons);
            LoadModifiers(cons);
            LoadParameters(cons, model);
            if (cons.Initializer != null) LoadInitializer(cons.Initializer, model);
            if(cons.Body != null && cons.Body?.DescendantNodes().Count() > 0)
            {
                LoadConstructorBody(cons, model);
                hasBody = true;
            }
            if(cons.ExpressionBody != null && cons.ExpressionBody?.DescendantNodes().Count() > 0)
            {
                LoadConstructorExpressionBody(cons, model);
                hasBodyExpression = true;
            }
            if(!hasBody && !hasBodyExpression)
            {
                isEmpty = true;
            }
            
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            ConstructorDeclarationSyntax cons = root as ConstructorDeclarationSyntax;
            name = cons.Identifier.ToString();
            fullName = cons.Identifier.ToString();
            line = cons.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        /// <summary>
        /// Loads all the data
        /// </summary>
        /// <param name="root">Constructor Declaration</param>
        /// <param name="body">Body of the Constructor (Body/ExpressionBody)</param>
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


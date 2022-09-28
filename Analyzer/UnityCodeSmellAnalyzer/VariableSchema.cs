using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class VariableSchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected string assignment;
        protected string kind;
        protected VariableSchema variable;
        protected enum VariableKind { Definition, Assignment, Use }

        [JsonIgnore]
        public override int Line { get { return line; } }

        public virtual VariableSchema Variable { get { return variable; } }

        public VariableSchema() { }

        public void AddVariable(SyntaxNode v)
        {
            if(v is VariableDeclarationSyntax) { }
        }

        public void LoadDefinition(SyntaxNode root, SyntaxNode v, SemanticModel model)
        {
            DeclaredVariableSchema schema = new DeclaredVariableSchema();
            schema.LoadMe(root, v, model);
            variable = schema;
        }

        public void LoadAssignment(SyntaxNode root, SemanticModel model)
        {
            AssignedVariableSchema schema = new AssignedVariableSchema();
            schema.LoadMe(root, model);
            variable = schema;
        }

        public void LoadUsage(SyntaxNode root, SemanticModel model)
        {
            UsedVariableSchema schema = new UsedVariableSchema();
            schema.LoadMe(root, model);
            variable = schema;
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model) { }

    }

    [Serializable]
    class UsedVariableSchema : VariableSchema
    {
        protected string useKind;
        protected string assignedTo;
        protected string value;
        protected List<UsedVariableSchema> variablesList = new List<UsedVariableSchema>();
        [JsonIgnore]
        public override VariableSchema Variable { get { return variable; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Kind { get { return kind; } }
        public int UseLine { get { return line; } }
        public string UseKind { get { return useKind; } }
        public string AssignedTo { get { return assignedTo; } }
        public string Value { get { return value; } }
        public List<UsedVariableSchema> VariablesList { get { return variablesList; } }
        protected void AddVariable(UsedVariableSchema v) { variablesList.Add(v); }
        public UsedVariableSchema() { }
        protected void LoadInternal(SyntaxNode root, SemanticModel model, UsedVariableSchema v)
        {
            kind = VariableKind.Use.ToString();
            assignedTo = v.AssignedTo;
            line = v.Line;
            type = model.GetTypeInfo(root).Type.ToString();
            useKind = model.GetOperation(root)?.Kind.ToString();
            name = root.ToString();
            if (model.GetOperation(root)?.Kind is OperationKind.Literal)
            {
                value = name;
                name = null;
            }
            if (model.GetOperation(root)?.Kind is OperationKind.Binary)
            {
                UsedVariableSchema left = new UsedVariableSchema();
                UsedVariableSchema right = new UsedVariableSchema();
                var m = (model.GetOperation(root) as IBinaryOperation);
                left.LoadInternal(m.LeftOperand.Syntax, model, this);
                right.LoadInternal(m.RightOperand.Syntax, model, this);
                AddVariable(left);
                AddVariable(right);
            }
        }
        public void LoadMe(SyntaxNode root, SemanticModel model)
        {
            kind = VariableKind.Use.ToString();
            AssignmentExpressionSyntax exp = root as AssignmentExpressionSyntax;
            assignedTo = exp.Left.ToString();
            line = exp.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = model.GetTypeInfo(exp).Type.ToString();
            useKind = model.GetOperation(exp.Right)?.Kind.ToString();
            name = exp.Right.ToString();
            if (model.GetOperation(exp.Right)?.Kind is OperationKind.Literal)
            {
                value = name;
                name = null;
            }
            if(model.GetOperation(exp.Right)?.Kind is OperationKind.Binary)
            {
                UsedVariableSchema left = new UsedVariableSchema();
                UsedVariableSchema right = new UsedVariableSchema();
                var m = (model.GetOperation(exp.Right) as IBinaryOperation);
                left.LoadInternal(m.LeftOperand.Syntax, model, this);
                right.LoadInternal(m.RightOperand.Syntax, model, this);
                AddVariable(left);
                AddVariable(right);
            }
        }
    }

    [Serializable]
    class AssignedVariableSchema : VariableSchema
    {
        protected string assignmentKind;
        [JsonIgnore]
        public override VariableSchema Variable { get { return variable; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }
        public string Kind { get { return kind; } }
        public int AssignmentLine { get { return line; } }
        public string AssignmentKind { get { return assignmentKind; } }
        public AssignedVariableSchema() { }

        public void LoadMe(SyntaxNode root, SemanticModel model)
        {
            kind = VariableKind.Assignment.ToString();
            AssignmentExpressionSyntax exp = root as AssignmentExpressionSyntax;
            assignmentKind = exp.Kind().ToString();
            name = exp.Left.ToString();
            line = exp.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = model.GetTypeInfo(exp).Type.ToString();
            assignment = exp.Right.ToString();
        }
    }

    [Serializable]
    class DeclaredVariableSchema : VariableSchema
    {
        [JsonIgnore]
        public override VariableSchema Variable { get { return variable; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }
        public string Kind { get { return kind; } }
        public int DefinitionLine { get { return line; } }

        public DeclaredVariableSchema() { }
        public void LoadMe(SyntaxNode root, SyntaxNode v, SemanticModel model)
        {
            kind = VariableKind.Definition.ToString();
            VariableDeclaratorSyntax variable = v as VariableDeclaratorSyntax;
            if (variable.Initializer != null) assignment = variable.Initializer.Value.ToString();
            name = variable.Identifier.ToString();
            line = variable.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = (root as VariableDeclarationSyntax).Type.ToString();
        }
    }

}


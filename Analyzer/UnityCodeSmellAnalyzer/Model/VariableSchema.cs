using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Simple Variable. 
    /// Informations gathered: Name, Type, Kind Enum(Definition, Assignment, Use), Specialized Variable, LOC
    /// </summary>
    [Serializable]
    public class VariableSchema : SyntaxSchema
    {
        protected string name;
        protected string type;
        protected string kind;
        protected VariableSchema variable;
        protected enum VarKind { Definition, Assignment, Use }

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

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model) { }
    }

    /// <summary>
    /// Class representing the Variable of kind Use, inherit from VariableSchema. 
    /// Informations gathered: Use Kind, Assigned To, Value, Variable List
    /// </summary>
    [Serializable]
    class UsedVariableSchema : VariableSchema
    {
        protected string useKind;
        protected string assignedTo;
        protected string value;
        protected string operation;
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
        public string Operation { get { return operation; } }
        public List<UsedVariableSchema> VariablesList { get { return variablesList; } }
        protected void AddVariable(UsedVariableSchema v) { variablesList.Add(v); }
        public UsedVariableSchema() { }

        protected void LoadBinaryOperation(SyntaxNode root, SemanticModel model)
        {
            UsedVariableSchema left = new UsedVariableSchema();
            UsedVariableSchema right = new UsedVariableSchema();
            var m = (model.GetOperation(root) as IBinaryOperation);
            left.LoadInternal(m.LeftOperand.Syntax, model, this);
            right.LoadInternal(m.RightOperand.Syntax, model, this);
            AddVariable(left);
            AddVariable(right);
        }
        protected void LoadInternal(SyntaxNode root, SemanticModel model, UsedVariableSchema v)
        {
            kind = VarKind.Use.ToString();
            assignedTo = v.AssignedTo;
            line = v.Line;
            type = model.GetTypeInfo(root).Type?.ToString();
            useKind = model.GetOperation(root)?.Kind.ToString();
            operation = root.Kind().ToString();
            _ = (model.GetOperation(root)?.Kind is OperationKind.Literal) ? value = root.ToString() : name = root.ToString();
            if (model.GetOperation(root)?.Kind is OperationKind.Binary)  LoadBinaryOperation(root, model);
        }
        public void LoadMe(SyntaxNode root, SemanticModel model)
        {
            kind = VarKind.Use.ToString();
            AssignmentExpressionSyntax exp = root as AssignmentExpressionSyntax;
            assignedTo = exp.Left.ToString();
            line = exp.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = model.GetTypeInfo(exp).Type.ToString();
            useKind = model.GetOperation(exp.Right)?.Kind.ToString();
            operation = exp.Kind().ToString();
            _ = (model.GetOperation(exp.Right)?.Kind is OperationKind.Literal) ? value = exp.Right.ToString() : name = exp.Right.ToString();
            if (model.GetOperation(exp.Right)?.Kind is OperationKind.Binary) LoadBinaryOperation(exp.Right, model);
        }
    }

    /// <summary>
    /// Class representing the Variable of kind Assignment, inherit from VariableSchema. 
    /// Informations gathered: Assignment Kind, Assignment
    /// </summary>
    [Serializable]
    class AssignedVariableSchema : VariableSchema
    {
        protected string assignmentKind;
        protected string assignment;
        protected string variableKind;
        protected string fullName;
        [JsonIgnore]
        public override VariableSchema Variable { get { return variable; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Assignment { get { return assignment; } }
        public string Kind { get { return kind; } }
        public int AssignmentLine { get { return line; } }
        public string VariableKind { get { return variableKind; } }
        public string FullName { get { return fullName; } }
        public string AssignmentKind { get { return assignmentKind; } }
        public AssignedVariableSchema() { }

        public void LoadMe(SyntaxNode root, SemanticModel model)
        {
            kind = VarKind.Assignment.ToString();
            AssignmentExpressionSyntax exp = root as AssignmentExpressionSyntax;
            assignmentKind = exp.Kind().ToString();
            name = exp.Left.ToString();
            line = exp.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = model.GetTypeInfo(exp.Left).Type?.ToString();
            variableKind = model.GetSymbolInfo(exp.Left).Symbol?.Kind.ToString();
            fullName = model.GetSymbolInfo(exp.Left).Symbol?.ToString();
            assignment = exp.Right.ToString();
        }
    }

    /// <summary>
    /// Class representing the Variable of kind Definition, inherit from VariableSchema. 
    /// Informations gathered: Assignment
    /// </summary>
    [Serializable]
    class DeclaredVariableSchema : VariableSchema
    {
        protected string assignment;
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
            kind = VarKind.Definition.ToString();
            VariableDeclaratorSyntax variable = v as VariableDeclaratorSyntax;
            if (variable.Initializer != null) assignment = variable.Initializer.Value.ToString();
            name = variable.Identifier.ToString();
            line = variable.GetLocation().GetLineSpan().StartLinePosition.Line;
            type = (root as VariableDeclarationSyntax).Type.ToString();
        }
    }

}


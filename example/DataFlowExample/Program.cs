using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HelloWorld
{
    class Program
    {
        
        static void Main(string[] args)
        {

            string programText = File.ReadAllText("Ex\\HelloWorld.cs");

            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            List<MethodDeclarationSyntax> methodList = new List<MethodDeclarationSyntax>();
            List<LiteralExpressionSyntax> literalList = new List<LiteralExpressionSyntax>();

            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create("DataFlowEx",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);

            Console.WriteLine("DataFlow Initialized");

            List<MethodDeclarationSyntax> methods = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            
            foreach(var m in methods)
            {
                BlockSyntax statements = m.Body;
                DataFlowAnalysis dataFlow = model.AnalyzeDataFlow(statements);

                Console.WriteLine("All written variables inside the method");
                foreach (var v in dataFlow.WrittenInside) Console.WriteLine(v.Name);
                Console.WriteLine("All written variables outside the method");
                foreach (var v in dataFlow.WrittenOutside) Console.WriteLine(v.Name);
                // se non ci sono parametri del metodo stampa this
                Console.WriteLine("All variables Name Inside");
                foreach (var v in dataFlow.ReadInside) Console.WriteLine(v.Name);

                Console.WriteLine("All Statements");
                List<StatementSyntax> stat = m.DescendantNodes().OfType<StatementSyntax>().ToList();
                foreach (var v in stat) Console.WriteLine(v);

                Console.WriteLine("All Declarations");
                List<VariableDeclarationSyntax> variables = m.DescendantNodes().OfType<VariableDeclarationSyntax>().ToList();
                foreach (var v in variables) Console.WriteLine(v);

                Console.WriteLine("All Assignments");
                List<AssignmentExpressionSyntax> assign = m.DescendantNodes().OfType<AssignmentExpressionSyntax>().ToList();
                foreach (var v in assign) Console.WriteLine(v);

                Console.WriteLine("All Invocations");
                List<InvocationExpressionSyntax> invoc = m.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();
                foreach (var v in invoc) Console.WriteLine(v);

                Console.WriteLine("\n");
            }           

        }
    }
}

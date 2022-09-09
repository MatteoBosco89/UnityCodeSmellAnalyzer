using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection.Metadata.Ecma335;

namespace HelloWorld
{
    class Program
    {
        const string programText =
            @"using System;
            using System.Collections;
            using System.Linq;
            using System.Text;

            namespace HelloWorld
            {
                class Program
                {
                    static void Main(string[] args)
                    {
                        int c;
                        c = 1;
                        int a = 1;
                        int b = 2;
                        a = c;
                        c = 3;
                        b = c;
                        s = a;
                        t = 0;
                        Console.WriteLine(""Hello, World!"");
                    }

                    public void Stampa(){
                                        Console.WriteLine();}

                    public string Method1(){ int var1 = 10; }




                }
            }";
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            List<MethodDeclarationSyntax> methodList = new List<MethodDeclarationSyntax>();
            List<LiteralExpressionSyntax> literalList = new List<LiteralExpressionSyntax>();


            List<ClassDeclarationSyntax> classList = (from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration).ToList();
            foreach (ClassDeclarationSyntax s in classList)
            {
                methodList.AddRange((from MethodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>() select MethodDeclaration).ToList());


            }

            foreach (MethodDeclarationSyntax m in methodList)
            {
                Console.WriteLine(m.Identifier + " " + m.ReturnType + " " + m.Modifiers.ToString());

            }

            foreach (MethodDeclarationSyntax m in methodList)
            {
                literalList.AddRange((from LiteralDeclaration in m.DescendantNodes().OfType<LiteralExpressionSyntax>() select LiteralDeclaration).ToList());
            }

            Console.WriteLine("Literals");
            foreach (LiteralExpressionSyntax s in literalList)
            {
                Console.WriteLine(s.Kind());
                Console.WriteLine(s.Token);

            }


            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create("DataFlowEx",
                syntaxTrees: new[] { tree }, references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);

            MethodDeclarationSyntax methods = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            Console.WriteLine(methods.Body);
            BlockSyntax statements = methods.Body;
            DataFlowAnalysis dataFlow = model.AnalyzeDataFlow(statements);

            // per leggere il metodo nel data flow bisogna passare il body
            // bisogna checkare se il Body è null (penso null per body vuoto) oppure no, però se prendiamo sempre solo
            // body pieni (ergo, facciamo ricerca di metodi con body vuoti e quindi sappiamo i pieni) possiamo skippare
            // il passaggio, però rimane il warning, si può provare a wrappare in un Try/Catch
            // ci mette un panico di tempo
            Console.WriteLine(dataFlow.VariablesDeclared.Length);


            foreach (var v in dataFlow.AlwaysAssigned) // con alwaysassigned le variabili che sono sempre definite (int a = 1)
            {
                Console.WriteLine(v.Kind);

                Console.WriteLine(v.Name);
                Console.WriteLine(v.OriginalDefinition);

                Console.WriteLine(v.ContainingType);
                Console.WriteLine(v.ContainingSymbol);
                Console.WriteLine(v.CanBeReferencedByName);
                Console.WriteLine(v.DeclaringSyntaxReferences.First().SyntaxTree);
            }



        }
    }
}

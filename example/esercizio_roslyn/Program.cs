using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
                        Console.WriteLine(""Hello, World!"");
                    }

                    public void Stampa(){
                                        Console.WriteLine();}

                    public string Method1(){}




                }
            }";
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            List<MethodDeclarationSyntax> methodList = new List<MethodDeclarationSyntax>();
            Console.WriteLine("ciao");


            List<ClassDeclarationSyntax> classList = (from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration).ToList();
            foreach(ClassDeclarationSyntax s in classList)
            {
                methodList.AddRange((from MethodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>() select MethodDeclaration).ToList());


            }

            foreach(MethodDeclarationSyntax m in methodList)
            {
                Console.WriteLine(m.Identifier + " " + m.ReturnType  + " " + m.Modifiers.ToString());
            }

         


        }
    }
}
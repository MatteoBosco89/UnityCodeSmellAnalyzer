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
        public static List<string> nonLiterals = new List<string> { "0", "", "null", "0.0f" };
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
                        Method1();
                        Method2();
                        int x = 50;
                        y = 0;
                    }
                    static protected int Method1(string s)
                    {
                        Console.WriteLine(""Hello, World!"");
                        s = """";
                    }
                    protected float Method2(string s)
                    {
                        string s = ""Hello"";
                        Method2(); 
                        x = null;
                    }
                    static protected void Method3(string s)
                    {
                        Program.Method2();
                    }
                    int Methods4(string s){
                        
                    }
                }
            }";
        /*
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            //Get al the classes inside the compilation unit
            List<MethodDeclarationSyntax> methods = new List<MethodDeclarationSyntax>();
            List<ClassDeclarationSyntax> classes = (from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration).ToList();

            //Methods inside all all classes
            foreach (ClassDeclarationSyntax c in classes)
            {
                Console.WriteLine($"Class Identifier: {c.Identifier}");
                List<MethodDeclarationSyntax> temp = (from methodDeclaration in c.DescendantNodes().OfType<MethodDeclarationSyntax>() select methodDeclaration).ToList();
                methods.AddRange(temp);
            }
            PrintMethods("Methods signature:", methods);//methods signature

            foreach (ClassDeclarationSyntax c in classes)
            {
                PrintMethods("Empty Body Methods:", EmptyMethods(c));//get empty methods for each class
                Literals(c);//get all literals 
            }

            foreach (MethodDeclarationSyntax m in methods) MethodsCalls(m);//get all methods call for each method

        }*/

        protected static List<MethodDeclarationSyntax> EmptyMethods(ClassDeclarationSyntax c)
        {
            return (from methods in c.DescendantNodes().OfType<MethodDeclarationSyntax>() where methods.Body.Statements.Count <= 0 select methods).ToList();
        }

        protected static void MethodsCalls(MethodDeclarationSyntax m)
        {
            Console.WriteLine($"Methods invocation");
            List<InvocationExpressionSyntax> list = (from invocationExpressions in m.DescendantNodes().OfType<InvocationExpressionSyntax>() select invocationExpressions).ToList();
            foreach (InvocationExpressionSyntax md in list)
            {
                MethodDeclarationSyntax met = (from parents in md.Ancestors().OfType<MethodDeclarationSyntax>() select parents).FirstOrDefault();
                Console.WriteLine($"\tCalled from: {met.Identifier}");
                Console.WriteLine($"\t\t{md}");
            }
        }

        protected static void Literals(ClassDeclarationSyntax c)
        {
            Console.WriteLine($"Litterals");
            List<LiteralExpressionSyntax> literals = (from literalsExpression in c.DescendantNodes().OfType<LiteralExpressionSyntax>() select literalsExpression).ToList();
            foreach (LiteralExpressionSyntax l in literals)
            {
                MethodDeclarationSyntax met = (from parents in l.Ancestors().OfType<MethodDeclarationSyntax>() select parents).FirstOrDefault();
                Console.WriteLine($"\tCalled from: {met.Identifier}");
                string s = l.Token.ToString().Replace("\"", "");
                if (!nonLiterals.Contains(s)) Console.WriteLine($"\t\t{s}");
            }
        }

        protected static void PrintMethods(string s, List<MethodDeclarationSyntax> list)
        {
            Console.WriteLine(s);
            foreach (MethodDeclarationSyntax m in list)
            {
                ClassDeclarationSyntax c = (ClassDeclarationSyntax)m.Parent;
                Console.WriteLine($"\tClass: {c.Identifier}");
                Console.WriteLine($"\t\t{m.AttributeLists} {m.Modifiers} {m.ReturnType} {m.Identifier} {m.ParameterList}");
            }
        }
    }
}
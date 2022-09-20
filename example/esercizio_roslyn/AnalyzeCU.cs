using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

public interface C
{

}

namespace AnalyzerCU
{

    namespace Ana1
    {
        using System.CodeDom;

        class Ana1 { }

        namespace Ana2
        {
            using System.Buffers;

            class Ana2 { }

            namespace Ana3
            {
                using System.Collections.Concurrent;

                interface ICiaone
                {
                    string Name { get; set; }

                    void Method1() { }
                    void Method2() { }
                }

                [Serializable]
                class Ana3 { }
            }

        }
    }

    public class AnalyzeCU
    {
        public static List<string> nonLiterals = new List<string> { "0", "", "null", "0.0f" };
        protected static List<SyntaxTree> compilationUnits = new List<SyntaxTree>();
        static MetadataReference Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        protected static string path = "D:\\progetti\\UnityCodeSmellAnalyzer\\example\\esercizio_roslyn\\classi";

        static void Main(string[] args)
        {

            List<string> ext = new List<string> { "cs" };
            List<string> myFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant())).ToList();
            Console.WriteLine(myFiles.Count);

            foreach(string file in myFiles)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                compilationUnits.Add(tree);
            }

            foreach (SyntaxTree s in compilationUnits)
            {
                Console.WriteLine("\nAnalisi Classe");
                
                CompilationUnitSyntax root = s.GetCompilationUnitRoot();
                CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees: new[] { compilationUnits[0], compilationUnits[1] }, references: new[] { Mscorlib });
                SemanticModel model = compilation.GetSemanticModel(s);
                //Get al the classes inside the compilation unit
                List<MethodDeclarationSyntax> methods = new List<MethodDeclarationSyntax>();
                List<ClassDeclarationSyntax> classes = (from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration).ToList();
                List<InterfaceDeclarationSyntax> interfaces = (from interfaceDeclaration in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select interfaceDeclaration).ToList();
                

                foreach(InterfaceDeclarationSyntax i in interfaces)
                {
                    Console.WriteLine(i.AttributeLists.ToString());
                    Console.WriteLine(i.Identifier);
                    Console.WriteLine(i.BaseList);
                }

                //Methods inside all all classes
                foreach (ClassDeclarationSyntax c in classes)
                {
                    Console.WriteLine($"{c.Identifier}");
                    foreach(var cc in c.AttributeLists)
                    {
                        foreach (var ccc in cc.Attributes) Console.WriteLine(ccc.Name.ToString());
                    }

                    List<MethodDeclarationSyntax> temp = (from methodDeclaration in c.DescendantNodes().OfType<MethodDeclarationSyntax>() select methodDeclaration).ToList();
                    methods.AddRange(temp);
                    List<PropertyDeclarationSyntax> properties = (from propertyDeclaration in c.DescendantNodes().OfType<PropertyDeclarationSyntax>() select propertyDeclaration).ToList();
                    Console.WriteLine("PROPERTIES:");
                    foreach(PropertyDeclarationSyntax property in properties)
                    {
                        Console.WriteLine(property.Identifier);
                        Console.WriteLine(property.Modifiers);
                        Console.WriteLine(property.Type);

                    }
                }
                PrintMethods("Methods signature:", methods);//methods signature

                foreach (ClassDeclarationSyntax c in classes)
                {
                    PrintMethods("Empty Body Methods:", EmptyMethods(c));//get empty methods for each class
                    Literals(c);//get all literals 
                }

                foreach (MethodDeclarationSyntax m in methods) MethodsCalls(m, model);//get all methods call for each method
            }


        }

        protected static List<MethodDeclarationSyntax> EmptyMethods(ClassDeclarationSyntax c)
        {
            return (from methods in c.DescendantNodes().OfType<MethodDeclarationSyntax>() where methods.Body.Statements.Count <= 0 select methods).ToList();
        }

        protected static void MethodsCalls(MethodDeclarationSyntax m, SemanticModel model)
        {
            Console.WriteLine($"Methods invocation");
            List<InvocationExpressionSyntax> list = (from invocationExpressions in m.DescendantNodes().OfType<InvocationExpressionSyntax>() select invocationExpressions).ToList();
            foreach (InvocationExpressionSyntax md in list)
            {
                MethodDeclarationSyntax met = (from parents in md.Ancestors().OfType<MethodDeclarationSyntax>() select parents).FirstOrDefault();
                Console.WriteLine($"\tCalled from: {met.Identifier}");
                Console.WriteLine($"\t\t{md}");
                Console.WriteLine(md.Ancestors().OfType<ClassDeclarationSyntax>().First().Identifier);
                Console.WriteLine("---SemanticModel---");
                var symbolType = model.GetTypeInfo(md);
                var invokedSymbol = model.GetSymbolInfo(md).Symbol;
                Console.WriteLine(model.GetAliasInfo(md));
                if (invokedSymbol == null) continue;
                if (invokedSymbol.ContainingAssembly == null) Console.WriteLine("NULL");
                else Console.WriteLine(invokedSymbol.ContainingAssembly); 
                Console.WriteLine(invokedSymbol.ContainingSymbol); 
                Console.WriteLine(invokedSymbol.ContainingSymbol.Name);
                Console.WriteLine(symbolType.ToString());
                Console.WriteLine(symbolType.Type);
            }
        }

        protected static void Literals(ClassDeclarationSyntax c)
        {
            Console.WriteLine($"Literals");
            List<LiteralExpressionSyntax> literals = (from literalsExpression in c.DescendantNodes().OfType<LiteralExpressionSyntax>() select literalsExpression).ToList();
            foreach (LiteralExpressionSyntax l in literals)
            {
                MethodDeclarationSyntax met = (from parents in l.Ancestors().OfType<MethodDeclarationSyntax>() select parents).FirstOrDefault();
                if (met == null) return;
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
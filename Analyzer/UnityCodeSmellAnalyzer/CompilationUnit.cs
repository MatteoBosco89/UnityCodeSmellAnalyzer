using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class CompilationUnit
    {
        protected string name;
        protected List<UsingSchema> usings = new List<UsingSchema>();
        protected List<NamespaceSchema> namespaces = new List<NamespaceSchema>();
        protected List<InterfaceSchema> interfaces = new List<InterfaceSchema>();
        protected List<ClassSchema> classes = new List<ClassSchema>();


        public string Name
        {
            get { return name; }
        }

        public List<InterfaceSchema> Interfaces { get { return interfaces; } }
        public List<NamespaceSchema> Namespaces { get { return namespaces; } }
        public List<ClassSchema> Classes { get { return classes; } }
        public List<UsingSchema> Usings { get { return usings; } }

        public CompilationUnit(string name)
        {
            this.name = name;
        }

        public void AddInterface(InterfaceSchema i)
        {
            interfaces.Add(i);
        }

        public void AddNamespace(NamespaceSchema n)
        {
            namespaces.Add(n);
        }

        public void AddClass(ClassSchema c)
        {
            classes.Add(c);
        }

        public void AddUsing(UsingSchema u)
        {
            usings.Add(u);
        }

        public void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            List<UsingDirectiveSyntax> udsl = (from directives in root.DescendantNodes().OfType<UsingDirectiveSyntax>() select directives).ToList();
            List<NamespaceDeclarationSyntax> ndsl = (from namespaces in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>() select namespaces).ToList();
            List<InterfaceDeclarationSyntax> idsl = (from interfaces in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select interfaces).ToList();
            List<ClassDeclarationSyntax> cdsl = (from classes in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classes).ToList();

            foreach(UsingDirectiveSyntax u in udsl)  AddUsing(new UsingSchema(u.Name.ToString(), u.GetLocation().GetLineSpan().StartLinePosition.Line));
            
            foreach(NamespaceDeclarationSyntax n in ndsl)
            {
                NamespaceSchema ns = new NamespaceSchema(n.Name.ToString(), n.GetLocation().GetLineSpan().StartLinePosition.Line);
                ns.LoadInformations(n, model);
                AddNamespace(ns);
            }

            foreach(InterfaceDeclarationSyntax i in idsl)
            {
                if(i.Parent == root)
                {
                    InterfaceSchema ins = new InterfaceSchema(i.Identifier.ToString(), i.GetLocation().GetLineSpan().StartLinePosition.Line);
                    ins.LoadInformations(i, model);
                    AddInterface(ins);
                }
                
            }

            foreach(ClassDeclarationSyntax c in cdsl)
            {
                if (c.Parent == root)
                {
                    string inh;
                    ITypeSymbol its = model.GetDeclaredSymbol(c);
                    inh = its.BaseType.Name;
                    List<INamedTypeSymbol> interf = its.Interfaces.ToList();
                    List<string> interfs = new List<string>();
                    foreach (INamedTypeSymbol t in interf) interfs.Add(t.Name);
                    ClassSchema cs = new ClassSchema(c.Identifier.ToString(), c.GetLocation().GetLineSpan().StartLinePosition.Line, inh, interfs);
                    cs.LoadInformations(c, model);
                    AddClass(cs);
                }
            }

        }

    }
}



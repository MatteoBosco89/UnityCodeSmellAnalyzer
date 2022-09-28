using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class CompilationUnit : SyntaxSchema
    {
        protected string name;
        protected string fileName;
        protected List<UsingSchema> usings = new List<UsingSchema>();
        protected List<NamespaceSchema> namespaces = new List<NamespaceSchema>();
        protected List<InterfaceSchema> interfaces = new List<InterfaceSchema>();
        protected List<ClassSchema> classes = new List<ClassSchema>();


        public string Name
        {
            get { return name; }
        }
        public string FileName
        {
            get { return fileName; }
        }

        public List<InterfaceSchema> Interfaces { get { return interfaces; } }
        public List<NamespaceSchema> Namespaces { get { return namespaces; } }
        public List<ClassSchema> Classes { get { return classes; } }
        public List<UsingSchema> Usings { get { return usings; } }


        public CompilationUnit(string name, string fileName)
        {
            this.name = name;
            this.fileName = fileName;
        }

        public CompilationUnit() { }

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

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            line = root.GetLocation().GetLineSpan().StartLinePosition.Line;

            List<UsingDirectiveSyntax> udsl = (from directives in root.DescendantNodes().OfType<UsingDirectiveSyntax>() select directives).ToList();
            List<NamespaceDeclarationSyntax> ndsl = (from namespaces in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>() select namespaces).ToList();
            List<InterfaceDeclarationSyntax> idsl = (from interfaces in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select interfaces).ToList();
            List<ClassDeclarationSyntax> cdsl = (from classes in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classes).ToList();

            foreach(UsingDirectiveSyntax u in udsl)
            {
                UsingSchema us = new UsingSchema();
                us.LoadInformations(u, model);
                AddUsing(us);
            }
            
            foreach(NamespaceDeclarationSyntax n in ndsl)
            {
                NamespaceSchema ns = new NamespaceSchema();
                ns.LoadInformations(n, model);
                AddNamespace(ns);
            }

            foreach(InterfaceDeclarationSyntax i in idsl)
            {
                if(i.Parent == root)
                {
                    InterfaceSchema ins = new InterfaceSchema();
                    ins.LoadInformations(i, model);
                    AddInterface(ins);
                }
                
            }

            foreach(ClassDeclarationSyntax c in cdsl)
            {
                if (c.Parent == root)
                {
                    ClassSchema cs = new ClassSchema();
                    cs.LoadInformations(c, model);
                    AddClass(cs);
                }
            }

        }
    }
}



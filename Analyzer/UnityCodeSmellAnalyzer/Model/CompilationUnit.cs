using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCodeSmellAnalyzer
{
    /// <summary>
    /// Class representing the Compilation Unit.
    /// Informations gathered: Name, Path, Usings, Namespaces, Interfaces, Classes, LOC
    /// </summary>
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
        /// <summary>
        /// Loads directly children Namespaces of the CompilationUnit/Namespace
        /// </summary>
        /// <param name="root">The direct parent</param>
        /// <param name="ndsl">List of Namespaces found</param>
        /// <param name="model">The model</param>
        protected void LoadNamespaces(SyntaxNode root, List<NamespaceDeclarationSyntax> ndsl, SemanticModel model)
        {
            foreach (NamespaceDeclarationSyntax n in ndsl)
            {
                if(n.Parent == root)
                {
                    NamespaceSchema ns = new NamespaceSchema();
                    ns.LoadInformations(n, model);
                    AddNamespace(ns);
                }
            }
        }
        /// <summary>
        /// Loads directly children Using of the CompilationUnit/Namespace
        /// </summary>
        /// <param name="root">The direct parent</param>
        /// <param name="udsl">List of Usings found</param>
        /// <param name="model">The model</param>
        protected void LoadUsings(SyntaxNode root, List<UsingDirectiveSyntax> udsl, SemanticModel model)
        {
            foreach (UsingDirectiveSyntax u in udsl)
            {
                if(u.Parent == root)
                {
                    UsingSchema us = new UsingSchema();
                    us.LoadInformations(u, model);
                    AddUsing(us);
                }
            }
        }
        /// <summary>
        /// Loads directly children Interfaces of the CompilationUnit/Namespace
        /// </summary>
        /// <param name="root">The direct parent</param>
        /// <param name="idsl">List of Interfaces found</param>
        /// <param name="model">The model</param>
        protected void LoadInterfaces(SyntaxNode root, List<InterfaceDeclarationSyntax> idsl, SemanticModel model)
        {
            foreach (InterfaceDeclarationSyntax i in idsl)
            {
                if (i.Parent == root)
                {
                    InterfaceSchema ins = new InterfaceSchema();
                    ins.LoadInformations(i, model);
                    AddInterface(ins);
                }

            }
        }
        /// <summary>
        /// Loads directly children Classes of the CompilationUnit/Namespace
        /// </summary>
        /// <param name="root">The direct parent</param>
        /// <param name="idsl">List of Classes found</param>
        /// <param name="model">The model</param>
        protected void LoadClasses(SyntaxNode root, List<ClassDeclarationSyntax> cdsl, SemanticModel model)
        {
            foreach (ClassDeclarationSyntax c in cdsl)
            {
                if (c.Parent == root)
                {
                    ClassSchema cs = new ClassSchema();
                    cs.LoadInformations(c, model);
                    AddClass(cs);
                }
            }
        }

        public override void LoadInformations(SyntaxNode root, SemanticModel model)
        {
            LoadBasicInformations(root, model);

            List<UsingDirectiveSyntax> udsl = (from directives in root.DescendantNodes().OfType<UsingDirectiveSyntax>() select directives).ToList();
            List<NamespaceDeclarationSyntax> ndsl = (from namespaces in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>() select namespaces).ToList();
            List<InterfaceDeclarationSyntax> idsl = (from interfaces in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>() select interfaces).ToList();
            List<ClassDeclarationSyntax> cdsl = (from classes in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classes).ToList();

            LoadUsings(root, udsl, model);
            LoadNamespaces(root, ndsl, model);
            LoadInterfaces(root, idsl, model);
            LoadClasses(root, cdsl, model);
        }

        public override void LoadBasicInformations(SyntaxNode root, SemanticModel model)
        {
            line = root.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}



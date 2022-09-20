using System;
using System.Collections.Generic;


namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class CompilationUnit
    {

        protected string name;
        protected List<InterfaceSchema> interfaces = new List<InterfaceSchema>();
        protected List<NamespaceSchema> namespaces = new List<NamespaceSchema>();
        protected List<ClassSchema> classes = new List<ClassSchema>();
        protected List<UsingSchema> usings = new List<UsingSchema>();


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

    }
}



﻿using System;
using System.Collections.Generic;

namespace UnityCodeSmellAnalyzer
{
    [Serializable]
    public class InterfaceSchema
    {
        protected List<MethodSchema> methods = new List<MethodSchema>();
        protected List<PropertySchema> properties = new List<PropertySchema>();
        protected List<string> attributes = new List<string>();
        protected string name;
        protected int line;
        protected string modifier;

        public InterfaceSchema(string name, int line, string modifier)
        {
            this.name = name;
            this.line = line;
            this.modifier = modifier;
        }
        public void AddMethod(MethodSchema m)
        {
            methods.Add(m);
        }
        public void AddProperty(PropertySchema p)
        {
            properties.Add(p);
        }
        public void AddAttribute(string a)
        {
            attributes.Add(a);
        }

    }
}



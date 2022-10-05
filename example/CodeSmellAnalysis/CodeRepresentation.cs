using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeSmellFinder
{
    public class MethodReference
    {
        protected string fullName;
        protected int line;

        public string FullName { get { return fullName; } }
        public int Line { get { return line; } }
        public MethodReference(string fullName, int line)
        {
            this.fullName = fullName;
            this.line = line;
        }
    }

    public class UsingReference
    {
        protected string fullName;
        protected int line;

        public string FullName { get { return fullName; } }
        public int Line { get { return line; } }
        public UsingReference(string fullName, int line)
        {
            this.fullName = fullName;
            this.line = line;
        }
    }
}
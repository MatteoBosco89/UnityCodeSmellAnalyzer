using System;

namespace Element
{
    public class Element
    {
        public Element() { }

        virtual public void PrintElement() { }
        virtual public int LoadNormalDictionary(string[] lines, int i) { return 0; }
        virtual public void LoadParenthesisDictionary(string line) { }
    }
}


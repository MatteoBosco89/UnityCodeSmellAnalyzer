using System;
using System.Collections.Generic;

namespace Element
{
    public class DictionaryElement : Element
    {
        protected Dictionary<string, Element> values;
        
        public Dictionary<string, Element> Values { get { return values; } }
        public DictionaryElement(Dictionary<string, Element> new_values)
        {
            values = new_values;    
        }
    }
}


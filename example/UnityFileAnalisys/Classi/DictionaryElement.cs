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
        public DictionaryElement() { }

        public int LoadNormalDictionary(string[] lines, int i)
        {
            values = new Dictionary<string, Element>();
            int ind_block = NumOfSpaces(lines[i]);
            for (; i < lines.Length; i++)
            {
                if (NumOfSpaces(lines[i]) - ind_block <= 0) break;
                string[] vals = lines[i].Split(':');
                if (vals[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i++;
                    i = d.LoadNormalDictionary(lines, i);
                    values.Add(vals[0], d);
                }
                else if (lines[i].Contains("{"))
                {
                    DictionaryElement d = new DictionaryElement();
                    d.LoadParenthesisDictionary(lines[i]);
                    values.Add(vals[0].Trim(), d);
                }
                else
                {
                    SimpleElement e = new SimpleElement(vals[1].Trim());
                    values.Add(vals[0].Trim(), e);
                }
                
            }
            return i;
        }

        public void LoadParenthesisDictionary(string line)
        {
            string d = line.Split('}')[0];
            d = d.Split('{')[1];
            values = new Dictionary<string, Element>();
            string[] s = d.Split(',');
            foreach (string i in s)
            {
                string[] elements = i.Split(':');
                values[elements[0].Trim()] = new SimpleElement(elements[1].Trim());
            }
        }

        private int NumOfSpaces(string line)
        {
            int i = 0;
            foreach(char c in line)
            {
                if (c == ' ') i++;
                else break;
            }
            return i;
        }
    }
}


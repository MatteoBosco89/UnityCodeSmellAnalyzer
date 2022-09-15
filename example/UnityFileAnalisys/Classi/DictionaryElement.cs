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

        public int LoadNormalDictionary(string[] lines, int i, string end_line)
        {
            values = new Dictionary<string, Element>();
            int ind_block = NumOfSpaces(lines[i]);
            for (; i < lines.Length; i++)
            {
                if ((NumOfSpaces(lines[i]) - ind_block <= 0)&& ind_block !=0 || lines[i].Contains(end_line)) break;
                string[] vals = lines[i].Split(':');
                Console.WriteLine(vals[0]);
                if (vals[1].Length <= 1)
                {
                    Console.WriteLine("Dizionario normale");
                    DictionaryElement d = new DictionaryElement();
                    i++;
                    i = d.LoadNormalDictionary(lines, i, end_line);
                    values.Add(vals[0], d);
                }
                else if (lines[i].Contains("{"))
                {
                    Console.WriteLine("Dizionario parentesi");
                    DictionaryElement d = new DictionaryElement();
                    d.LoadParenthesisDictionary(lines[i]);
                    if(values.ContainsKey(vals[0].Trim()))values.Add(vals[0].Trim(), d);
                }
                else
                {
                    Console.WriteLine("Simple element");
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

        override public void PrintElement()
        {
            foreach(KeyValuePair<string, Element> kvp in values)
            {
                Console.WriteLine("\t" + kvp.Key);
                if(kvp.Value is SimpleElement)
                {
                    SimpleElement e = (SimpleElement)kvp.Value;
                    Console.WriteLine("\t\t" + e.Value);
                }
                if(kvp.Value is DictionaryElement)
                {
                    DictionaryElement d = (DictionaryElement)kvp.Value;
                    d.PrintElement();
                }
            }
        }
    }
}


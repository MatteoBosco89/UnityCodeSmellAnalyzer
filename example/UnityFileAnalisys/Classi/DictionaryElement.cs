using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Element
{
    public class DictionaryElement : Element
    {
        protected Dictionary<string, Element> values;
        protected string type;
        
        public Dictionary<string, Element> Values { get { return values; } }
        public string Type { get { return type; } }
        
        public DictionaryElement() { }

        override public int LoadNormalDictionary(string[] lines, int i)
        {
            values = new Dictionary<string, Element>();
            type = lines[i].Split(':')[0].Trim();
            i++;
            int indent = NumOfSpaces(lines[i]);
            int j = 0;
            for (; i < lines.Length; i++)
            {
                
                int num_spaces = NumOfSpaces(lines[i]) - indent;

                if (num_spaces < 0)return i;
                if (lines[i].Trim().Length <= 1) return i;
                
                string[] vals = lines[i].Split(':');
                Element d;
                try 
                {
                    if (vals[1].Length <= 1)
                    {
                        if (i + 1 < lines.Length)
                        {
                            if (NumOfSpaces(lines[i + 1]) - indent > 0)
                            {
                                d = new DictionaryElement();
                                i = d.LoadNormalDictionary(lines, i);
                                i--;
                            }
                            else
                                d = new SimpleElement("");
                        }
                        else
                            d = new SimpleElement("");
                    }
                    else if (vals[1].Contains("{"))
                    {
                        if (!vals[1].Contains("{}"))
                        {
                            d = new DictionaryElement();
                            d.LoadParenthesisDictionary(lines[i]);
                        }
                        else
                            d = new SimpleElement("");
                    }
                    else
                    {
                        d = new SimpleElement(vals[1].Trim());
                    }
                    string key = vals[0].Trim();
                    if (values.ContainsKey(key))
                    {
                        j++;
                        key = key + j;
                    }
                    else j = 0;

                    values.Add(key, d);
                } 
                catch (IndexOutOfRangeException) {
                    return i;
                }
            }
            return i;
        }

        override public void LoadParenthesisDictionary(string line)
        {
            type = line.Split(':')[0].Trim();
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
                if (c == ' ' || c == '-') i++;
                else break;
            }
            return i;
        }

        override public void PrintElement()
        {
            Console.WriteLine(type);
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

        public JObject ToJson(JObject jo)
        {
            JArray ja = new JArray();
            foreach(KeyValuePair<string, Element> kvp in values)
            {
                JObject j = new JObject();
                if (kvp.Value is SimpleElement)
                {
                    SimpleElement s = (SimpleElement)kvp.Value;
                    j.Add(kvp.Key, s.Value);
                }
                if(kvp.Value is DictionaryElement)
                {
                    DictionaryElement d = (DictionaryElement)kvp.Value;
                    j = d.ToJson(j);
                }
                ja.Add(j);
            }
            jo.Add(type, ja);
            return jo;
        }
    }
}


using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element
{
    /*
     * DictionaryElement represents the sub dictionary inside the unity file 
     */
    public class DictionaryElement : Element
    {
        protected Dictionary<string, Element> values;
        protected string type;
        
        public Dictionary<string, Element> Values { get { return values; } }
        public string Type { get { return type; } }
        
        public DictionaryElement() { }

        /*
         * This method load the unity dictionary using the indentation of the file
         */
        override public int LoadNormalDictionary(string[] lines, int i, string cmpId)
        {
            values = new Dictionary<string, Element>();
            type = lines[i].Split(':')[0].Trim(); //read the type of the subdictionary from the first line passed
            i++;
            int indent = NumOfSpaces(lines[i]);//mesure the number of indentation spaces presents
            int j = 0;
            for (; i < lines.Length; i++)
            {
                int num_spaces = NumOfSpaces(lines[i]) - indent; //calculate the indentation of the current line in respect of the line containing the type of the subdictionary

                if (num_spaces < 0)return i; //if the current line doesn't have the same indentation of the subdictionary block return
                if (lines[i].Trim().Length <= 1) return i; //if the line is empty return
                if (lines[i].Contains(cmpId)) return i;
                string[] vals = lines[i].Split(':'); //split the line using the ':' separator
                                                    // the first element of vals is the name of the element inside the subdictionary
                                                    // the second element is the value
                Element d;
                try 
                {
                    if(vals.Length == 1)// verify if the dictionary doesn't contain key value pair inside
                    {
                        d = new DictionaryElement();
                        i = d.LoadSpecialDictionary(lines, i);
                        i--;
                    }
                    else if (vals[1].Length <= 1)//if the second element of the line is empty it means that we have another sub dictionary or an empty value
                    {
                        if (i + 1 < lines.Length)//if the next line doesn't have the same indentation that means we have a sub dictionary else we have an empty element
                        {
                            if (NumOfSpaces(lines[i + 1]) - indent > 0)
                            {
                                d = new DictionaryElement();
                                i = d.LoadNormalDictionary(lines, i, cmpId);
                                i--;
                            }
                            else
                                d = new SimpleElement("");
                        }
                        else
                            d = new SimpleElement("");
                    }
                    else if (vals[1].Contains("{"))//if the second element inside the line contains a { we have a sub dictionary with inside the {}
                    {
                        if (!vals[1].Contains("{}"))//if the second element contains only the two bracket it means that we have an empty element
                        {
                            d = new DictionaryElement();
                            d.LoadParenthesisDictionary(lines[i]);
                        }
                        else
                            d = new SimpleElement("");
                    }else if (vals[1].Contains("'"))
                    {
                        string s = vals[1].Trim().Replace("'", "");
                        i++;
                        bool exit = false;
                        for (; i < lines.Length && !exit; i++)
                        {
                            if (lines[i].Contains("'"))exit = true;
                            s +=lines[i].Trim().Replace("'", "");
                        }
                        d = new SimpleElement(s);
                    }else if (vals[1].Contains(","))
                    {
                        string s = vals[1].Trim();
                        i++;
                        for (; i < lines.Length && !lines[i].Contains(":"); i++)
                        {
                            s += lines[i].Trim();
                        }
                        i--;
                        d = new SimpleElement(s);
                    }
                    else
                    {
                        d = new SimpleElement(vals[1].Trim());
                    }
                    string key = vals[0].Trim();//because inside the unity file some element inside the dictionary have the same name a progressiv index is added to avoid conflicts
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
        /*
         * This method is used to load a sub dictionary contained inside {}
         */
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

        public override int LoadSpecialDictionary(string[] lines, int i)
        {
            values = new Dictionary<string, Element>();
            type = lines[i].Split(':')[0].Trim(); //read the type of the subdictionary from the first line passed
            i++;
            int indent = NumOfSpaces(lines[i]);
            int j = 0;
            for(; i < lines.Length; i++)
            {
                int num_spaces = NumOfSpaces(lines[i]) - indent;
                if (num_spaces < 0) return i;
                if (lines[i].Trim()[0] != '-') return i;
                SimpleElement e = new SimpleElement(lines[i].Trim());
                values.Add(j.ToString(), e);
                j++;
            }
            return i;
        }

        /*
         * This methods mesure the indentation of a line
         */
        private int NumOfSpaces(string line)
        {
            int i = 0;
            foreach(char c in line)
            {
                if (c == ' ' || c == '-') i++;//in some file the indentation is made also by '-'
                else break;
            }
            return i;
        }

        /*
         * Print the object
         */
        override public void Print()
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
                    d.Print();
                }
            }
        }

        /*
         * Rapresent the Dictionary as a Json Object
         */
        public JObject ToJson(JObject jo)
        {
            JArray ja = new JArray();
            foreach(KeyValuePair<string, Element> kvp in values)
            {
                JObject j = new JObject();
                if (kvp.Value is SimpleElement)
                {
                    SimpleElement s = (SimpleElement)kvp.Value;
                    string str = Regex.Replace(kvp.Key, @"[\d-]", string.Empty);//remove the charater added to the key of the dictionary to avoid conflicts
                    j.Add(str, s.Value);
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


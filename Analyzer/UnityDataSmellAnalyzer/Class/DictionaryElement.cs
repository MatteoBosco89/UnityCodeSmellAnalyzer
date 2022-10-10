using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element
{
    /// <summary>
    /// Dictionary element represents the sub dictionary inside the unity meta data 
    /// </summary>
    public class DictionaryElement : Element
    {
        protected Dictionary<string, Element> values;
        protected string type;
        
        public Dictionary<string, Element> Values { get { return values; } }
        public string Type { get { return type; } }
        
        public DictionaryElement() { }

        /// <summary>
        /// Load a standar dictionary inside the unity meta data file. The standard dictionary is is made up of an indented sequence of elements, 
        /// each of which can itself be a dictionary
        /// </summary>
        /// <param name="lines">The array of string containing the lines of the unity data file</param>
        /// <param name="i">The current index inside the array of lines</param>
        /// <param name="cmpId">The cmpId string indicating the line containing the id of the component</param>
        /// <returns>The last index visited inside the array of string</returns>
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
                                //some time the element of the sub dictionary are - {fileID: 8600129312555473672}
                                string[] temp = lines[i + 1].Split('{');
                                if (!temp[0].Contains(":"))
                                {
                                    d = new DictionaryElement();
                                    i = d.LoadDictionaryWithSpecialElements(lines, i, cmpId);
                                    i--;
                                }
                                else
                                {
                                    d = new DictionaryElement();
                                    i = d.LoadNormalDictionary(lines, i, cmpId);
                                    i--;
                                }
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
        /// <summary>
        /// This method load a sub dictionary inside the unity data contained within the curly brackets on a single line
        /// </summary>
        /// <param name="line">The line that contains the dictionary</param>
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
        /// <summary>
        /// This method load a dictionay with a special format
        /// </summary>
        /// <param name="lines">The array of string containing the lines inside the unity data file</param>
        /// <param name="i">The current index inside the array of string</param>
        /// <returns>The last index visited inside the array of string</returns>
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
        /// <summary>
        /// Load dictionary with a special format inside the element
        /// </summary>
        /// <param name="lines">The array of string that contain the lines of the unity data file</param>
        /// <param name="i">The current index inside the array of string</param>
        /// <param name="cmpId">The special string representig the line containig the id of the component</param>
        /// <returns>The last visited index inside the array of string</returns>
        override public int LoadDictionaryWithSpecialElements(string[] lines, int i, string cmpId)
        {
            values = new Dictionary<string, Element>();
            type = lines[i].Split(':')[0].Trim(); //read the type of the subdictionary from the first line passed
            i++;
            int indent = NumOfSpaces(lines[i]);
            int k = 0;
            for(; i < lines.Length; i++){
                if (NumOfSpaces(lines[i]) - indent < 0) return i;
                if (lines[i].Contains(cmpId)) return i;
                string d = lines[i].Split('}')[0];
                d = d.Split('{')[1];
                string[] s = d.Split(',');
                foreach (string r in s)
                {
                    string[] elements = r.Split(':');
                    values[elements[0].Trim()+k] = new SimpleElement(elements[1].Trim());
                    k++;
                }

            }
            return i;
        }
        /// <summary>
        /// Measure the indentation of a given string (the indentation is made by spaces and -)
        /// </summary>
        /// <param name="line">The line in witch you need to count the indentation</param>
        /// <returns>The indentation of the given line</returns>
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

        /// <summary>
        /// Print the dictionary
        /// </summary>
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
        /// <summary>
        /// Convert the dictionary in to a JObject
        /// </summary>
        /// <param name="jo">The parent json object</param>
        /// <returns>The modified json object</returns>
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


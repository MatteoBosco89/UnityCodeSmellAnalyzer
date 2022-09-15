using System;
using System.Collections.Generic;
using Element;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityData
{
    public class Prefab
    {
        protected string id;
        protected string name;
        protected string comp_id = "--- !u!";
        protected string spec_str = "%";
        protected Dictionary<string, Element.Element> components;
        public Prefab(string[] lines)
        {
            for(int i = 0; i < lines.Length; i++ )
            {
                Console.Write(i + "\t");
                string cid = "";
                if (lines[i].Contains(spec_str)) continue;
                if (lines[i].Contains(comp_id))
                {
                    cid = lines[i].Split('&')[1];
                    i++;
                }
                if (lines[i].Split(':')[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i++;
                    i = d.LoadNormalDictionary(lines, i);
                } 
            }
        }
    }
}


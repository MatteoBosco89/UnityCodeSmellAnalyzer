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

            components = new Dictionary<string, Element.Element>();
            for(int i = 0; i < lines.Length; i++ )
            {
                string cid = "";
                if (lines[i].Contains(spec_str)) continue;
                if (lines[i].Contains(comp_id))
                {
                    cid = lines[i].Split('&')[1];
                    i++;
                }
                DictionaryElement d = new DictionaryElement();
                i = d.LoadNormalDictionary(lines, i, comp_id);
                Console.WriteLine(cid);
                if(cid != "")components.Add(cid, d);
            }
        }

        public void PrintPrefab()
        {
            foreach(KeyValuePair<string, Element.Element> kvp in components)
            {
                Console.WriteLine("id: " + kvp.Key);
                kvp.Value.PrintElement();
            }
        }
    }
}


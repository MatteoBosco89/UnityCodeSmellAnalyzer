using System;
using System.Collections.Generic;
using Element;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityData
{
    public class Prefab
    {
        protected string id;
        protected string name;
        protected string COMP_ID = "--- !u!";
        protected string SPEC_STR = "%";
        protected string GUID = "guid";
        protected Dictionary<string, Element.Element> components;
        public Prefab(string mainFile, string metaFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(mainFile);
                string[] metaLines = File.ReadAllLines(metaFile);
                name = Path.GetFileNameWithoutExtension(mainFile);
                LoadMainData(lines);
                LoadMetaData(metaLines);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No file found");
            }
        }

        private void LoadMainData(string[] lines)
        {
            components = new Dictionary<string, Element.Element>();
            for (int i = 0; i < lines.Length; i++)
            {
                string cid = "";
                if (lines[i].Contains(SPEC_STR)) continue;

                Console.WriteLine(lines[i]);
                if (lines[i].Contains(COMP_ID))
                {
                    cid = lines[i].Split('&')[1];
                    i++;
                }
                if (lines[i].Split(':')[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i = d.LoadNormalDictionary(lines, i);
                    i--;
                    components.Add(cid, d);
                }

            }
        }

        private void LoadMetaData(string[] lines)
        {
            foreach(string line in lines)
            {
                if (line.Contains(GUID))
                {
                    id = line.Split(':')[1].Trim();
                    return;
                }
            }
        }

        public void PrintPrefab()
        {
            Console.WriteLine(name);
            Console.WriteLine(id);
            foreach(KeyValuePair<string, Element.Element> kvp in components)
            {
                Console.WriteLine("id: " + kvp.Key);
                kvp.Value.PrintElement();
            }
        }
    }
}


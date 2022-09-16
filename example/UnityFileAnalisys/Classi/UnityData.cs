using System;
using System.Collections.Generic;
using Element;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace UnityAnalyzer
{
    public class UnityData
    {
        protected string id;
        protected string name;
        protected string COMP_ID = "--- !u!";
        protected string SPEC_STR = "%";
        protected string GUID = "guid";
        protected Dictionary<string, Element.Element> components;
        public UnityData(string mainFile, string metaFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(mainFile);
                string[] metaLines = File.ReadAllLines(metaFile);
                name = Path.GetFileNameWithoutExtension(mainFile);
                LoadMainData(lines);
                LoadId(metaLines);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No file found");
            }
        }

        public UnityData(string metaFile)
        {
            try
            {
                string[] metaLines = File.ReadAllLines(metaFile);
                name = Path.GetFileNameWithoutExtension(metaFile);
                LoadMetaFile(metaLines);
            }
            catch (FileNotFoundException)
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

        private void LoadMetaFile(string[] lines)
        {
            components = new Dictionary<string, Element.Element>();
            for(int i = 1; i < lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
                if (lines[i].Contains(GUID))
                {
                    id = lines[i].Split(':')[1].Trim();
                    i++;
                }
                string[] vals = lines[i].Split(':');
                if (vals[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i = d.LoadNormalDictionary(lines, i);
                    components.Add(vals[0].Trim(), d);
                }

            }
        }

        private void LoadId(string[] lines)
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


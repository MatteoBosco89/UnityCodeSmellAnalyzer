using System;
using System.Collections.Generic;
using Element;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityAnalyzer
{
    public class UnityData
    {
        protected string id;
        protected string name;
        protected string type;
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
                type = Path.GetExtension(mainFile).Trim('.');
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
                type = Path.GetExtension(metaFile).Trim('.');
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
            int j = 0;
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
                    if (components.ContainsKey(cid))
                    {
                        j++;
                        cid += i;
                    }
                    else j = 0;
                    components.Add(cid, d);
                }

            }
        }

        private void LoadMetaFile(string[] lines)
        {
            components = new Dictionary<string, Element.Element>();
            for(int i = 1; i < lines.Length; i++)
            {
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

        public JObject ToJsonObject()
        {
            JObject json = new JObject();
            json.Add("guid", id);
            json.Add("name", name);
            json.Add("type", type);
            JArray ja = new JArray();
            foreach(KeyValuePair<string, Element.Element> kvp in components)
            {
                if (kvp.Value is DictionaryElement)
                {
                    DictionaryElement d = (DictionaryElement)kvp.Value;
                    JObject jo = new JObject();
                    jo.Add("id", kvp.Key);
                    jo = d.ToJson(jo);
                    ja.Add(jo);

                }
            }
            json.Add("COMPONENTS", ja);
            return json;
        }

        public void SaveDataToJsonFile(string filename)
        {
            File.WriteAllText(filename, ToJsonObject().ToString());
        }
    }
}


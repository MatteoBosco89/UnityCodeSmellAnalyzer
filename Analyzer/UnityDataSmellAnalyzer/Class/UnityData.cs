using System;
using System.Collections.Generic;
using Element;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Text.RegularExpressions;

namespace UnityAnalyzer
{
    public class UnityData
    {
        protected string id;
        protected string name;
        protected string type;
        protected string COMP_ID = "--- !u!";//special substring indicating the id of a component
        protected string SPEC_STR = "%";//substring indicating the line that aren't needed
        protected string GUID = "guid";//special substring indicating the id of a unity object
        protected Dictionary<string, Element.Element> components;

        /*
         * This constructor is used for those file in witch the mainfile (.prefab, .controller ecc...) contains the information and
         * the metafile only contains the guid of the object
         */
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
        /*
         * This constructor is used to load the data from oject stored on meta file
         */
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
        /*
         * Load the data from the mainfile(.prefab, .unity, .controller ecc...)
         */
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
                //Console.WriteLine("maindata"+lines[i]);
                if (lines[i].Split(':')[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i = d.LoadNormalDictionary(lines, i, COMP_ID);
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

        /*
         * Load the data from the meta file
         */
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
                    i = d.LoadNormalDictionary(lines, i, COMP_ID);
                    components.Add(vals[0].Trim(), d);
                }
            }
        }
        /*
         * Load the id from the meta file
         */
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

        /*
         * Print the UnityData Object
         */
        public void Print()
        {
            Console.WriteLine(name);
            Console.WriteLine(id);
            foreach(KeyValuePair<string, Element.Element> kvp in components)
            {
                Console.WriteLine("id: " + kvp.Key);
                kvp.Value.Print();
            }
        }
        /*
         * Convert the DataObject in json object
         */
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
                    string s = kvp.Key;
                    //remove the charater added to the key of the dictionary to avoid conflicts
                    //if(id == d.Type)s = Regex.Replace(kvp.Key, @"[\d-]", string.Empty);
                    jo.Add("id", s);
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


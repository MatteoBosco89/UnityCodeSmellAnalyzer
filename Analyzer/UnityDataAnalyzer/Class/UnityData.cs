using System;
using System.Collections.Generic;
using Element;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityAnalyzer
{
    /// <summary>
    /// This class represents the information inside a unity meta data files.
    /// Every unity meta data file is made by a main dictionary with other sub dictionary inside
    /// </summary>
    [Serializable]
    public class UnityData
    {
        protected string id;
        protected string name;
        protected string type;
        protected string filePath;
        protected long fileSize = 0; // the file size in byte
        protected const string COMP_ID = "--- !u!";//special substring indicating the id of a component
        protected const string SPEC_STR = "%";//substring indicating the line that aren't needed
        protected const string GUID = "guid";//special substring indicating the id of a unity object
        protected Dictionary<string, Element.Element> components;
        protected int numComponents = 0;

        public string Id { get { return id; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string FilePath { get { return filePath; } }
        public long FileSize { get { return fileSize; } }
        public int NumComponents { get { return numComponents; } }
        public Dictionary<string, Element.Element> COMPONENTS { get { return components; } }


        public UnityData(string id, string name, string type)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.filePath = "No path";
            this.fileSize = 0;
            this.components = new Dictionary<string, Element.Element>();
            this.numComponents = 0;
        }
        /// <summary>
        /// This constructor is used to load the main data from a unity project (suc as .unity files, .prefab ecc...)
        /// For the files the associated .meta is needed because it contains the GUID of that specific object inside the project
        /// </summary>
        /// <param name="mainFile">The path to the main file</param>
        /// <param name="metaFile">The path to the associated meta file</param>
        public UnityData(string mainFile, string metaFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(mainFile);
                string[] metaLines = File.ReadAllLines(metaFile);
                name = Path.GetFileNameWithoutExtension(mainFile);
                type = Path.GetExtension(mainFile).Trim('.');
                fileSize = (new FileInfo(mainFile)).Length;
                filePath = mainFile;
                LoadMainData(lines);
                LoadId(metaLines);
                numComponents = components.Count;
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No file found");
            }
        }
        /// <summary>
        /// This constructor is used to load only the assets witch information are stored inside the .meta files (such as .png, .texture ecc...)
        /// </summary>
        /// <param name="metaFile">The path to the meta file</param>
        public UnityData(string metaFile)
        {
            try
            {
                string[] metaLines = File.ReadAllLines(metaFile);
                name = Path.GetFileNameWithoutExtension(metaFile);
                type = Path.GetExtension(metaFile).Trim('.');
                fileSize = (new FileInfo(metaFile)).Length;
                filePath = metaFile;
                LoadMetaFile(metaLines);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No file found");
            }
        }
        /// <summary>
        /// This method load the information from the main data file in unity, such as .unity, .prefab ecc...)
        /// </summary>
        /// <param name="lines">The array of string containing the content of the main file</param>
        private void LoadMainData(string[] lines)
        {
            components = new Dictionary<string, Element.Element>();
            int j = 0;
            if (lines[0].Contains("\u0004")) return;
            if (!lines[0].Contains(SPEC_STR)) return;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[0].Contains("\u0004")) return;
                string cid = "";
                if (lines[i].Contains(SPEC_STR)) continue;
                
                if (lines[i].Contains(COMP_ID))
                {
                    cid = lines[i].Split('&')[1];
                    
                    i++;
                }
                if (lines[i].Split(':').Length <= 1) continue;
                if (lines[i].Split(':')[1].Length <= 1)
                {
                    //Console.WriteLine("if " + lines[i]);
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

        /// <summary>
        /// This method load the information from the meta data file in unity
        /// </summary>
        /// <param name="lines">The array of string containing the content of the meta data</param>
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
                if (i == lines.Length) continue;
                string[] vals = lines[i].Split(':');
                if (vals.Length < 2) continue;
                if (vals[1].Length <= 1)
                {
                    DictionaryElement d = new DictionaryElement();
                    i = d.LoadNormalDictionary(lines, i, COMP_ID);
                    int k = 0;
                    string keyName = vals[0].Trim();
                    if (components.ContainsKey(keyName))
                    {
                        while (components.ContainsKey(keyName+k)) k++;
                        keyName += k;
                    }
                    components.Add(keyName, d);
                    
                }
            }
        }
      /// <summary>
      /// Load the GUID from the meta file
      /// </summary>
      /// <param name="lines">The array of string containing the content of the meta file</param>
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

        /// <summary>
        /// print the content of the unity data object
        /// </summary>
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
        /// <summary>
        /// Convert the unity data object in a json object
        /// </summary>
        /// <returns>The JObject representing the unity data object</returns>
        public JObject ToJsonObject()
        {
            JObject json = new JObject();
            json.Add("guid", id);
            json.Add("file_path", filePath);
            json.Add("name", name);
            json.Add("type", type);
            json.Add("file_size", fileSize);
            json.Add("num_components", numComponents);
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
    }
}


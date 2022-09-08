using System;
using System.Collections.Generic;
using System.IO;

namespace UnityAnalyzer
{
    public class UnityPrefab
    {
        private string id = "";
        private const string end_line = "--- !u!";
        private Dictionary<string, DataObject> components = new Dictionary<string, DataObject>();
        
        public string Id { get { return id; } }
        public Dictionary<string, DataObject> Components { get { return components; } }
        public UnityPrefab(){}

        public void LoadFile(string filepath)
        { 
            string[] lines = File.ReadAllLines(filepath); 
            for (int i = 2; i < lines.Length; i++)
            {
                string id = lines[i].Substring(lines[2].LastIndexOf('&') + 1);
                i++;
                string c_name = lines[i].Replace(":", "");
                DataObject d = new DataObject(c_name, id);
                i = d.LoadData(lines, i+1, end_line) - 1;
                components[id] = d;
            }
        }
        public void LoadMeta(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            id = lines[1].Split(':')[1].Trim();
        }

        static void Main(string[] args)
        {
            UnityPrefab instance = new UnityPrefab();
            instance.LoadMeta("FilesToLoad\\Cube.prefab.meta");
            instance.LoadFile("FilesToLoad\\Cube.prefab");
            Console.WriteLine("GUID " + instance.Id);
            foreach(KeyValuePair<string, DataObject> k in instance.Components)
            {
                //Console.WriteLine(k.Key);
                k.Value.PrintData();
            }
        }
    }

}

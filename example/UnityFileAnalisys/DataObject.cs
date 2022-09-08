using System;
using System.Collections.Generic;

namespace UnityAnalyzer
{   
    public class DataObject
    {
        private Dictionary<string, DataRaw> data;
        private string name;
        private string id;

        public string Name { get { return name; } }
        public string Id { get { return id; } }
        public Dictionary<string, DataRaw> Data { get { return data; } }

        public DataObject(string c_name, string c_id)
        {
            name = c_name;
            id = c_id;
            data = new Dictionary<string, DataRaw>();
        }

        public int LoadData(string[] lines, int i, string end_line)
        {
            int j = 0;
            for(; i < lines.Length && !lines[i].Contains(end_line); i++)
            {
                string[] elements = lines[i].Split(':');
                string e_name = elements[0].Trim();
                DataRaw dr = new DataRaw();
                if (lines[i].Contains("{"))
                {
                    string d = lines[i].Split('}')[0];
                    d = d.Split('{')[1];
                    dr.AddData(d);
                }
                else
                {
                    dr.Element = elements[1].Trim();
                }
                if (data.ContainsKey(e_name)) 
                {
                    j++;
                    e_name = e_name + j.ToString();
                }
                else j = 0;
                data[e_name] = dr;
            }
           
            return i;
        }

        public void PrintData()
        {
            Console.WriteLine(name+ " " + id);
            foreach(KeyValuePair<string, DataRaw> k in data)
            {
                Console.Write(k.Key);
                k.Value.PrintData();
            }
        }
        public class DataRaw
        {
            private Dictionary<string, string> rawData;
            private string element = "";
            
            public string Element { get { return element; } set { element = value; } }
            public Dictionary<string, string> RawData { get { return rawData; }}
            public DataRaw() 
            {
                rawData = new Dictionary<string, string>();
            }

            public void AddData(string s)
            {
                string[] d = s.Split(',');
                foreach(string i in d)
                {
                    string[] elements = i.Split(':');
                    rawData[elements[0].Trim()] = elements[1].Trim();
                }
            }

            public void PrintData()
            {
                Console.WriteLine("\t"+element);
                foreach (KeyValuePair<string, string> k in rawData) Console.WriteLine("\t\t"+k.Key + " " + k.Value);
            }
        }
    }
    
}


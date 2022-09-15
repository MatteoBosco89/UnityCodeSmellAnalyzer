using System;
using System.Collections.Generic;
using System.Data;

namespace UnityAnalyzer
{   
    /*
     * Il DataObject rappresenta le informazioni di un elemento all'interno di un prefab (le informazioni delle componenti)
     * è costituito da un dizionario di RowData, un oggetto che rappresenta i paramentri associato ad ogni proprietà della componente
     */
    public class DataObject
    {
        private Dictionary<string, RowData> data;
        private string name;//nome della componente
        private string id;//id interno al prefab della componente

        public string Name { get { return name; } }
        public string Id { get { return id; } }
        public Dictionary<string, RowData> Data { get { return data; } }

        public DataObject(string c_name, string c_id)
        {
            name = c_name;
            id = c_id;
            data = new Dictionary<string, RowData>();
        }

        //estrae dal file le informazioni per creare il dizionario data
        public int LoadData(string[] lines, int i, string end_line)
        {
            int j = 0;
            for(; i < lines.Length && !lines[i].Contains(end_line); i++)
            {
                string[] elements = lines[i].Split(':');
                string e_name = elements[0].Trim();
                RowData dr = new RowData();
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
            foreach(KeyValuePair<string, RowData> k in data)
            {
                Console.Write(k.Key);
                k.Value.PrintData();
            }
        }
        
    }

    /*
     * Rappresenta la lista di parametri di una proprietà di una componente del prefab
     * contiene un dizionario string-string che rappresenta i parametri della proprietà
     */
    public class RowData
    {
        private Dictionary<string, string> rowData;
        private string element = "";

        public string Element { get { return element; } set { element = value; } }
        public Dictionary<string, string> RawData { get { return rowData; } }
        public RowData()
        {
            rowData = new Dictionary<string, string>();
        }

        public void AddData(string s)
        {
            string[] d = s.Split(',');
            foreach (string i in d)
            {
                string[] elements = i.Split(':');
                rowData[elements[0].Trim()] = elements[1].Trim();
            }
        }

        public void PrintData()
        {
            Console.WriteLine("\t" + element);
            foreach (KeyValuePair<string, string> k in rowData) Console.WriteLine("\t\t" + k.Key + " " + k.Value);
        }

        //ricerca un parametro specifico nel dizionario
        public string FindParameters(string p_name)
        {
            if (!rowData.ContainsKey(p_name)) return "";
            return rowData[p_name];
        }
    }

}


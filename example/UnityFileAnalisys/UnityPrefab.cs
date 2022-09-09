using System;
using System.Collections.Generic;
using System.IO;

namespace UnityAnalyzer
{
    /*
     * La classe UnityPrefab rappresenta l'oggetto prefab e le sue caratteristiche
     * All'interno è presente un dizionario contenente le componenti del prefab
     * 
     */
    public class UnityPrefab
    {
        private string id = "";
        private string name = "";
        private const string id_string = "--- !u!"; //identifica la striga che rappresenta l'id delle componenti
        private Dictionary<string, DataObject> components = new Dictionary<string, DataObject>();
        
        public string Id { get { return id; } }
        public string Name { get { return name; } } //il nome è estratto dal file .prefab
        public Dictionary<string, DataObject> Components { get { return components; } }
        public UnityPrefab(string name)
        {
            this.name = name;
        }

        //legge il file .prefab
        public void LoadFile(string filepath)
        { 
            string[] lines = File.ReadAllLines(filepath); 
            for (int i = 2; i < lines.Length; i++)
            {
                string id = lines[i].Substring(lines[2].LastIndexOf('&') + 1);
                i++;
                string c_name = lines[i].Replace(":", "");
                DataObject d = new DataObject(c_name, id);
                i = d.LoadData(lines, i+1, id_string) - 1;
                components[id] = d;
            }
        }
        //legge il file .meta per estrarre l'GUID del prefab
        public void LoadMeta(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            id = lines[1].Split(':')[1].Trim();
        }

        static void Main(string[] args)
        {
            //carichiamo due prefab, cube e sphere
            UnityPrefab cube = new UnityPrefab("Cube");
            cube.LoadMeta("FilesToLoad\\Cube.prefab.meta");
            cube.LoadFile("FilesToLoad\\Cube.prefab");
            UnityPrefab sphere = new UnityPrefab("Sphere");
            sphere.LoadMeta("FilesToLoad\\Sphere.prefab.meta");
            sphere.LoadFile("FilesToLoad\\Sphere.prefab");

            //creiamo una lista di prefab (quando verrà analizzato tutto il progetto avremo tutta la lista dei prefab del progetto)
            List<UnityPrefab> prefabs = new List<UnityPrefab>();
            prefabs.Add(cube);
            prefabs.Add(sphere);

            //Console.WriteLine("GUID " + cube.Id);
            /*foreach(KeyValuePair<string, DataObject> k in instance.Components)
            {
                //Console.WriteLine(k.Key);
                k.Value.PrintData();
            }*/
            //Stampiamo le informaizoni di una delle componenti di Cube
            Console.WriteLine("numero di componenti: " + cube.Components.Count);
            DataObject d = cube.Components["1109823753433287060"];
            Console.WriteLine("Proprietà di: " + d.Id);
            d.PrintData();

            //ricerchiamo i riferimenti ad oggetti esterni (altri prefab) presenti in Cube
            //i riferimenti ad oggetti esterni sono sempre identificati dalla chiave guid
            Console.WriteLine("Estrazione dei riferimenti esterni dal Cube.prefab");
            List<string> refer = new List<string>(); 
            foreach(KeyValuePair<string, DataObject> c in cube.Components)
            {
                DataObject data = c.Value;
                foreach(RowData k  in data.Data.Values)
                {
                    string s = k.FindParameters("guid");
                    if (s != "") refer.Add(s);

                }
            }
            //andiamo a cercare i riferimenti a prefab esterni confrontando tutti i guid estratti con la
            //lista dei prefab caricati
            foreach(UnityPrefab up in prefabs)
            {
                if (refer.Contains(up.id)) Console.WriteLine("Riferimento ad oggetto esterno\n\tNome: " + up.Name + "\n\tId: " + up.Id);
            }
        }
    }

}

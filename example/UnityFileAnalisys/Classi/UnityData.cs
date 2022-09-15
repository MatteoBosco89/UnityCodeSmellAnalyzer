using System;
using System.Collections.Generic;
using Element;

namespace UnityData
{
    public class Prefab
    {
        protected string id;
        protected string name;
        protected string comp_id = "--- !u!";
        protected Dictionary<string, Element.Element> components;
        public Prefab(string[] lines)
        {
            for(int i = 1; i < lines.Length; i++ )
            {
                Console.Write(i + "\t");
                if (lines[i].Contains(comp_id))
                {
                    Console.WriteLine("Ho un id");
                }
                else if (lines[i].Split(':')[1].Length <= 1)
                {
                    Console.WriteLine("Ho un dizionario interno");
                }
                else if (lines[i].Contains("{"))
                {
                    Console.WriteLine("Ho un dizionario interno con le graffe");
                }
                else
                {
                    Console.WriteLine("Ho un valore");
                }
                
            }
        }
    }
}


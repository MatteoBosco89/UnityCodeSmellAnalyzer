using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeSmellFinder
{
    public static class Utility
    {
        public static string QueryString(string sop, string param, List<string> names, string compOp, string logicOp)
        {
            string query1 = "";

            for (int i = 0; i < names.Count; i++)
            {
                query1 += $"@{sop}{param} {compOp} '{names[i]}'";
                if (i < names.Count - 1)
                    query1 += $" {logicOp} ";
            }
            return query1;
        }

        public static bool ListInString(List<string> list, string s)
        {
            bool found = false;
            for (int i = 0; i < list.Count && !found; i++)
            {
                if (s.Contains(list[i])) found = true;
            }
            return found;
        }

        public static bool StringInList(List<string> list, string s)
        {
            bool found = false;

            for (int i = 0; i < list.Count && !found; i++)
            {
                if (list[i].Contains(s)) found = true;
            }
            return found;
        }

        public static bool ListInList(List<string> values, List<string> inList)
        {
            bool found = false;
            for (int i = 0; i < values.Count && !found; i++)
            {
                found = StringInList(inList, values[i]);
            }
            return found;
        }

        public static List<string> GetAllType(JArray data, List<string> names, string param)
        {
            List<string> types = new List<string>();
            foreach (string name in names)
            {
                types.AddRange(GetAllParamOfElement(data, name, param));
            }
            return types;
        }
        public static List<string> GetAllParamOfElement(JArray data, string element, string param)
        {
            List<string> results = new List<string>();
            var res = data.SelectTokens($"$..{element}");
            JArray arr = new JArray(res);
            foreach (JToken tokens in arr)
            {
                foreach (JToken token in tokens)
                {
                    if (token is JObject)
                    {
                        results.Add(token[param].ToString());
                    }
                }
            }
            return results;
        }

        public static List<UsingReference> FindUsing(JObject comUnit, List<string> lib)
        {
            List<UsingReference> results = new List<UsingReference>();
            var query = comUnit.SelectTokens($"$..Usings");
            JArray usings = new JArray(query);
            foreach (JToken token in usings.Values())
            {
                if (!Utility.ListInString(lib, token["Name"].ToString())) continue;
                results.Add(new UsingReference(token["Name"].ToString(), (int)token["Line"]));
            }
            return results;

        }

    }
}


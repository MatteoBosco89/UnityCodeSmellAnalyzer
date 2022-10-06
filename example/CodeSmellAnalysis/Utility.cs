using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

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
        public static List<string> ClassInheritance(JArray data, string param1, string param2, string param3, string inheritances)
        {
            List<string> classes = new List<string>();
            var res = data.SelectTokens($"$..{param1}");
            JArray results = new JArray(res);

            foreach (JToken c in results.Values())
            {
                if (c is JObject)
                {
                    string inh = c[param2].ToString();
                    if (inheritances.Contains(inh))
                    {
                        classes.Add(c[param3].ToString());
                    }
                }
            }
            return classes;
        }

        public static List<String> AllClassInheritances(JArray data, string inheritance)
        {
            List<string> baseInher = new List<string>();
            baseInher.AddRange(ClassInheritance(data, "Classes", "FullInheritanceName", "FullName", inheritance));
            baseInher = AllInheritances(data, baseInher);
            return baseInher;
        }

        public static List<string> AllInheritances(JArray data, List<string> baseInher)
        {
            List<string> allInheritances = new List<string>();
            allInheritances.AddRange(baseInher);
            for (int i = 0; i < baseInher.Count; i++)
            {
                var res = data.SelectTokens($"$..Classes[?(@.FullInheritanceName == '{baseInher[i]}')]");
                JArray results = new JArray(res);
                foreach (JToken cu in results)
                {
                    string name = cu["FullName"].ToString();
                    if(!allInheritances.Contains(name))allInheritances.Add(name);
                }
            }
            if (baseInher.Count < allInheritances.Count) AllInheritances(data, allInheritances);
            return allInheritances;
        }

        public static List<string> GetAllInvocationsOfClassInMethods(JObject cl, string methodName, string param)
        {
            List<string> invoks = new List<string>();

            var queryRes = cl.SelectTokens("$.Methods");
            JArray res = new JArray(queryRes);
            foreach (JToken m in res.Values())
            {
                invoks.Add(m[param].ToString());
            }
            JToken method = cl.SelectToken($"$.Methods[?(@.{param} == '{methodName}')]");
            queryRes = method.SelectTokens("$..Invocations");
            JArray results = new JArray(queryRes);
            List<string> invkres = new List<string>();
            foreach(JToken inv in results.Values())
            {
                if (ListInString(invoks, inv["FullName"].ToString())) invkres.Add(inv["FullName"].ToString());
            }

            return invkres;

        }
    }
    
}


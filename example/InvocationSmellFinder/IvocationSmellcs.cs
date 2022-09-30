using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace InvocationSmellFinder
{
    public static class InvocationSmell
    {
        public static void FindInvocationSmell(JArray data, List<string> methods, List<string> invocations)
        {
            Console.WriteLine("Analyzing...");
            //extract all the compilation units that exhibit the the requested invocations inside the specified methods
            JArray smells = new JArray();
            JArray res = DirectInvocation(data, methods, invocations);
            smells.Merge(res);
            Console.Write("\tExtracting Smell With Depth >= 1...");
            //anlyze the other invocation insiede the specified methods to found out if they contain the requested invocations
            foreach (JToken token in data)
            {
                JObject comUnit = new JObject();
                if (token is JObject @object) comUnit = @object;
                //get the invocations of the current compilation unit inside the specified methods
                var r = comUnit.SelectTokens($"$..Methods[?({QueryString("Name", methods, "==", "||")})].Invocations");
                JArray jArray = new JArray(r);
                //contains the methods already checked for the current compilation unit (to avoid recursive invocation)
                List<string> checkedMethods = new List<string>();
                //contains the methods to check for the current compiltion unit
                List<MethodReference> methodsToCheck = new List<MethodReference>();
                foreach (JToken inv in jArray.Values())
                {
                    string name = inv["Name"].ToString();
                    string fullName = inv["FullName"].ToString();
                    //Console.WriteLine(fullName);
                    if (!invocations.Contains(name))
                    {
                        var m = new MethodReference(fullName, (int)inv["Line"]);
                        methodsToCheck.Add(m);
                    }
                }
                //analyze to found the possible smell in depth > 1
                JArray depthResutl = SearchIvocation(comUnit, data, methodsToCheck, invocations, checkedMethods);
                if (depthResutl.Count() > 0)
                {
                    JObject j = new JObject();
                    j.Add("Script", comUnit["FileName"].ToString());
                    j.Add("Traceback", depthResutl);
                    smells.Add(j);
                }
            }
            Console.WriteLine("Done!");
            Console.WriteLine("Done!!");
            SaveResults(smells, invocations);
        }

        public static JArray DirectInvocation(JArray data, List<string> methods, List<string> invocations)
        {
            Console.Write("\tExtracting Direct Invocations...");
            JArray smells = new JArray();
            var res1 = data.SelectTokens($"$.[?(@..Methods[?({QueryString("Name", methods, "==", "||")})])]");
            JArray fresults = new JArray(res1);

            foreach (JToken met in fresults)
            {
                if (met is JObject)
                {
                    JObject obj = met as JObject;
                    var r = obj.SelectTokens($"$..Methods[?({QueryString("Name", methods, "==", "||")})].Invocations");
                    JArray invok = new JArray(r);
                    foreach (JToken inv in invok.Values())
                    {
                        if (invocations.Contains(inv["Name"].ToString()))
                        {
                            //Console.WriteLine("Found direct invocation");
                            JObject jo = new JObject();
                            jo.Add("Script", obj["FileName"].ToString());
                            jo.Add("Name", inv["Name"].ToString());
                            jo.Add("Invocation", inv["FullName"].ToString());
                            jo.Add("Line", inv["Line"]);
                            smells.Add(jo);
                        }
                    }
                }
            }
            Console.WriteLine("Done!");
            return smells;
        }

        public static JArray SearchIvocation(JObject parentCU, JArray data, List<MethodReference> methodsToCheck, List<string> methodsWithSmell, List<string> checkedMethods)
        {
            JArray results = new JArray();
            //new list of methods to check that will be passed at the recursive call of the function
            List<MethodReference> m_check = new List<MethodReference>();
            List<string> check_m = new List<string>();

            foreach (var method in methodsToCheck)
            {
                //to avoid recursive call and stack overflow, if the methods is already visited skip
                if (checkedMethods.Contains(method.FullName)) continue;
                check_m.Add(method.FullName);
                //get the compilation unit that contain the method to analyze
                var queryRes = data.SelectTokens($"$.[?(@..Methods[?(@.FullName == '{method.FullName}')])]");
                if (queryRes.Count() < 0) continue;
                JArray compUnit = new JArray(queryRes);
                //get the list invocations of the method to analyze
                queryRes = compUnit.SelectTokens($"$..Methods[?(@FullName == '{method.FullName}')].Invocations");
                JArray invocations = new JArray(queryRes);
                //if the method doesn't contain invocations skip
                if (invocations.Count() <= 0) continue;

                //analyze the invocation inside the method
                foreach (JToken inv in invocations.Values())
                {
                    string name = inv["Name"].ToString();
                    string fullName = inv["FullName"].ToString();
                    if (fullName == method.FullName) continue;

                    if (methodsWithSmell.Contains(name))
                    {
                        //Console.WriteLine("Found smell depth > 1");
                        JObject smell = new JObject();
                        smell.Add("Method", method.FullName);
                        smell.Add("LineCall", method.Line);
                        smell.Add("Script", compUnit.First()["FileName"].ToString());
                        smell.Add("Invocation", fullName);
                        smell.Add("Name", inv["Name"].ToString());
                        smell.Add("Line", inv["Line"]);
                        results.Add(smell);
                    }
                    else
                    {
                        var m = new MethodReference(fullName, (int)inv["Line"]);
                        m_check.Add(m);
                    }
                }
                JArray arr = SearchIvocation((JObject)compUnit.First(), data, m_check, methodsWithSmell, check_m);
                if (arr.Count() > 0)
                {
                    JObject j = new JObject();
                    j.Add("Invocation", method.FullName);
                    j.Add("Line", method.Line);
                    j.Add("Script", compUnit.First()["FileName"].ToString());
                    j.Add("Traceback", arr);
                    results.Add(j);
                }
            }
            return results;
        }
        public static string QueryString(string param, List<string> names, string compOp, string logicOp)
        {
            string query1 = "";

            for (int i = 0; i < names.Count; i++)
            {
                query1 += $"@.{param} {compOp} '{names[i]}'";
                if (i < names.Count - 1)
                    query1 += $" {logicOp} ";
            }
            return query1;
        }

        public static void SaveResults(JArray data, List<string> names)
        {
            string s = "";
            for (int i = 0; i < names.Count; i++)
            {
                s += names[i];
                if (i < names.Count - 1) s += "-";
            }
            s += ".json";
            if (s == "") Console.WriteLine("Error file name not valid");
            else
            {
                Console.Write("Saving to file...");
                File.WriteAllText(s, data.ToString());
                Console.WriteLine("Done!");
            }

        }

        public static string QueryCheckInArray(string param, List<string> names)
        {
            string s = "";
            s += $"@.{param} in [";
            for (int i = 0; i < names.Count; i++)
            {
                s += $"'{names[i]}'";
                if (i < names.Count - 1)
                    s += ",";
            }
            s += "]";
            return s;
        }
    }
    public class MethodReference
    {
        protected string fullName;
        protected int line;

        public string FullName { get { return fullName; } }
        public int Line { get { return line; } }
        public MethodReference(string fullName, int line)
        {
            this.fullName = fullName;
            this.line = line;
        }
    }
}

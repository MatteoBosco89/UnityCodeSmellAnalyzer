using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CodeSmellFinder
{
    /// <summary>
    /// Class containing utility methods for the SmellDetector
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Generate a string to use inside a Json Path query to verify if a parameter
        /// is equal to a list of values
        /// </summary>
        /// <param name="sop">Search parameter, . to access direct children, .. to do a depth search</param>
        /// <param name="param"> The name of the parameter to check</param>
        /// <param name="names"> The list of values to compare to the value of the given paramter</param>
        /// <param name="compOp"> The comparison operator (==, <=, >= , <, >, !=)</param>
        /// <param name="logicOp"> The logic operator (&&, ||)</param>
        /// <returns> The generated query string</returns>
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


        public static string QueryStringMultipleParms(string sop, List<string> param, List<string> names, string compOp, string logicOp)
        {
            string query1 = "";
            if (param.Count != names.Count)
                return null;
            for (int i = 0; i < names.Count; i++)
            {
                query1 += $"@{sop}{param[i]} {compOp} '{names[i]}'";
                if (i < names.Count - 1)
                    query1 += $" {logicOp} ";
            }
            return query1;
        }


        /// <summary>
        /// Search if one of the values inside the List is contained inside a a string
        /// </summary>
        /// <param name="list"> The list of value for the comparison</param>
        /// <param name="s">The string to search in to</param>
        /// <returns>True if one of the string inside the list i contained inside the string</returns>
        public static bool ListInString(List<string> list, string s)
        {
            bool found = false;
            for (int i = 0; i < list.Count && !found; i++)
            {
                if (s.Contains(list[i])) found = true;
            }
            return found;
        }
        /// <summary>
        /// Search if a list is contained inside the values of a list
        /// </summary>
        /// <param name="list"> The list to search in to</param>
        /// <param name="s">The string for the comaprison</param>
        /// <returns>True if the list is contained at least inside one of the value of the list</returns>
        public static bool StringInList(List<string> list, string s)
        {
            bool found = false;

            for (int i = 0; i < list.Count && !found; i++)
            {
                if (list[i].Contains(s)) found = true;
            }
            return found;
        }
        /// <summary>
        /// Search if one of the value inside a list is contained inside another list
        /// </summary>
        /// <param name="values">The list of values to search</param>
        /// <param name="inList">The list to search in to</param>
        /// <returns>True if one of the element of values is contained inside one of the element of inList</returns>
        public static bool ListInList(List<string> values, List<string> inList)
        {
            bool found = false;
            for (int i = 0; i < values.Count && !found; i++)
            {
                found = StringInList(inList, values[i]);
            }
            return found;
        }
        /// <summary>
        /// Return the name of all the classes/interfaces created inside the Unity Project
        /// </summary>
        /// <param name="data">The JArray containing the representation of the project code in Json format</param>
        /// <param name="names">The name of the element to get the value of the specified param</param>
        /// <param name="param">The Json element to extract the value from</param>
        /// <returns>The list of the names of the classes/interfaces</returns>
        public static List<string> GetAllType(JArray data, List<string> names, string param)
        {
            List<string> types = new List<string>();
            foreach (string name in names)
            {
                types.AddRange(GetAllParamOfElement(data, name, param));
            }
            return types;
        }
        /// <summary>
        /// Return all the value of a specified json element inside a json object
        /// </summary>
        /// <param name="data">The JArray containig the Dataset</param>
        /// <param name="element">The type of JObject to search</param>
        /// <param name="param">The Json element to extract the value from</param>
        /// <returns>The list of the values of the specified param for all Jobjects of type element</returns>
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
        /// <summary>
        /// Find all Using inside the Compilation Unit relative to the list of library
        /// </summary>
        /// <param name="comUnit">The compilation unit</param>
        /// <param name="lib">The list of library name to search inside the using of the compilation unit</param>
        /// <returns>A list of UsingReference found inside the compilation unit for the specified list of library</returns>
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

        /// <summary>
        /// Find Usings in a Compilation Unit belonging to different
        /// second level package.
        /// For example UnityEngine.Input and UnityEngine.UI are different
        /// whereas Unityengine.Input.Keyboard and UnityEngine.Input.Mouse are not
        /// </summary>
        /// <param name="comUnit">The compilation unit</param>
        /// <param name="lib">The list of library name to search inside the using of the compilation unit</param>
        /// <returns>A list of UsingReference found inside the compilation unit for the specified list of library</returns>
        public static List<UsingReference> FindUsingModule(JObject comUnit, List<string> lib)
        {
            List<UsingReference> results = new List<UsingReference>();
            Dictionary<string, int> allImports = new Dictionary<string, int>();

            var query = comUnit.SelectTokens($"$..Usings");
            JArray usings = new JArray(query);
            foreach (JToken token in usings.Values())
            {
                String[] moduleNameSpaces = token["Name"].ToString().Split('.');
                String module = token["Name"].ToString();

                if (moduleNameSpaces.Length > 1)

                    module = moduleNameSpaces[0] + "." + moduleNameSpaces[1];

                    if (!Utility.ListInString(lib, module)) continue;
                    if (allImports.ContainsKey(module)) continue;
                    allImports[module] = 1;
                    results.Add(new UsingReference(token["Name"].ToString(), (int)token["Line"]));
            }
            return results;

        }



        /// <summary>
        /// Return all the classes name that inherit the specified class
        /// </summary>
        /// <param name="data">The dataset to search in to</param>
        /// <param name="param1">The type of json object to search inside the dataset (Classes)</param>
        /// <param name="param2">The name of the jelement that contains the inheritance value of the jobject</param>
        /// <param name="param3">The value to extract from the class (Name or FullName) </param>
        /// <param name="inheritances">The specified inherited class</param>
        /// <returns>The list of the classes name that inherit the specified class</returns>
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
        /// <summary>
        /// Search all class that inherit a specified class with depth search
        /// </summary>
        /// <param name="data">The dataset containing the list of compilation unit</param>
        /// <param name="inheritance">The class to searh for inheritance</param>
        /// <returns>The list of the full name of the classes that inherit the specified class</returns>
        public static List<String> AllClassInheritances(JArray data, string inheritance)
        {
            List<string> baseInher = new List<string>();
            baseInher.AddRange(ClassInheritance(data, "Classes", "FullInheritanceName", "FullName", inheritance));
            baseInher = AllInheritances(data, baseInher);
            return baseInher;
        }
        /// <summary>
        /// Recursive method to serch for all classes that inherit a set of classes
        /// </summary>
        /// <param name="data">The dataset containing the list of compilation unit</param>
        /// <param name="baseInher">The list of classes to search for inheritance</param>
        /// <returns>The list of classes full name that inherit the specified set of classes</returns>
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
                    if (!allInheritances.Contains(name)) allInheritances.Add(name);
                }
            }
            if (baseInher.Count < allInheritances.Count) AllInheritances(data, allInheritances);
            return allInheritances;
        }
        /// <summary>
        /// Return all the invocation of the methods of the same class inside a method of the class
        /// </summary>
        /// <param name="cl">The Jobject representing the class</param>
        /// <param name="methodName">The name of the method where to search the invocation</param>
        /// <param name="param">The name of the parameter of the method name</param>
        /// <returns>The list of full name of the invocation inside the method</returns>
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
            foreach (JToken inv in results.Values())
            {
                if (ListInString(invoks, inv["FullName"].ToString())) invkres.Add(inv["FullName"].ToString());
            }

            return invkres;

        }
        /// <summary>
        /// Return the path of all the compilation unit of the project
        /// </summary>
        /// <param name="data">The dataset containing the list of compilation unit</param>
        /// <returns></returns>
        public static List<JToken> AllCompUnitFileName(JArray data)
        {
            List<JToken> compUni = new List<JToken>();
            foreach(JToken j in data)
            {
                compUni.Add(j["FileName"]);
            }
            return compUni;
        }
    }

}


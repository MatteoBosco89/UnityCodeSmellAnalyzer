using CSharpAnalyzer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CodeSmellFinder
{
    /// <summary>
    /// Class containing the methods to search element inside the dataset of the UnityProject
    /// </summary>
    public static class DataExtractor
    {
        /// <summary>
        /// Method to search all the Methods with specified param values
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="param">The param inside the method</param>
        /// <param name="methods">The values to compare the param to </param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray MethodsWithName(JArray data, string param, List<string> methods)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var queryRes = cu.SelectTokens($"$..Methods[?({Utility.QueryString(".", param, methods, "==", "||")})]");
                    JArray mets = new JArray(queryRes);
                    foreach (JToken m in mets)
                    {
                        JObject s = new JObject();
                        s.Add("Script", cu["FileName"]);
                        s.Add("Name", m["Name"]);
                        s.Add("Line", m["Line"]);
                        smells.Add(s);
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Method to search all the variable inside a method with specified parameters
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="param">The param of the method to search</param>
        /// <param name="methods">The values to compare to the param</param>
        /// <param name="param1">The first param of the variable to search</param>
        /// <param name="values">The values to compare to param1</param>
        /// <param name="param2">The second param of the variable to search</param>
        /// <param name="names">The values to compare to param2</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray VariableFromMethods(JArray data, string param, List<string> methods, string param1, List<string> values, string param2, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..Methods[?({Utility.QueryString(".", param, methods, "==", "||")})]");
                    JArray res = new JArray(query);

                    foreach (JToken met in res)
                    {
                        query = met.SelectTokens($"$..Variables[?({Utility.QueryString("..", param1, values, "==", "||")})]");
                        JArray variables = new JArray(query);
                        foreach (JToken var in variables.Values())
                        {
                            JObject j = new JObject(var);
                            JToken currVar = j["Variable"];
                            if (names != null)
                            {
                                string name = currVar[param2].ToString();

                                if (!Utility.ListInString(names, name)) continue;
                            }
                            JObject s = new JObject();
                            s.Add("Script", cu["FileName"].ToString());
                            string kind = currVar["Kind"].ToString();
                            s.Add("Kind", kind);
                            s.Add("Type", currVar["Type"].ToString());
                            s.Add("Line", currVar[kind + "Line"]);
                            smells.Add(s);
                        }
                    }

                }
            }
            return smells;
        }
        /// <summary>
        /// Method to search all the variable inside the project with specified values
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="types">The type of the variables to search</param>
        /// <param name="param">The first param of the variable to check</param>
        /// <param name="values">The values to compare to param1</param>
        /// <param name="param1">The second param of the variable to check</param>
        /// <param name="names">The values to compare to param2</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray VariablesFromData(JArray data, string typeParam, List<string> types, string param, List<string> values, string param1, List<string> names, bool searchType)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..Variables[?({Utility.QueryString("..", param, values, "==", "||")})]");
                    JArray res = new JArray(query);

                    foreach (JToken var in res.Values())
                    {
                        JObject j = new JObject(var);
                        JToken currVar = j["Variable"];
                        if (types != null)
                        {
                            string type = currVar[typeParam].ToString();
                            if (searchType)
                            {
                                if (!Utility.StringInList(types, type)) continue;
                            }
                            else
                            {
                                if (!Utility.ListInString(types, type)) continue;
                            }
                                
                        }
                        if (names != null)
                        {
                            string name = currVar[param1].ToString();
                            if (!Utility.ListInString(names, name)) continue;
                        }
                        JObject s = new JObject();
                        s.Add("Script", cu["FileName"].ToString());
                        string kind = currVar["Kind"].ToString();
                        s.Add("Kind", kind);
                        s.Add("Name", currVar["Name"]);
                        s.Add("Type", currVar["Type"].ToString());
                        s.Add("Line", currVar[kind + "Line"]);
                        smells.Add(s);
                    }

                }
            }
            return smells;
        }
        /// <summary>
        /// Method to search invocations with specified argument inside a list of methods 
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="param">The param of the methods to check</param>
        /// <param name="methods">The values to compare to param</param>
        /// <param name="names">The list of the Arguments to search inside the invocations</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FindArgumentsInInvocation(JArray data, string param, List<string> methods, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = token as JObject;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..Methods[?({Utility.QueryString(".", param, methods, "==", "||")})]..Invocations");
                    JArray results = new JArray(query);
                    foreach (JToken inv in results.Values())
                    {
                        string fullName = inv["FullName"].ToString();
                        JArray arguments = new JArray(inv["Arguments"]);
                        foreach (JToken arg in arguments.Values())
                        {
                            if (!Utility.ListInString(names, arg["Argument"].ToString())) continue;
                            JObject s1 = new JObject();
                            s1.Add("Script", cu["FileName"]);
                            s1.Add("Invocation", fullName);
                            s1.Add("Line", inv["Line"]);
                            smells.Add(s1);
                        }
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Search all the variable inside the invocation of a set of methods (Depth search)
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="param">The param of the methods to check</param>
        /// <param name="methods">The values to compare to param</param>
        /// <param name="param1">The first param of the variables to check</param>
        /// <param name="values">The list of values to compare to param1</param>
        /// <param name="param2">The second param of the variables to check</param>
        /// <param name="names">The list of values to compare to param2</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FindVariableInIvocations(JArray data, string param, List<string> methods, string param1, List<string> values, string param2, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    List<MethodReference> methodsToCheck = new List<MethodReference>();
                    List<string> checkedMethods = new List<string>();
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]..Invocations");
                    JArray results = new JArray(query);
                    JArray invArgs = new JArray();
                    foreach (JToken inv in results.Values())
                    {
                        string fullName = inv["FullName"].ToString();
                        methodsToCheck.Add(new MethodReference(fullName, (int)inv["Line"]));
                        JArray arguments = new JArray(inv["Arguments"]);
                        foreach (JToken arg in arguments.Values())
                        {
                            if (!Utility.ListInString(names, arg["Argument"].ToString())) continue;
                            JObject s1 = new JObject();
                            s1.Add("Invocation", fullName);
                            s1.Add("Line", inv["Line"]);
                            invArgs.Add(s1);
                        }
                    }
                    JArray res = SmellsInInvocation(data, methodsToCheck, checkedMethods, param1, values, param2, names);
                    if (res.Count <= 0) continue;
                    JObject s = new JObject();
                    s.Add("Script", cu["FileName"]);
                    s.Add("In", res);
                    smells.Add(s);
                }
            }

            return smells;
        }
        /// <summary>
        /// Recursive function to search all the variables with specified parameter inside a set of methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methodsToCheck">The list of methods to check</param>
        /// <param name="checkedMethods">The list of methods already checked</param>
        /// <param name="param1">The first param of the variables to check</param>
        /// <param name="values">The values to compare to param1</param>
        /// <param name="param2">The second param of the variables to check</param>
        /// <param name="names">The values to compare to param2</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray SmellsInInvocation(JArray data, List<MethodReference> methodsToCheck, List<string> checkedMethods, string param1, List<string> values, string param2, List<string> names)
        {
            JArray smells = new JArray();
            List<MethodReference> m_check = new List<MethodReference>();
            foreach (var method in methodsToCheck)
            {
                if (checkedMethods.Contains(method.FullName)) continue;
                checkedMethods.Add(method.FullName);
                var queryRes = data.SelectTokens($"$.[?(@..Methods[?(@.FullName == '{method.FullName}')])]");
                if (queryRes.Count() < 0) continue;
                JArray compUnit = new JArray(queryRes);
                queryRes = compUnit.SelectTokens($"$..Methods[?(@.FullName == '{method.FullName}')]..Invocations");
                JArray invocations = new JArray(queryRes);
                if (invocations.Count() <= 0) continue;
                JArray invArgs = new JArray();
                foreach (JToken inv in invocations.Values())
                {
                    string fullName = inv["FullName"].ToString();
                    if (fullName == method.FullName) continue;
                    var m = new MethodReference(fullName, (int)inv["Line"]);
                    m_check.Add(m);
                    JArray arguments = new JArray(inv["Arguments"]);
                    foreach (JToken arg in arguments.Values())
                    {
                        if (!Utility.ListInString(names, arg["Argument"].ToString())) continue;
                        JObject s1 = new JObject();
                        s1.Add("Invocation", fullName);
                        s1.Add("Line", inv["Line"]);
                        invArgs.Add(s1);
                    }
                }
                JArray res = VariableFromMethods(data, "FullName", new List<string> { method.FullName }, param1, values, param2, names);
                if (res.Count <= 0 && invArgs.Count <= 0) continue;
                JObject s = new JObject();
                s.Add("Script", compUnit.First()["FileName"]);
                s.Add("Method", method.FullName);
                s.Add("Variables", res);
                s.Add("Invocations", invArgs);
                smells.Add(s);
                JArray recRes = SmellsInInvocation(data, m_check, checkedMethods, param1, values, param2, names);
                if (recRes.Count() <= 0) continue;
                s = new JObject();
                s.Add("Script", compUnit.First()["FileName"]);
                s.Add("Method", method.FullName);
                s.Add("In", recRes);
                smells.Add(s);
            }
            return smells;
        }
        /// <summary>
        /// Search the implementation of the singleton pattern inside the classes of the project
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="param">The parameter to search inside the fields</param>
        /// <param name="fieldModifiers">The list of values (modifiers) to search inside the fields</param>
        /// <param name="constructorModifiers">The list of modifiers of the constructor</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FindSingleton(JArray data, string param, List<string> fieldModifiers, List<string> constructorModifiers)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                string fileName = token["FileName"].ToString();
                Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + token["Name"].ToString());
                var query = token.SelectTokens($"..Classes");
                JArray classes = new JArray(query);
                foreach (JToken c in classes.Values())
                {
                    if (c is JObject)
                    {
                        bool hasConstructor = false;
                        bool isSingleton = false;
                        string fullName = c["FullName"].ToString();
                        JArray res = FieldWithParam((JObject)c, ".", "FullName", false, new List<string> { fullName }, param, fieldModifiers);
                        if (res.Count() <= 0) continue;
                        List<string> fieldsName = new List<string>();
                        foreach (JToken field in res)
                        {
                            fieldsName.Add(field["FullName"].ToString());
                        }
                        query = c.SelectTokens("$.Constructors");
                        res = new JArray(query);
                        foreach (JToken constr in res.Values())
                        {
                            JArray modifiers = new JArray(constr["Modifiers"]);
                            if (modifiers.Values().Count() <= 0) hasConstructor = true;
                            foreach (JToken m in modifiers.Values())
                            {
                                if (Utility.StringInList(constructorModifiers, m.ToString())) hasConstructor = true;
                            }
                            if (hasConstructor) break;
                        }
                        if (!hasConstructor) continue;

                        query = c.SelectTokens("$.Methods");
                        res = new JArray(query);
                        foreach (JToken method in res.Values())
                        {
                            JArray returnObj = new JArray(method["Returns"]);
                            if (returnObj.Count() <= 0) continue;
                            var tempQ = returnObj.SelectTokens("$..Fields");
                            JArray returnFields = new JArray(tempQ);
                            foreach (JToken rf in returnFields)
                            {
                                foreach (JToken field in rf.Values())
                                {
                                    if (Utility.StringInList(fieldsName, field.ToString())) isSingleton = true;
                                }
                            }
                        }
                        if (isSingleton)
                        {
                            JObject s = new JObject();
                            s.Add("Script", fileName);
                            s.Add("Class", fullName);
                            smells.Add(s);
                        }
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Search all the Field with specified parameters inside the compilation unit
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="typeParam">The name of the type parameter of the fields</param>
        /// <param name="types">The list of type to comapre to the type of the fields</param>
        /// <param name="param">The other parameter to check for the field</param>
        /// <param name="values">The list of values to compare to param</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FieldDependeciesInCompilationUnit(JArray data, string typeParam, List<string> types, string param, List<string> values)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    smells.Merge(FieldWithParam(cu, "..", typeParam, false, types, param, values));
                }
            }
            return smells;
        }
        /// <summary>
        /// Search all the Fields inside a JObject with specified parameters 
        /// </summary>
        /// <param name="cu">The JObject where to search the fields</param>
        /// <param name="queryOp">The query operator to use inside the Jpath query</param>
        /// <param name="keyParam">The first param of the fields to check</param>
        /// <param name="equals">The type of comparison between keyparam and types</param>
        /// <param name="types">The vaules to comapre to keyParam</param>
        /// <param name="param1">A param to check for the fields</param>
        /// <param name="values">The values to compare to param1</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FieldWithParam(JObject cu, string queryOp, string keyParam, bool equals, List<string> types, string param1, List<string> values)
        {
            JArray smells = new JArray();
            var query = cu.SelectTokens($"${queryOp}Fields");
            JArray res = new JArray(query);
            foreach (JToken field in res.Values())
            {
                if (!(field is JObject)) continue;
                if (values != null)
                {
                    List<string> valueRes = new List<string>();

                    foreach (JToken a in field[param1]) valueRes.Add(a.ToString());

                    if (!Utility.ListInList(valueRes, values)) continue;
                }

                string type = field[keyParam].ToString();
                if (equals)
                    if (!Utility.StringInList(types, type) && types != null) continue;
                if (!equals) if (!Utility.ListInString(types, type) && types != null) continue;
                JObject s = new JObject();
                if (cu.ContainsKey("FileName"))
                    s.Add("Script", cu["FileName"]);
                s.Add("Kind", "Field");
                s.Add("Type", field["Type"]);
                s.Add("FullName", field["FullName"]);
                s.Add("Line", field["Line"]);
                smells.Add(s);

            }
            return smells;
        }
        /// <summary>
        /// Search all the invocations inside methods that return one of the specified types
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="types">The list of return type to check</param>
        /// <param name="methods">The list of methods where to analyze the invocation</param>
        /// <param name="collection">The type of methods to check (Methods or Constructors)</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray DependeciesInMethods(JArray data, List<string> types, List<string> methods, string collection, string invParam,bool compType)
        {
            //Aggiugere se possibile il full name del returntype di un invocation
            JArray results = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..{collection}");

                    JArray res = new JArray(query);
                    foreach (JToken method in res.Values())
                    {
                        JArray invocations = new JArray(method.SelectTokens("..Invocations"));
                        foreach (JToken inv in invocations.Values())
                        {
                            string name = inv["Name"].ToString();
                            if (!methods.Contains(name)) continue;
                            string returnType = inv[invParam].ToString();
                            if (compType)
                            {
                                if (!Utility.StringInList(types, returnType)) continue;
                            }
                            else
                            {
                                if (!Utility.ListInString(types, returnType)) continue;
                            }
                            JObject s = new JObject();
                            s.Add("Script", cu["FileName"].ToString());
                            s.Add("Kind", "Invocation");
                            s.Add("Name", name);
                            s.Add("ReturnType", returnType);
                            s.Add("Line", inv["Line"]);
                            results.Add(s);

                        }

                    }

                }
            }
            return results;
        }
        /// <summary>
        /// Search the specified types inside the parameters of methods and constructors
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="types">The list of types to check</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray DependeciesInParameters(JArray data, List<string> types)
        {
            JArray smells = new JArray();
            foreach (JToken cu in data)
            {
                Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                var queryRes = cu.SelectTokens($"$..Methods");
                JArray res = new JArray(queryRes);
                foreach (JToken m in res.Values())
                {
                    JArray pars = m["Parameters"] as JArray;
                    foreach (JToken p in pars)
                    {
                        string type = p["Type"].ToString();
                        if (!types.Contains(type)) continue;
                        JObject s = new JObject();
                        s.Add("Script", cu["FileName"]);
                        s.Add("Parameter", p["Name"]);
                        s.Add("Method", m["FullName"]);
                        s.Add("Type", type);
                        s.Add("Line", m["Line"]);
                        smells.Add(s);
                    }
                }
                queryRes = cu.SelectTokens($"$..Constructors");
                res = new JArray(queryRes);
                foreach (JToken m in res.Values())
                {
                    JArray pars = m["Parameters"] as JArray;
                    foreach (JToken p in pars)
                    {
                        string type = p["Type"].ToString();
                        if (!types.Contains(type)) continue;
                        JObject s = new JObject();
                        s.Add("Script", cu["FileName"]);
                        s.Add("Parameter", p["Name"]);
                        s.Add("Constructor", m["FullName"]);
                        s.Add("Type", type);
                        s.Add("Line", m["Line"]);
                        smells.Add(s);
                    }
                }

            }
            return smells;
        }
        /// <summary>
        /// Search inside a list of methods all the specified invocations (Depth search)
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methods">The list of methods where to search the invocations</param>
        /// <param name="invocations">The list of invocation to search</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FindInvocationSmell(JArray data, List<string> methods, List<string> invocations)
        {
            //Console.WriteLine("\tAnalyzing...");
            //extract all the compilation units that exhibit the the requested invocations inside the specified methods
            JArray smells = new JArray();
            JArray res = DirectInvocation(data, methods, invocations);
            smells.Merge(res);
            //Console.Write("\tExtracting Smell With Depth >= 1...");
            //anlyze the other invocation insiede the specified methods to found out if they contain the requested invocations
            foreach (JToken token in data)
            {
                JObject comUnit = new JObject();
                if (token is JObject @object) comUnit = @object;
                Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + comUnit["Name"].ToString());

                JArray mets = new JArray(comUnit.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]"));
                foreach (JToken m in mets)
                {
                    //get the invocations of the current method
                    var r = m.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]..Invocations");
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
                            var mr = new MethodReference(fullName, (int)inv["Line"]);
                            methodsToCheck.Add(mr);
                        }
                    }
                    //analyze to found the possible smell in depth > 1
                    JArray depthResutl = SearchIvocation(data, methodsToCheck, invocations, checkedMethods);
                    if (depthResutl.Count() > 0)
                    {
                        JObject j = new JObject();
                        j.Add("Script", comUnit["FileName"].ToString());
                        j.Add("Traceback", depthResutl);
                        smells.Add(j);
                    }
                }
            }
            //Console.WriteLine("Done!");
            //Console.WriteLine("\tDone!");
            return smells;
        }
        /// <summary>
        /// Extract all the direct invocations inside a list of methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methods">The list of methods where to search the invocations</param>
        /// <param name="invocations">The list of invocation to search</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray DirectInvocation(JArray data, List<string> methods, List<string> invocations)
        {
            //Console.Write("\tExtracting Direct Invocations...");
            JArray smells = new JArray();
            var res1 = data.SelectTokens($"$.[?(@..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})])]");
            JArray fresults = new JArray(res1);

            foreach (JToken met in fresults)
            {
                if (met is JObject)
                {
                    JObject obj = met as JObject;
                    var r = obj.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]..Invocations");
                    JArray invok = new JArray(r);
                    foreach (JToken inv in invok.Values())
                    {
                        if (Utility.ListInString(invocations, inv["FullName"].ToString()))
                        {
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
            //Console.WriteLine("Done!");
            return smells;
        }
        /// <summary>
        /// Recoursive method to search all invocation inside a set of methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methodsToCheck">The list of methods where to search the invocations</param>
        /// <param name="methodsWithSmell">The list of invocation to search</param>
        /// <param name="checkedMethods">The list of methods already visited</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray SearchIvocation(JArray data, List<MethodReference> methodsToCheck, List<string> methodsWithSmell, List<string> checkedMethods)
        {
            JArray results = new JArray();
            //new list of methods to check that will be passed at the recursive call of the function
            List<MethodReference> m_check = new List<MethodReference>();

            foreach (var method in methodsToCheck)
            {
                //to avoid recursive call and stack overflow, if the methods is already visited skip
                if (checkedMethods.Contains(method.FullName)) continue;
                checkedMethods.Add(method.FullName);
                //get the compilation unit that contain the method to analyze
                var queryRes = data.SelectTokens($"$.[?(@..Methods[?(@.FullName == '{method.FullName}')])]");
                if (queryRes.Count() < 0) continue;
                JArray compUnit = new JArray(queryRes);
                //get the list invocations of the method to analyze
                queryRes = compUnit.SelectTokens($"$..Methods[?(@.FullName == '{method.FullName}')]..Invocations");
                JArray invocations = new JArray(queryRes);
                //if the method doesn't contain invocations skip
                if (invocations.Count() <= 0) continue;

                //analyze the invocation inside the method
                foreach (JToken inv in invocations.Values())
                {
                    string name = inv["Name"].ToString();
                    string fullName = inv["FullName"].ToString();
                    if (fullName == method.FullName) continue;

                    if (Utility.ListInString(methodsWithSmell, fullName))
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
                JArray arr = SearchIvocation(data, m_check, methodsWithSmell, checkedMethods);
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
        /// <summary>
        /// Search all the fields used inside conditions block in specified methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="inInvocation">Specify if the search shuld be executed insithe the invocation (only depth == 1)</param>
        /// <param name="metParam">specified the method parameter to check</param>
        /// <param name="methods">the list values to comapre to metParam</param>
        /// <param name="condBlock">The name of the condition block (IfBlocks or SwitchBlocks)</param>
        /// <param name="lineParam">The name of the line parameter</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FieldsInConditionStatementInMethods(JArray data, bool inInvocation, string metParam, List<string> methods, string condBlock, string lineParam)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {

                if (token is JObject)
                {
                    JObject cu = token as JObject;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var queryRes = cu.SelectTokens("$..Classes");
                    JArray classes = new JArray(queryRes);
                    foreach (JToken cl in classes.Values())
                    {
                        if (cl is JObject)
                        {
                            JArray fields = new JArray(cl.SelectTokens($"$.Fields"));

                            List<string> fieldsFullName = new List<string>();
                            foreach (JToken f in fields.Values())
                            {
                                fieldsFullName.Add(f["FullName"].ToString());
                            }

                            JArray methodsFound = new JArray(cl.SelectTokens($"$.Methods[?({Utility.QueryString(".", metParam, methods, "==", "||")})]"));
                            foreach (JToken m in methodsFound)
                            {
                                if (inInvocation)
                                {
                                    List<string> invoks = Utility.GetAllInvocationsOfClassInMethods(cl as JObject, m["FullName"].ToString(), "FullName");
                                    foreach (string invName in invoks)
                                    {
                                        JArray res = new JArray(cl.SelectTokens($"$.Methods[?(@.FullName == '{invName}')]"));
                                        foreach (JToken Minv in res)
                                        {
                                            JArray condStatements1 = new JArray(Minv.SelectTokens($"$..{condBlock}"));
                                            foreach (JToken ifs in condStatements1.Values())
                                            {
                                                JToken fieldsFound = ifs["Condition"]["Fields"];
                                                foreach (JToken f in fieldsFound)
                                                {
                                                    if (Utility.StringInList(fieldsFullName, f.ToString()))
                                                    {
                                                        JObject s = new JObject();
                                                        s.Add("Script", cu["FileName"]);
                                                        s.Add("Class", cl["FullName"]);
                                                        s.Add("Method", m["FullName"]);
                                                        s.Add("Invocation", Minv["FullName"]);
                                                        s.Add("ConditionBlock", condBlock);
                                                        s.Add("Line", ifs[lineParam]);
                                                        smells.Add(s);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                JArray condStatements = new JArray(m.SelectTokens($"$..{condBlock}"));
                                foreach (JToken ifs in condStatements.Values())
                                {
                                    JToken fieldsFound = ifs["Condition"]["Fields"];
                                    foreach (JToken f in fieldsFound)
                                    {
                                        if (Utility.StringInList(fieldsFullName, f.ToString()))
                                        {
                                            JObject s = new JObject();
                                            s.Add("Script", cu["FileName"]);
                                            s.Add("Class", cl["FullName"]);
                                            s.Add("Method", m["FullName"]);
                                            s.Add("ConditionBlock", condBlock);
                                            s.Add("Line", ifs[lineParam]);
                                            smells.Add(s);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Search all the properties used inside conditions block in specified methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="inInvocation">Specify if the search shuld be executed insithe the invocation (only depth == 1)</param>
        /// <param name="metParam">specified the method parameter to check</param>
        /// <param name="methods">the list values to comapre to metParam</param>
        /// <param name="condBlock">The name of the condition block (IfBlocks or SwitchBlocks)</param>
        /// <param name="lineParam">The name of the line parameter</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray PropertiesInConditionStatementInMethods(JArray data, bool inInvocation, string metParam, List<string> methods, string condBlock, string lineParam)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
               
                if (token is JObject)
                {
                    JObject cu = token as JObject;
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var queryRes = cu.SelectTokens("$..Classes");
                    JArray classes = new JArray(queryRes);
                    foreach (JToken cl in classes.Values())
                    {
                        if (cl is JObject)
                        {
                            JArray properties = new JArray(cl.SelectTokens($"$.Properties"));
                            string classFullName = cl["FullName"].ToString();
                            List<string> propertiesFullName = new List<string>();
                            foreach (JToken f in properties.Values())
                            {
                                string fFullName = classFullName + "." + f["Name"].ToString();
                                propertiesFullName.Add(fFullName);
                            }

                            JArray methodsFound = new JArray(cl.SelectTokens($"$.Methods[?({Utility.QueryString(".", metParam, methods, "==", "||")})]"));
                            foreach (JToken m in methodsFound)
                            {
                                if (inInvocation)
                                {
                                    List<string> invoks = Utility.GetAllInvocationsOfClassInMethods(cl as JObject, m["FullName"].ToString(), "FullName");
                                    foreach (string invName in invoks)
                                    {
                                        JArray res = new JArray(cl.SelectTokens($"$.Methods[?(@.FullName == '{invName}')]"));
                                        foreach (JToken Minv in res)
                                        {
                                            JArray condStatements1 = new JArray(Minv.SelectTokens($"$..{condBlock}"));
                                            foreach (JToken ifs in condStatements1.Values())
                                            {
                                                JToken propertiesFound = ifs["Condition"]["Properties"];
                                                foreach (JToken f in propertiesFound)
                                                {
                                                    if (Utility.StringInList(propertiesFullName, f.ToString()))
                                                    {
                                                        JObject s = new JObject();
                                                        s.Add("Script", cu["FileName"]);
                                                        s.Add("Class", cl["FullName"]);
                                                        s.Add("Method", m["FullName"]);
                                                        s.Add("Invocation", Minv["FullName"]);
                                                        s.Add("ConditionBlock", condBlock);
                                                        s.Add("Line", ifs[lineParam]);
                                                        smells.Add(s);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                JArray condStatements = new JArray(m.SelectTokens($"$..{condBlock}"));
                                foreach (JToken ifs in condStatements.Values())
                                {
                                    JToken fieldsFound = ifs["Condition"]["Fields"];
                                    foreach (JToken f in fieldsFound)
                                    {
                                        if (Utility.StringInList(propertiesFullName, f.ToString()))
                                        {
                                            JObject s = new JObject();
                                            s.Add("Script", cu["FileName"]);
                                            s.Add("Class", cl["FullName"]);
                                            s.Add("Method", m["FullName"]);
                                            s.Add("ConditionBlock", condBlock);
                                            s.Add("Line", ifs[lineParam]);
                                            smells.Add(s);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Search all the assignmet made to properties of specified object type
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="type">The object type to search</param>
        /// <param name="propertiesName">The name of the properties to search</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray FindPropertiesChange(JArray data, string type, List<string> propertiesName)
        {
            JArray smells = new JArray();
            foreach (JToken cu in data)
            {
                Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                JArray variables = new JArray(cu.SelectTokens("$..Variable"));
                foreach (JToken v in variables)
                {
                    JObject var = v as JObject;
                    if (var.ContainsKey("FullName"))
                    {
                        string varKind = var["Kind"].ToString();
                        if (varKind == "Assignment" || varKind == "Definition")
                        {
                            string fullName = var["FullName"].ToString();
                            if (fullName.Contains(type))
                            {
                                if (!Utility.ListInString(propertiesName, fullName)) continue;
                                JObject s = new JObject();
                                s.Add("Script", cu["FileName"]);
                                s.Add("Name", var["Name"]);
                                s.Add("Line", var["AssignmentLine"]);
                                smells.Add(s);
                            }
                        }
                    }
                }
            }

            return smells;
        }
        /// <summary>
        /// Search all the use inside a condition block of a specified object properties
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methods">The list of methods where to search the use of the properties</param>
        /// <param name="properties">The list of properties to search</param>
        /// <param name="condBlock">The type of condition block</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray CheckPropertiesInInvocation(JArray data, List<string> methods, List<string> properties, string condBlock)
        {
            JArray smells = new JArray();

            foreach (JToken cu in data)
            {
                if (cu is JObject)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + cu["Name"].ToString());
                    var query = cu.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]");
                    JArray mets = new JArray(query);
                    foreach (JToken m in mets)
                    {
                        List<MethodReference> methodsToCheck = new List<MethodReference>();
                        List<string> checkedMethods = new List<string>();
                        JArray invocations = new JArray(m.SelectTokens("$..Invocations"));
                        foreach (JToken inv in invocations.Values())
                        {
                            methodsToCheck.Add(new MethodReference(inv["FullName"].ToString(), (int)inv["Line"]));
                        }
                        JArray conditionBlocks = new JArray(m.SelectTokens($"$..{condBlock}"));
                        foreach (JToken cb in conditionBlocks.Values())
                        {
                            JArray propertiesFound = cb["Condition"]["Properties"] as JArray;
                            foreach (JToken p in propertiesFound)
                            {
                                if (!Utility.StringInList(properties, p.ToString())) continue;
                                JObject s = new JObject();
                                s.Add("Script", cu["FileName"]);
                                s.Add("Method", m["FullName"]);
                                s.Add("ConditionBlock", condBlock);
                                s.Add("Property", p);
                                s.Add("Line", cb["StartLine"]);
                                smells.Add(s);
                            }
                        }
                        JArray results = PropertiesInInvocations(data, methodsToCheck, checkedMethods, properties, condBlock);
                        if (results.Count() <= 0) continue;
                        JObject j = new JObject();
                        j.Add("Script", cu["FileName"].ToString());
                        j.Add("Method", m["FullName"]);
                        j.Add("Line", m["Line"]);
                        j.Add("Traceback", results);
                        smells.Add(j);

                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Recoursive search of properties of specified object used in condition blocks
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methodsToCheck">The methods to check</param>
        /// <param name="checkedMethods">The methods already checked</param>
        /// <param name="properties">The list of properties to search</param>
        /// <param name="condBlock">The type of the condition block</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray PropertiesInInvocations(JArray data, List<MethodReference> methodsToCheck, List<string> checkedMethods, List<string> properties, string condBlock)
        {
            JArray smells = new JArray();
            List<MethodReference> metToCk = new List<MethodReference>();
            foreach (var method in methodsToCheck)
            {
                //to avoid recursive call and stack overflow, if the methods is already visited skip
                if (checkedMethods.Contains(method.FullName)) continue;
                checkedMethods.Add(method.FullName);
                //get the compilation unit that contain the method to analyze
                var queryRes = data.SelectTokens($"$.[?(@..Methods[?(@.FullName == '{method.FullName}')])]");
                if (queryRes.Count() < 0) continue;
                JArray compUnit = new JArray(queryRes);
                //get the list invocations of the method to analyze
                queryRes = compUnit.SelectTokens($"$..Methods[?(@FullName == '{method.FullName}')]..Invocations");
                JArray invocations = new JArray(queryRes);
                foreach (JToken inv in invocations.Values())
                {
                    string fullName = inv["FullName"].ToString();
                    if (checkedMethods.Contains(fullName)) continue;
                    metToCk.Add(new MethodReference(inv["FullName"].ToString(), (int)inv["Line"]));
                }
                var m = compUnit.SelectToken($"..Methods[?(@.FullName == '{method.FullName}')]");
                if (m == null) continue;
                JArray conditionBlocks = new JArray(m.SelectTokens($"$..{condBlock}"));
                foreach (JToken cb in conditionBlocks.Values())
                {
                    JArray propertiesFound = cb["Condition"]["Properties"] as JArray;
                    foreach (JToken p in propertiesFound)
                    {
                        if (!Utility.StringInList(properties, p.ToString())) continue;
                        JObject s = new JObject();
                        s.Add("Script", compUnit.First()["FileName"]);
                        s.Add("Method", method.FullName);
                        s.Add("LineCall", method.Line);
                        s.Add("Property", p);
                        s.Add("ConditionBlock", condBlock);
                        s.Add("Line", cb["StartLine"]);
                        smells.Add(s);
                    }
                }
                JArray results = PropertiesInInvocations(data, methodsToCheck, checkedMethods, properties, condBlock);
                if (results.Count() <= 0) continue;
                JObject j = new JObject();
                j.Add("Script", compUnit.First()["FileName"].ToString());
                j.Add("Invocation", method.FullName);
                j.Add("Line", method.Line);
                j.Add("Traceback", results);
                smells.Add(j);
            }
            return smells;
        }
        /// <summary>
        /// Search all variable with specific parameters that are changed inside the specifieds methods
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methods">The list of methods where to search the variables</param>
        /// <param name="varKind">The Kind parameter of the variables</param>
        /// <param name="varType">The type of the variables</param>
        /// <param name="varNames">The name of the variables</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray ChangesToVariableOfTypeInMethods(JArray data, List<string> methods, string varKind, string varType, List<string> varNames)
        {
            JArray smells = new JArray();
            foreach(JToken compUnit in data)
            {
                if(compUnit is JObject)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Analyzing: " + compUnit["Name"].ToString());
                    var query = compUnit.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]");
                    JArray res = new JArray(query);
                    foreach(JToken met in res)
                    {
                        //get all invocations to check
                        List<MethodReference> methodsToCheck = new List<MethodReference>();
                        List<string> checkedMethods = new List<string>();
                        JArray invocations = new JArray(met.SelectTokens("$..Invocations"));
                        foreach(JToken inv in invocations.Values())
                        {
                            string fullName = inv["FullName"].ToString();
                            int line = (int)inv["Line"];
                            methodsToCheck.Add(new MethodReference(fullName, line));
                        }
                        smells.Merge(SearchVariablesOfKind(compUnit, met, "..", varKind, varType, varNames));
                        JArray results = SearchVariablesOfKindInInvocations(data, methodsToCheck, checkedMethods, varKind, varType, varNames);
                        if (results.Count() <= 0) continue;
                        JObject s = new JObject();
                        s.Add("Script", compUnit["FileName"]);
                        s.Add("Method", met["FullName"]);
                        s.Add("Line", met["Line"]);
                        s.Add("Traceback", results);
                        smells.Add(s);
                    }
                }
            }
            return smells;
        }
        /// <summary>
        /// Search variables with specified paramenters inside a jtoken
        /// </summary>
        /// <param name="compUnit">The compilation unit of the container</param>
        /// <param name="container">The jtoken where to search the container</param>
        /// <param name="searchOp">The depth opearator for the json path query</param>
        /// <param name="varKind">The Kind parameter of the variables</param>
        /// <param name="varType">The type of the variables</param>
        /// <param name="varNames">The name of the variables</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray SearchVariablesOfKind(JToken compUnit, JToken container, string searchOp, string varKind, string varType, List<string> varNames)
        {
            JArray smells = new JArray();
            JArray variables = new JArray(container.SelectTokens($"${searchOp}Variables"));
            JArray varsAssigned = new JArray(variables.SelectTokens($"$..[?(@.Kind == '{varKind}')]"));
            foreach (JToken currVar in varsAssigned)
            {
                string varName = currVar["Name"].ToString();
                if (!Utility.ListInString(varNames, varName)) continue;
                JArray cascades = currVar["Cascades"] as JArray;
                foreach (JToken reference in cascades)
                {
                    string refName = reference["Type"].ToString();
                    if (!refName.Contains(varType)) continue;
                    //Console.WriteLine(refName);
                    JObject s = new JObject();
                    s.Add("Script", compUnit["FileName"]);
                    s.Add("Method", container["FullName"]);
                    s.Add("Variable", varName);
                    s.Add("Line", currVar[$"{varKind}Line"]);
                    //Console.WriteLine(s.ToString());
                    smells.Add(s);
                    break;
                }

            }
            return smells;
        }
        /// <summary>
        /// Recoursive function to search variables with specified parameters inside a list of methods and their invocations
        /// </summary>
        /// <param name="data">The dataset containing the compilation unit of the project</param>
        /// <param name="methodsToCheck">The methods to check</param>
        /// <param name="checkedMethods">The methods already checked</param>
        /// <param name="varKind">The Kind parameter of the variables</param>
        /// <param name="varType">The type of the variables</param>
        /// <param name="varNames">The name of the variables</param>
        /// <returns>A Jarray containing the smells found</returns>
        public static JArray SearchVariablesOfKindInInvocations(JArray data, List<MethodReference> methodsToCheck, List<string> checkedMethods, string varKind, string varType, List<string> varNames)
        {
            JArray smells = new JArray();
            List<MethodReference> metToCk = new List<MethodReference>();
            foreach(MethodReference mr in methodsToCheck)
            {
                if (checkedMethods.Contains(mr.FullName)) continue;
                checkedMethods.Add(mr.FullName);
                //get the compilation unit that contain the method to analyze
                var queryRes = data.SelectTokens($"$.[?(@..Methods[?(@.FullName == '{mr.FullName}')])]");
                if (queryRes.Count() < 0) continue;
                JArray compUnit = new JArray(queryRes);
                //get the list invocations of the method to analyze
                queryRes = compUnit.SelectTokens($"$..Methods[?(@FullName == '{mr.FullName}')]..Invocations");
                JArray invocations = new JArray(queryRes);
                foreach (JToken inv in invocations.Values())
                {
                    string fullName = inv["FullName"].ToString();
                    if (checkedMethods.Contains(fullName)) continue;
                    metToCk.Add(new MethodReference(inv["FullName"].ToString(), (int)inv["Line"]));
                }
                var m = compUnit.SelectToken($"..Methods[?(@.FullName == '{mr.FullName}')]");
                if (m == null) continue;
                smells.Merge(SearchVariablesOfKind(compUnit.First(), m, "..", varKind, varType, varNames));
                JArray results = SearchVariablesOfKindInInvocations(data, metToCk, checkedMethods, varKind, varType, varNames);
                if (results.Count() <= 0) continue;
                JObject s = new JObject();
                s.Add("Script", compUnit.First()["FileName"]);
                s.Add("Invocation", mr.FullName);
                s.Add("Line", mr.Line);
                s.Add("Traceback", results);
                smells.Add(s);
            }
            return smells;
        }
    }
}

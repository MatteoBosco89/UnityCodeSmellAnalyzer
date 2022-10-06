using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CodeSmellFinder
{
    public static class DataExtractor
    {
        public static JArray MethodsWithName(JArray data, string param, List<string> methods)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
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

        public static JArray VariableFromMethods(JArray data, string param, List<string> methods, string param1, List<string> values, string param2, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
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

        public static JArray VariablesFromData(JArray data, List<string> types, string param, List<string> values, string param1, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    var query = cu.SelectTokens($"$..Variables[?({Utility.QueryString("..", param, values, "==", "||")})]");
                    JArray res = new JArray(query);

                    foreach (JToken var in res.Values())
                    {
                        JObject j = new JObject(var);
                        JToken currVar = j["Variable"];
                        if (types != null)
                        {
                            string type = currVar["Type"].ToString();
                            if (!Utility.StringInList(types, type)) continue;
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

        public static JArray FindArgumentsInInvocation(JArray data, string param, List<string> methods, List<string> names)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = token as JObject;
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

        public static JArray FindSingleton(JArray data, string param, List<string> fieldModifiers, List<string> constructorModifiers)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                string fileName = token["FileName"].ToString();
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

        public static JArray FieldDependeciesInCompilationUnit(JArray data, string typeParam, List<string> types, string param, List<string> values)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    smells.Merge(FieldWithParam(cu, "..", typeParam, false, types, param, values));
                }
            }
            return smells;
        }

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


        public static JArray DependeciesInMethods(JArray data, List<string> types, List<string> methods, string collection)
        {
            //Aggiugere se possibile il full name del returntype di un invocation
            JArray results = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    var query = cu.SelectTokens($"$..{collection}");

                    JArray res = new JArray(query);
                    foreach (JToken method in res.Values())
                    {
                        JArray invocations = new JArray(method["Invocations"].ToArray());
                        foreach (JToken inv in invocations)
                        {
                            string name = inv["Name"].ToString();
                            if (!methods.Contains(name)) continue;
                            string returnType = inv["ReturnType"].ToString();
                            if (Utility.StringInList(types, returnType))
                            {
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
            }
            return results;
        }

        public static JArray DependeciesInParameters(JArray data, List<string> types)
        {
            JArray smells = new JArray();
            foreach(JToken cu in data)
            {
                var queryRes = cu.SelectTokens($"$..Methods");
                JArray res = new JArray(queryRes);
                foreach(JToken m in res.Values())
                {
                    JArray pars = m["Parameters"] as JArray;
                    foreach(JToken p in pars)
                    {
                        string type = p["Type"].ToString();
                        if (!Utility.StringInList(types, type)) continue;
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
                        if (!Utility.StringInList(types, type)) continue;
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

        public static JArray FindInvocationSmell(JArray data, List<string> methods, List<string> invocations)
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
                var r = comUnit.SelectTokens($"$..Methods[?({Utility.QueryString(".", "Name", methods, "==", "||")})]..Invocations");
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
            return smells;
        }

        public static JArray DirectInvocation(JArray data, List<string> methods, List<string> invocations)
        {
            Console.Write("\tExtracting Direct Invocations...");
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
            Console.WriteLine("Done!");
            return smells;
        }

        public static JArray SearchIvocation(JObject parentCU, JArray data, List<MethodReference> methodsToCheck, List<string> methodsWithSmell, List<string> checkedMethods)
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
                queryRes = compUnit.SelectTokens($"$..Methods[?(@FullName == '{method.FullName}')]..Invocations");
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
                JArray arr = SearchIvocation((JObject)compUnit.First(), data, m_check, methodsWithSmell, checkedMethods);
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

        public static JArray FieldsInConditionStatementInMethods(JArray data, bool inInvocation, string metParam, List<string> methods, string condBlock, string lineParam)
        {
            JArray smells = new JArray();
            foreach(JToken token in data)
            {
                if(token is JObject)
                {
                    JObject cu = token as JObject;
                    var queryRes = cu.SelectTokens("$..Classes");
                    JArray classes = new JArray(queryRes);
                    foreach(JToken cl in classes.Values())
                    {
                        if(cl is JObject)
                        {
                            JArray fields = new JArray(cl.SelectTokens($"$.Fields"));

                            List<string> fieldsFullName = new List<string>();
                            foreach(JToken f in fields.Values())
                            {
                                fieldsFullName.Add(f["FullName"].ToString());
                            }

                            JArray methodsFound = new JArray(cl.SelectTokens($"$.Methods[?({Utility.QueryString(".", metParam, methods, "==", "||")})]"));
                            foreach(JToken m in methodsFound)
                            {
                                if (inInvocation)
                                {
                                    List<string> invoks = Utility.GetAllInvocationsOfClassInMethods(cl as JObject,  m["FullName"].ToString(), "FullName");
                                    foreach(string invName in invoks)
                                    {
                                        JArray res = new JArray(cl.SelectTokens($"$.Methods[?(@.FullName == '{invName}')]"));
                                        foreach(JToken Minv in res)
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

        public static JArray PropertiesInConditionStatementInMethods(JArray data, bool inInvocation, string metParam, List<string> methods, string condBlock, string lineParam)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = token as JObject;
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
                                    List<string> invoks = Utility.GetAllInvocationsOfClassInMethods(cl as JObject,  m["FullName"].ToString(), "FullName");
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
        public static JArray FindVelocityChange(JArray data, string type, List<string> velocityParam)
        {
            JArray smells = new JArray();
            foreach(JToken cu in data)
            {
                JArray classes = new JArray(cu.SelectTokens("$..Classes"));
                foreach(JToken c in classes)
                {
                    List<string> fieldsName = new List<string>();
                    
                    JArray fields = c["Fields"] as JArray;
                    foreach (JToken f in fields)
                    {
                        if (f["Type"].ToString().Contains(type)) fieldsName.Add(f["Name"].ToString());
                    }
                    JArray methods = c["Methods"] as JArray;
                    foreach(JToken m in methods)
                    {
                        List<string> variablesName = new List<string>();
                        JArray variables = new JArray(m.SelectTokens("$..Variable"));
                        foreach(JToken v in variables)
                        {
                            if (v["Type"].ToString().Contains(type)) variablesName.Add(v["Name"].ToString());
                        }
                        foreach(JToken v in variables)
                        {
                            if (v["Kind"].ToString().Contains("Assignment"))
                            {
                                string name = v["Name"].ToString();
                                if(Utility.ListInString(velocityParam, name))
                                {
                                    string varName = name.Split('.')[0];
                                    if (v["VariableKind"].ToString() == "Field")
                                    {
                                        if (fieldsName.Contains(varName))
                                        {
                                            JObject s = new JObject();
                                            s.Add("");
                                        }
                                    }else
                                    {
                                        if (variablesName.Contains(varName))
                                        {

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
    }
}

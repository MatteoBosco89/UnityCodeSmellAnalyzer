using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace TestJsonQuery
{
    internal class Program
    {
        public static List<string> chek = new List<string>();
        static void Main(string[] args)
        {
            string text = File.ReadAllText("C:\\Users\\pasqu\\Documents\\Università\\Tesi\\UnityCodeSmellAnalyzer\\Analyzer\\UnityCodeSmellAnalyzer\\bin\\Debug\\net472\\results.json");
            JArray data = JArray.Parse(text);
            List<string> types = GetAllType(data, new List<string> {"Classes", "Interfaces"}, "Name");
            JArray results = new JArray();
            results.Add(StaticCoupling(data, types, new List<string> {"SerializeField"}));
            results.Add(DependenciesBetweenObjects(data, types, new List<string> {"GetComponent"}));
            results.Add(ImproperCollider(data, new List<string> { "MeshCollider" }, new List<string> { "GetComponent" }));
            results.Add(WeakTemporization(data, new List<string> {"FixedUpdate"}, new List<string> {"Assignment", "Definition"}, new List<string> {"Time.time"}));
            results.Add(LackOfSeparationOfConcern(data, new List<string> { "UnityEngine." }));
            results.Add(SingletonPattern(data));
            File.WriteAllText("prova.json", results.ToString());
        }

        public static JObject StaticCoupling(JArray data, List<string> types, List<string> attributesList)
        {
            JObject result = new JObject();
            result.Add("Name", "Static Couplig");
            JArray smells = new JArray();
            smells.Merge(FieldDependeciesInCompilationUnit(data, types, "Attributes", attributesList));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("StaticCoupling.json", result.ToString());
            return result;
        }

        public static JObject WeakTemporization(JArray data, List<string> methods, List<string> kinds, List<string> names)
        {
            JObject result = new JObject();
            result.Add("Name", "Weak Temporization");
            JArray smells = new JArray();
            smells.Merge(MethodsWithName(data, "Name", new List<string> { "Update" }));
            smells.Merge(FindArgumentsInInvocation(data, "Name", methods, names));
            smells.Merge(VariableFromMethods(data, "Name", methods, "Kind", kinds, "Assignment", names));
            smells.Merge(FindVariableInIvocations(data, "Name", methods, "Kind", kinds, "Assignment", names));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("WeakTemporization.json", result.ToString());
            return result;
        }

        public static JObject ImproperCollider(JArray data, List<string> types, List<string> methods)
        {
            //TODO: Extract all classes with specified inheritances of the types
            //TODO: Controllare i parametri dei metodi se presentano i tipi richiesti
            JObject result = new JObject();
            result.Add("Name", "Improper Collider");
            JArray smells = new JArray();
            smells.Merge(FieldDependeciesInCompilationUnit(data, types, null, null));
            smells.Merge(DependeciesInMethods(data, types, methods));
            smells.Merge(VariablesFromData(data, types, "Kind", new List<string> { "Definition" }, "", null));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("ImproperCollider.json", result.ToString());
            return result;

        }

        public static JObject DependenciesBetweenObjects(JArray data, List<string> types, List<string> methods)
        {
            JObject result = new JObject();
            result.Add("Name", "Dependency Between Objects");
            JArray smells = new JArray();
            smells.Merge(FieldDependeciesInCompilationUnit(data, types, null, null));
            smells.Merge(DependeciesInMethods(data, types, methods));
            //smells.Merge(DependeciesInMethodsParameters(data, types));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("DependenciesBetweenObjects.json", result.ToString());
            return result;
        }

        public static JObject LackOfSeparationOfConcern(JArray data, List<string> lib)
        {
            JObject result = new JObject();
            result.Add("Name", "Lack of separation of concern");
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject comUnit = (JObject)token;
                    List<UsingReference> usings = FindUsing(comUnit, lib);
                    if(usings.Count > 1)
                    {
                        JObject s = new JObject();
                        s.Add("Script", comUnit["FileName"]);
                        JArray arr = new JArray();
                        foreach(var u  in usings)
                        {
                            JObject a = new JObject();
                            a.Add("Name", u.FullName);
                            a.Add("Line", u.Line);
                            arr.Add(a);
                        }
                        s.Add("Usings", arr);
                        smells.Add(s);
                    }
                }
            }
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("LackOfSeparation.json", result.ToString());
            return result;
        }
        
        public static JObject SingletonPattern(JArray data)
        {
            JObject result = new JObject();
            result.Add("Name", "Singleton Pattern");
            JArray smells = new JArray();
            smells.Merge(FindSingleton(data, "Modifiers", new List<string> { "static" }, new List<string> {"protected", "private"}));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            //File.WriteAllText("SingletonPattern.json", result.ToString());
            return result;
        }
        
        public static void VelocityChange(JArray data)
        {
            //TODO: Eseguire la ricerca su RIGIDBODY E CHARACTER CONTROLLER
            JArray smells = new JArray();
            
            File.WriteAllText("TrasformChange.json", smells.ToString());
        }

        public static JArray DependeciesInMethodsParameters(JArray data, List<string> types)
        {
            JArray smells = new JArray();
            return smells;
        }

        public static List<UsingReference> FindUsing(JObject comUnit, List<string> lib)
        {
            List<UsingReference>  results= new List<UsingReference>();
            var query = comUnit.SelectTokens($"$..Usings");
            JArray usings = new JArray(query);
            foreach(JToken token in usings.Values())
            {
                if (!ListInString(lib, token["Name"].ToString())) continue;
                results.Add(new UsingReference(token["Name"].ToString(), (int)token["Line"]));
            }
            return results;

        }

        public static List<string> ClassInheritance(JArray data, string param1, string param2, string param3,  string inheritances)
        {
            List<string> classes = new List<string>();
            var res = data.SelectTokens($"$..{param1}");
            JArray results = new JArray(res);
           
            foreach(JToken c in results)
            {
                if(c is JObject)
                {
                    string inh = c[param2].ToString();
                    if (inh.Contains(inheritances))
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
            baseInher.AddRange(ClassInheritance(data, "Classes", "Inheritance", "FullName", inheritance));
            baseInher.AddRange(AllInheritances(data, baseInher));
            return baseInher;
        }

        public static List<string> AllInheritances(JArray data, List<string> baseInher)
        {
            List<string> allInheritances = new List<string>();
            allInheritances.AddRange(baseInher);
            for (int i = 0; i < baseInher.Count; i++)
            {
                var res = data.SelectTokens($"$..Classes[?(@.InheritanceFullName == '{baseInher[i]}')]");
                JArray results = new JArray(res);
                foreach(JToken cu in results)
                {
                    string name = cu["FullName"].ToString();
                    allInheritances.Add(name);
                }
            }
            if (baseInher.Count < allInheritances.Count) AllInheritances(data, allInheritances);
            return allInheritances;
        }

        public static JArray MethodsWithName(JArray data, string param,  List<string> methods)
        {
            JArray smells = new JArray();
            foreach (JToken token in data)
            {
                if(token is JObject)
                {
                    JObject cu = (JObject)token;
                    var queryRes = cu.SelectTokens($"$..Methods[?({QueryString(".", param, methods, "==", "||")})]");
                    JArray mets = new JArray(queryRes);
                    foreach(JToken m in mets)
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
                    var query = cu.SelectTokens($"$..Methods[?({QueryString(".", param, methods, "==", "||")})]");
                    JArray res = new JArray(query);

                    foreach (JToken met in res)
                    {
                        query = met.SelectTokens($"$..Variables[?({QueryString("..", param1, values, "==", "||")})]");
                        JArray variables = new JArray(query);
                        foreach(JToken var in variables.Values())
                        {
                            JObject j = new JObject(var);
                            JToken currVar = j["Variable"];
                            if (names != null)
                            {
                                string name = currVar[param2].ToString();
                                
                                if (!ListInString(names, name)) continue;
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
                    var query = cu.SelectTokens($"$..Variables[?({QueryString("..", param, values, "==", "||")})]");
                    JArray res = new JArray(query);
                    
                    foreach(JToken var in res.Values())
                    {
                        JObject j = new JObject(var);
                        JToken currVar = j["Variable"];
                        if(types != null)
                        {
                            string type = currVar["Type"].ToString();
                            if (!StringInList(types, type)) continue;
                        }
                        if(names != null)
                        {
                            string name = currVar[param1].ToString();
                            if (!ListInString(names, name)) continue;
                        }
                        JObject s = new JObject();
                        s.Add("Script", cu["FileName"].ToString());
                        string kind = currVar["Kind"].ToString();
                        s.Add("Kind", kind);
                        s.Add("Type", currVar["Type"].ToString());
                        s.Add("Line", currVar[kind+"Line"]);
                        smells.Add(s);
                    }

                }
            }
            return smells;
        }

        public static JArray FindArgumentsInInvocation(JArray data, string param, List<string> methods, List<string> names)
        {
            JArray smells = new JArray();
            foreach(JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = token as JObject;
                    var query = cu.SelectTokens($"$..Methods[?({QueryString(".", param, methods, "==", "||")})]..Invocations");
                    JArray results = new JArray(query);
                    foreach (JToken inv in results.Values())
                    {
                        string fullName = inv["FullName"].ToString();
                        JArray arguments = new JArray(inv["Arguments"]);
                        foreach (JToken arg in arguments.Values())
                        {
                            if (!ListInString(names, arg["Argument"].ToString())) continue;
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
            foreach(JToken token in data)
            {
                if(token is JObject)
                {
                    List<MethodReference> methodsToCheck = new List<MethodReference>();
                    List<string> checkedMethods = new List<string>();
                    JObject cu = (JObject)token;
                    var query = cu.SelectTokens($"$..Methods[?({QueryString(".", "Name", methods, "==", "||")})]..Invocations");
                    JArray results = new JArray(query);
                    JArray invArgs = new JArray();
                    foreach (JToken inv in results.Values())
                    {
                        string fullName = inv["FullName"].ToString();
                        methodsToCheck.Add(new MethodReference(fullName, (int)inv["Line"]));
                        JArray arguments = new JArray(inv["Arguments"]);
                        foreach (JToken arg in arguments.Values())
                        {
                            if (!ListInString(names, arg["Argument"].ToString())) continue;
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
            foreach(var method in methodsToCheck)
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
                foreach(JToken inv in invocations.Values())
                { 
                    string fullName = inv["FullName"].ToString();
                    if (fullName == method.FullName) continue;
                    var m = new MethodReference(fullName, (int)inv["Line"]);
                    m_check.Add(m);
                    JArray arguments = new JArray(inv["Arguments"]);
                    foreach(JToken arg in arguments.Values())
                    {
                        if (!ListInString(names, arg["Argument"].ToString()))continue;
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

      

        public static JArray FieldDependeciesInCompilationUnit(JArray data, List<string> types, string param, List<string> values)
        {
            JArray smells = new JArray();
            foreach(JToken token in data)
            {
                if(token is JObject)
                {   
                    JObject cu = (JObject)token;
                    smells.Merge(FieldWithParam(cu, "..", "Type", true, types, param, values));
                }
            }
            return smells;
        }

        public static JArray FindSingleton(JArray data, string param, List<string> fieldModifiers, List<string> constructorModifiers)
        {
            JArray smells = new JArray();
            foreach(JToken token in data)
            {
                string fileName = token["FileName"].ToString();
                var query = token.SelectTokens($"..Classes");
                JArray classes = new JArray(query);
                foreach (JToken c in classes.Values())
                {
                    if(c is JObject)
                    {
                        bool hasConstructor = false;
                        bool isSingleton = false;
                        string fullName = c["FullName"].ToString();
                        JArray res = FieldWithParam((JObject)c, ".", "FullName", false, new List<string> {fullName}, param, fieldModifiers);
                        if (res.Count() <= 0) continue;
                        List<string> fieldsName = new List<string>(); 
                        foreach(JToken field in res)
                        {
                            fieldsName.Add(field["FullName"].ToString());
                        }
                        query = c.SelectTokens("$.Constructors");
                        res = new JArray(query);
                        foreach (JToken constr in res.Values())
                        {
                            JArray modifiers = new JArray(constr["Modifiers"]);
                            if (modifiers.Values().Count() <= 0) hasConstructor = true;
                            foreach(JToken m in modifiers.Values())
                            {
                                if(StringInList(constructorModifiers, m.ToString())) hasConstructor = true;
                            }
                            if (hasConstructor) break;
                        }
                        if (!hasConstructor) continue;

                        query = c.SelectTokens("$.Methods");
                        res = new JArray(query);
                        foreach(JToken method in res.Values())
                        {
                            JArray returnObj = new JArray(method["Returns"]);
                            if (returnObj.Count() <= 0) continue;
                            var tempQ = returnObj.SelectTokens("$..Fields");
                            JArray returnFields = new JArray(tempQ);
                            foreach(JToken rf in returnFields)
                            {
                                foreach(JToken field in rf.Values())
                                {
                                    if (StringInList(fieldsName, field.ToString())) isSingleton = true;
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

                    if (!ListInList(valueRes, values)) continue;
                }

                string type = field[keyParam].ToString();
                if(equals)if (!types.Contains(type) && types != null) continue;
                if(!equals)if(!ListInString(types, type)) continue;
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
        public static List<string> GetAllType(JArray data, List<string> names, string param)
        {
            List<string> types = new List<string>();
            foreach(string name in names)
            {
                types.AddRange(GetAllParamOfElement(data, name, param));
            }
            return types;
        }

        public static JArray DependeciesInMethods(JArray data, List<string> types, List<string> methods)
        {
            JArray results = new JArray();
            foreach (JToken token in data)
            {
                if (token is JObject)
                {
                    JObject cu = (JObject)token;
                    var query = cu.SelectTokens($"$..Methods");

                    JArray res = new JArray(query);
                    foreach (JToken method in res.Values())
                    {
                        JArray invocations = new JArray(method["Invocations"].ToArray());
                        foreach (JToken inv in invocations)
                        {
                            string name = inv["Name"].ToString();
                            if (!methods.Contains(name)) continue;
                            string returnType = inv["ReturnType"].ToString();
                            if (types.Contains(returnType))
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

        public static List<string> GetAllParamOfElement(JArray data, string element, string param)
        {
            List<string> results = new List<string>();
            var res = data.SelectTokens($"$..{element}");
            JArray arr = new JArray(res);
            foreach(JToken tokens in arr)
            {
                foreach(JToken token in tokens)
                {
                    if (token is JObject)
                    {
                        results.Add(token[param].ToString());
                    }
                } 
            }
            return results;
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
                var r = comUnit.SelectTokens($"$..Methods[?({QueryString(".", "Name", methods, "==", "||")})]..Invocations");
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
            var res1 = data.SelectTokens($"$.[?(@..Methods[?({QueryString(".", "Name", methods, "==", "||")})])]");
            JArray fresults = new JArray(res1);

            foreach (JToken met in fresults)
            {
                if (met is JObject)
                {
                    JObject obj = met as JObject;
                    var r = obj.SelectTokens($"$..Methods[?({QueryString(".", "Name", methods, "==", "||")})]..Invocations");
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
            for(int i = 0; i < values.Count && !found; i++)
            {
                found = StringInList(inList, values[i]);
            }
            return found;
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

    public class UsingReference
    {
        protected string fullName;
        protected int line;

        public string FullName { get { return fullName; } }
        public int Line { get { return line; } }
        public UsingReference(string fullName, int line)
        {
            this.fullName = fullName;
            this.line = line;
        }
    }
}

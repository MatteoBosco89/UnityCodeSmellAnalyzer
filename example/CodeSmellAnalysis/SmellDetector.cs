using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeSmellFinder
{
    public static class SmellDetector
    {
        public static JObject StaticCoupling(JArray data, List<string> types, List<string> attributesList)
        {
            JObject result = new JObject();
            result.Add("Name", "Static Couplig");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, types, "Attributes", attributesList));
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
            smells.Merge(DataExtractor.MethodsWithName(data, "Name", new List<string> { "Update" }));
            smells.Merge(DataExtractor.FindArgumentsInInvocation(data, "Name", methods, names));
            smells.Merge(DataExtractor.VariableFromMethods(data, "Name", methods, "Kind", kinds, "Assignment", names));
            smells.Merge(DataExtractor.FindVariableInIvocations(data, "Name", methods, "Kind", kinds, "Assignment", names));
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
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, types, null, null));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods));
            smells.Merge(DataExtractor.VariablesFromData(data, types, "Kind", new List<string> { "Definition" }, "", null));
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
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, types, null, null));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods));
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
                    List<UsingReference> usings = Utility.FindUsing(comUnit, lib);
                    if (usings.Count > 1)
                    {
                        JObject s = new JObject();
                        s.Add("Script", comUnit["FileName"]);
                        JArray arr = new JArray();
                        foreach (var u in usings)
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
            smells.Merge(DataExtractor.FindSingleton(data, "Modifiers", new List<string> { "static" }, new List<string> { "protected", "private" }));
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
    }
}


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
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, "Attributes", attributesList));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
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
            return result;
        }

        public static JObject ImproperCollider(JArray data, List<string> types, List<string> methods)
        {
            List<string> classesFound = Utility.AllClassInheritances(data, "UnityEngine.MeshCollider");
            types.AddRange(classesFound);
            JObject result = new JObject();
            result.Add("Name", "Improper Collider");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, null, null));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Methods"));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Constructors"));
            smells.Merge(DataExtractor.VariablesFromData(data, types, "Kind", new List<string> { "Definition" }, "", null));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        public static JObject DependenciesBetweenObjects(JArray data, List<string> types, List<string> methods)
        {
            JObject result = new JObject();
            result.Add("Name", "Dependency Between Objects");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, null, null));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Methods"));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Constructors"));
            smells.Merge(DataExtractor.DependeciesInParameters(data, types));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
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
            return result;
        }

        public static JObject InstantiateDestroy(JArray data)
        {
            JObject result = new JObject();
            result.Add("Name", "Instantiate - Destroy");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Instantiate", "Destroy" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        public static JObject FindMethods(JArray data)
        {
            JObject result = new JObject();
            result.Add("Name", "Find Methods");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Find", "FindGameObjectsWithTag", "FindObjectOfType", "FindGameObjectWithTag" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        public static JObject PoorStateDesign(JArray data)
        {
            JObject result = new JObject();
            result.Add("Name", "Poor State Design");
            JArray smells = new JArray();
            List<string> methods = new List<string> { "Update", "FixedUpdate" };
            //le invocazioni vengono controllate per tutto il metodo quindi dovrebbero essere gia gestite
            smells.Merge(DataExtractor.FieldsInConditionStatementInMethods(data, true, "Name", methods, "IfBlocks", "StartLine"));
            smells.Merge(DataExtractor.FieldsInConditionStatementInMethods(data, true, "Name", methods, "SwitchBlocks", "StartLine"));
            smells.Merge(DataExtractor.PropertiesInConditionStatementInMethods(data, true, "Name", methods, "IfBlocks", "StartLine"));
            smells.Merge(DataExtractor.PropertiesInConditionStatementInMethods(data, true, "Name", methods, "SwitchBlocks", "StartLine"));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        public static JObject VelocityChange(JArray data)
        {
            //TODO: Eseguire la ricerca su RIGIDBODY
            //ricercare tutte le variabili e i fields di tipo RigidBody
            //Verificare se .velocity e .angularVelocity del rigidBody vengono assegnati
            JObject result = new JObject();
            JArray smells = new JArray();
            result.Add("Name", "Velocity Change");
            smells.Merge(DataExtractor.VariablesFromData(data, null, "Kind", new List<string> { "Assignment" }, "Name", new List<string> { ".velocity", ".angularVelocity" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }
    }
}


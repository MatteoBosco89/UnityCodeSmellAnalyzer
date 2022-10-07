using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace CodeSmellFinder
{
    /// <summary>
    /// Static class containing all the methods to search a specified smell
    /// </summary>
    public class SmellDetector
    {

        public static JArray DetectAllSmellsFromList(JArray data, List<string> methodsName)
        {
            JArray results = new JArray();
            SmellDetector s = new SmellDetector();
            foreach (string method in methodsName)
            {
                results.Add(InvokeSmellMethods(data, method));
            }
            return results;
        }

        public static JArray DetectAllSmells(JArray data)
        {
            JArray results = new JArray();
            results.Add(StaticCoupling(data));
            results.Add(DependenciesBetweenObjects(data));
            results.Add(WeakTemporization(data));
            results.Add(InstantiateDestroy(data));
            results.Add(FindMethods(data));
            results.Add(PoorStateDesign(data));
            results.Add(ImproperCollider(data));
            results.Add(LackOfSeparationOfConcern(data));
            results.Add(SingletonPattern(data));
            results.Add(VelocityChange(data));
            results.Add(CountinuouslyCheckingPositionRotation(data));
            return results;
        }

        public static JObject InvokeSmellMethods(JArray data, string name)
        {
            JObject result = new JObject();
            SmellDetector s = new SmellDetector();
            MethodInfo method = s.GetType().GetMethod(name);
            if (method != null)
            {
                object[] args = { data };
                result = (JObject)method.Invoke(s, args);
            }
            return result;
        }
        /// <summary>
        /// Method to search for all smell of type Static couplig. The smell is present inside a compilation unit if 
        /// the classes inside contains a fild with attribute SerializeField and the type of the field is one of the 
        /// classes of the project
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject StaticCoupling(JArray data)
        {
            Console.Write("Searching Static Coupling Smell...");
            List<string> types = Utility.GetAllType(data, new List<string> { "Classes", "Interfaces" }, "Name");
            List<string> attributesList = new List<string> { "SerializeField" };
            JObject result = new JObject();
            result.Add("Name", "Static Couplig");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, "Attributes", attributesList));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all smell of type Weak Temporization. The smell is present if the classes use the Update method and
        /// if the classes use the Time.time instead of Time.deltatime for all operation relative to time inside the FixedUpdate Method.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject WeakTemporization(JArray data)
        {
            Console.Write("Searching Weak Temporization Smell...");
            List<string> methods = new List<string> { "FixedUpdate" };
            List<string> kinds = new List<string> { "Assignment", "Definition" };
            List<string> names = new List<string> { "Time.time" };
            JObject result = new JObject();
            result.Add("Name", "Weak Temporization");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.MethodsWithName(data, "Name", new List<string> { "Update" }));
            smells.Merge(DataExtractor.FindArgumentsInInvocation(data, "Name", methods, names));
            smells.Merge(DataExtractor.VariableFromMethods(data, "Name", methods, "Kind", kinds, "Assignment", names));
            smells.Merge(DataExtractor.FindVariableInIvocations(data, "Name", methods, "Kind", kinds, "Assignment", names));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type Improper collider. The smell is present inside a class if that classe uses object of type 
        /// MeshCollider instead of the standard collider present in unity.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject ImproperCollider(JArray data)
        {
            Console.Write("Searching Improper Collider Smell...");
            List<string> classesFound = Utility.AllClassInheritances(data, "UnityEngine.MeshCollider");
            List<string> types = new List<string> { "UnityEngine.MeshCollider" };
            List<string> methods = new List<string> { "GetComponent" };
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
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search the smells of type Dependecies between objects. The smell is found inside a class if the class
        /// uses object of other classes inside the project.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject DependenciesBetweenObjects(JArray data)
        {
            Console.Write("Searching Dependencies BetweenObjects Smell...");
            List<string> types = Utility.GetAllType(data, new List<string> { "Classes", "Interfaces" }, "Name");
            JObject result = new JObject();
            List<string> methods = new List<string> { "GetComponent" };
            result.Add("Name", "Dependency Between Objects");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, null, null));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Methods"));
            smells.Merge(DataExtractor.DependeciesInMethods(data, types, methods, "Constructors"));
            smells.Merge(DataExtractor.DependeciesInParameters(data, types));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type Lack of separation of concern. The smell is found inside a class if
        /// that class uses multiple Object of the same library. For example the use of multiple object of the library of UnityEngine
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject LackOfSeparationOfConcern(JArray data)
        {
            Console.Write("Searching Lack of separation of concern Smell...");
            List<string> lib = new List<string> { "UnityEngine." };
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
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search for all the smell of type Singleton Patter. The smell is present inside a class if the class
        /// implements the singleton pattern. To verify if a class implements the pattern the following condition needs to check:
        /// - the class have a field of the same kind of the class itself
        /// - the class have a protected/private constructor
        /// - the class have a method that returns the field of the same type of the class
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject SingletonPattern(JArray data)
        {
            Console.Write("Searching Singleton Pattern Smell...");
            JObject result = new JObject();
            result.Add("Name", "Singleton Pattern");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindSingleton(data, "Modifiers", new List<string> { "static" }, new List<string> { "protected", "private" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type Instantiate and Destroy. The smell is present inside a class if that class contain inside the 
        /// methods Update or FixedUpdate an invocation of the methods Instantiate or Destroy.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject InstantiateDestroy(JArray data)
        {
            Console.WriteLine("Searching Invocation to Instantiate and Destroy Smell...");
            JObject result = new JObject();
            result.Add("Name", "Instantiate - Destroy");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Instantiate", "Destroy" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type FindMethods. The smell is present inside a class if that class contain inside the 
        /// methods Update or FixedUpdate an invocation of the methods Find of the UnityEngine library.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject FindMethods(JArray data)
        {
            Console.WriteLine("Searching Invoation to Find Methods Smell...");
            JObject result = new JObject();
            result.Add("Name", "Find Methods");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Find", "FindGameObjectsWithTag", "FindObjectOfType", "FindGameObjectWithTag" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type Poor state design. The smell is present inside a class if that class uses condition blocks to 
        /// check for the status of the class inside the methods Update and FixedUpdate
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject PoorStateDesign(JArray data)
        {
            Console.Write("Searching Poor state management Smell...");
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
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search all the smell of type Velocy Change. The smell is present inside a class if that classe modify the velocity of a 
        /// Rigidbody object directly
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject VelocityChange(JArray data)
        {
            Console.Write("Searching Improper Velocity Change Smell...");
            JObject result = new JObject();
            JArray smells = new JArray();
            result.Add("Name", "Velocity Change");
            smells.Merge(DataExtractor.FindPropertiesChange(data, "UnityEngine.Rigidbody", new List<string> { ".velocity", ".angularVelocity" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
        /// <summary>
        /// Method to search for all smell of type Continuously Checking Position/Rotation. The smell is present inside a class
        /// if that class checks continuously the variable of type Transform.Position, Trasform.Rotation inside the Update and FixedUpdate
        /// methods and all the invocations inside that methods.
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>A Jobject containing the result of the analisys</returns>
        public static JObject CountinuouslyCheckingPositionRotation(JArray data)
        {
            Console.Write("Searching Continuous check of position and rotation Smell...");
            JObject result = new JObject();
            JArray smells = new JArray();
            result.Add("Name", "Check position/rotation");
            smells.Merge(DataExtractor.CheckPropertiesInInvocation(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "UnityEngine.Transform.position", "UnityEngine.Transform.rotation" }, "IfBlocks"));
            smells.Merge(DataExtractor.CheckPropertiesInInvocation(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "UnityEngine.Transform.position", "UnityEngine.Transform.rotation" }, "SwitchBlocks"));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Console.WriteLine("Done!!");
            return result;
        }
    }
}


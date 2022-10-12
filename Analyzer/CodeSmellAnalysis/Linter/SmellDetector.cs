using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using CSharpAnalyzer;
using System.Xml.Linq;

namespace CodeSmellFinder
{
    /// <summary>
    /// Static class containing all the methods to search a specified smell
    /// </summary>
    public class SmellDetector
    {
        /// <summary>
        /// Execute all the given methods using reflection
        /// </summary>
        /// <param name="data">The dataset</param>
        /// <param name="methodsName">The list of methods to invoke</param>
        /// <returns>The result of all operation inside a JArray</returns>
        public static JArray DetectAllSmellsFromList(JArray data, List<string> methodsName)
        {
            JArray results = new JArray();
            foreach (string method in methodsName)
            {
                results.Add(InvokeSmellMethods(data, method));
            }
            return results;
        }
        /// <summary>
        /// Test only, execute all the methods that search for a smell
        /// </summary>
        /// <param name="data">The JArray Containing the dataset</param>
        /// <returns>The result of all operation inside a JArray</returns>
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
        /// <summary>
        /// Return the name of all the methods that search for a smell
        /// </summary>
        /// <returns></returns>
        public static List<string> ExposeAllMethods()
        {
            List<string> methods = new List<string>();
            SmellDetector s = new SmellDetector();
            MethodInfo[] method = s.GetType().GetMethods();
            foreach(MethodInfo m in method)
            {
                if(m.ReturnType == typeof(JObject))
                {
                    if(m.Name != "InvokeSmellMethods")methods.Add(m.Name);
                }
            }
            return methods;
        }
        /// <summary>
        /// Ivoke a method using reflections
        /// </summary>
        /// <param name="data">The JArray containing the dataset</param>
        /// <param name="name">The name of the method to invoke</param>
        /// <returns>The result of the invocation</returns>
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Static Coupling Smells...");
            List<string> types = Utility.GetAllType(data, new List<string> { "Classes", "Interfaces" }, "Name");
            List<string> attributesList = new List<string> { "SerializeField" };
            JObject result = new JObject();
            result.Add("Name", "Static Coupling");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FieldDependeciesInCompilationUnit(data, "Type", types, "Attributes", attributesList));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Weak Temporization Smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Improper Collider Smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Dependency Between Object Smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Lack Of Seapration Of Concern Smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Singleton Pattern Smells...");
            JObject result = new JObject();
            result.Add("Name", "Singleton Pattern");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindSingleton(data, "Modifiers", new List<string> { "static" }, new List<string> { "protected", "private" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Instantiate and Destroy Smells...");
            JObject result = new JObject();
            result.Add("Name", "Instantiate - Destroy");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Instantiate", "Destroy" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Find Methods Smells...");
            JObject result = new JObject();
            result.Add("Name", "Find Methods");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.FindInvocationSmell(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "Find", "FindGameObjectsWithTag", "FindObjectOfType", "FindGameObjectWithTag" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Poor State Design Smells...");
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
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Rigidbody Velocity Change Smell...");
            JObject result = new JObject();
            JArray smells = new JArray();
            result.Add("Name", "Velocity Change");
            smells.Merge(DataExtractor.FindPropertiesChange(data, "UnityEngine.Rigidbody", new List<string> { ".velocity", ".angularVelocity" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
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
            Logger.Log(Logger.LogLevel.Debug, "Searching Continuously Checking Position/Rotation...");
            JObject result = new JObject();
            JArray smells = new JArray();
            result.Add("Name", "Check position/rotation");
            smells.Merge(DataExtractor.CheckPropertiesInInvocation(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "UnityEngine.Transform.position", "UnityEngine.Transform.rotation" }, "IfBlocks"));
            smells.Merge(DataExtractor.CheckPropertiesInInvocation(data, new List<string> { "Update", "FixedUpdate" }, new List<string> { "UnityEngine.Transform.position", "UnityEngine.Transform.rotation" }, "SwitchBlocks"));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            return result;
        }
        /// <summary>
        /// This methods search for heavy weight physics computation. The smell is present if physic computations are made inside the update or
        /// fixedupdate
        /// </summary>
        /// <param name="data">The dataset containing all the compilation unit of the project</param>
        /// <returns>>A Jobject containing the result of the analisys</returns>
        public static JObject HighPhysicsComputation(JArray data)
        {
            //per dividere gli smell posso anche 
            Logger.Log(Logger.LogLevel.Debug, "Searching High Physics Computation...");
            JObject result = new JObject();
            result.Add("Name", "High Physic Computaions");
            JArray smells = new JArray();
            smells.Merge(DataExtractor.ChangesToVariableOfTypeInMethods(data, new List<string> { "Update", "FixedUpdate" }, "Assignment", "UnityEngine.Rigidbody", new List<string> { ".position", ".rotation" }));
            result.Add("Occurency", smells.Count());
            result.Add("Smells", smells);
            Logger.Log(Logger.LogLevel.Debug, "Done!");
            return result;
        }
    }
}


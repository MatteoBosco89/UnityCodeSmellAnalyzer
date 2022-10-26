using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace MetaSmellDetector
{
    public class MetaExtractor
    {
        /// <summary>
        /// Get the names of all the mthods that extract smells
        /// </summary>
        /// <returns>A list containing the names of the methods</returns>
        public static List<string> SmellsMethods()
        {
            List<string> methods = new List<string>();
            MetaExtractor m = new MetaExtractor();
            MethodInfo[] mList = m.GetType().GetMethods();
            foreach(MethodInfo mi in mList)
            {
                if (mi.ReturnType != typeof(JObject)) continue;
                if (mi.Name == "InvokeMethods") continue;
                methods.Add(mi.Name);
            }
            return methods;
        }

        /// <summary>
        /// This method uses reflection for invoke specificed methods for search smells
        /// </summary>
        /// <param name="lines"> The line contanning the parameters to invoche the method</param>
        /// <param name="data"> The dataset to analyze</param>
        public static JObject InvokeMethods(string lines, JArray data) 
        {
            string[] s = lines.Split(',');
            string name = s[0];
            MetaExtractor ex = new MetaExtractor();
            MethodInfo m = ex.GetType().GetMethod(name);
            if(m!= null)
            {
                List<string> param = new List<string>();
                for(int i=1; i < s.Length; i++)
                {
                    param.Add(s[i]);

                }
                object[] args = { data,param };
                JObject result = m.Invoke(ex, args) as JObject;
                return result;
            }
            return null;
        }

        /// <summary>
        /// This method search smell "Heavy Phisics Computation". The smell is present if an object uses  a Rigidbody with m_CollisionDetection set
        /// to 1 or 2
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject HeavyPhisics(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Heavy Physics Computation...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Heavy Physics Computation ");
            if (paramList.Count == 5)
            {
                foreach (JObject c in data)
                {
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]");
                    if (token.Count() > 0)
                    {
                        token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[1]} {paramList[2]} '{paramList[3]}'|| '{paramList[4]}')]");
                        if (token.Count() > 0)
                        {
                            JObject jo = new JObject();
                            jo.Add("FilePath", c["file_path"].ToString());
                            jo.Add("Name", c["name"].ToString());
                            smells.Add(jo);
                        }
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell "SubOptimal", this method search all smell with activate rbaked light if object have Animator component (is dynamic)
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject SubOptimal(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching SubOptimal Expensive Lights...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "SubOptimal Expensive Lights");
            if (paramList.Count == 4)
            {
                foreach (JObject c in data)
                {
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]");
                    if (token.Count() > 0)
                    {
                        token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[1]}{paramList[2]}{paramList[3]})]");
                        if (token.Count() > 0)
                        {
                            JObject jo = new JObject();
                            jo.Add("FilePath", c["file_path"].ToString());
                            jo.Add("Name", c["name"].ToString());
                            smells.Add(jo);
                        }
                    }
                }          
            }
            Logger.Log(Logger.LogLevel.Debug,"Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell "SubOptimal1", this function search all smell with activate real-time light if doesn't have animator (is static)
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject SubOptimal1(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching SubOptimal Expensive Lights with enable LightRealTime...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "SubOptimal Expensive Lights with enable LightRealTime");
            if (paramList.Count == 4)
            {
                foreach (JObject c in data)
                {
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]");
                    if (token.Count() <= 0)
                    {
                        token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[1]}{paramList[2]}{paramList[3]})]");
                        if (token.Count() > 0)
                        {
                            JObject jo = new JObject();
                            jo.Add("FilePath", c["file_path"].ToString());
                            jo.Add("Name", c["name"].ToString());
                            smells.Add(jo);
                        }
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell with light of type real time activate and greater than a threshold
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject LightSmell(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching Lack of optimization when drawing-rendering...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Lack of optimization when drawing-rendering");
            if (paramList.Count == 3)
            {
                foreach (JObject c in data)
                {
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]} {paramList[1]} {paramList[2]})]");
                    foreach (JToken p in token)
                    {
                        JObject jo = new JObject();
                        jo.Add("FilePath", c["file_path"].ToString());
                        jo.Add("Name", c["name"].ToString());
                        smells.Add(jo);
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell with activate Animator 
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject MultipleAnimator(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching Multiple animators for a single object smells...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Multiple animators for a single object");
            if (paramList.Count == 1)
            {
                foreach (JObject c in data)
                {
                    List<string> st = new List<string>();
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]");
                    foreach (JToken p in token)
                    {
                        st.Add(p.ToString());
                    }
                    if (st.Count() > 1)
                    {
                        JObject jo = new JObject();
                        jo.Add("FilePath", c["file_path"].ToString());
                        jo.Add("Name", c["name"].ToString());
                        jo.Add("NumAnimator", st.Count());
                        smells.Add(jo);
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell "static coupling" with guid associates with other gameobject
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject InstanceCounter(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching Static Coupling Smells...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Static Coupling Smells");
            if (paramList.Count == 2)
            {
                int threshold = 0;
                try
                {
                    threshold = int.Parse(paramList[1]);
                }
                catch (Exception)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Parameter not a Int");
                }
                foreach (JObject c in data)
                {
                    List<string> st = new List<string>();
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]..{paramList[0]}");
                    foreach (JToken p in token)
                    {
                        st.Add(p.ToString());
                    }
                    if (st.Count() > threshold)
                    {
                        JObject jo = new JObject();
                        jo.Add("FilePath", c["file_path"].ToString());
                        jo.Add("Name", c["name"].ToString());
                        jo.Add("NumReference", st.Count());
                        smells.Add(jo);
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell with Anystate in the animator 
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject AnyState(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching AnyState Smells...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Anystate Smells");
            if (paramList.Count == 1) { 
                foreach (JObject c in data)
                {
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]..{paramList[0]}");
                    foreach (JToken p in token)
                    {
                        if (p is JArray)
                        {
                            if (p.Count() > 0)
                            {
                                JObject jo = new JObject();
                                jo.Add("FilePath", c["file_path"].ToString());
                                jo.Add("Name", c["name"].ToString());
                                smells.Add(jo);
                            }
                        }
                    }
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search with num_components or file_size greather than treshold
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject QuerySearch(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Searching Bloated Assets Smells...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Bloated Assets Smells");
            if (paramList.Count == 3)
            {
                try
                {
                    int result1 = Int32.Parse(paramList[2]);
                    List<JToken> tokens = data.SelectTokens($"$.[?(@.{paramList[0]} {paramList[1]} {result1})]").ToList();
                    foreach (JToken c in tokens)
                    {
                        JObject jo = new JObject();
                        jo.Add("FilePath", c["file_path"].ToString());
                        jo.Add("Name", c["name"].ToString());
                        jo.Add("NumComponents", c["num_components"].ToString());
                        smells.Add(jo);
                    }
                }
                catch (FormatException)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Parameter is not an int");
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// This method search smell contains meshcollider
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject SearchSmellByParam(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search Mesh Collider Smells...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Mesh Collider Smells");
            if (paramList.Count == 1)
            {
                var res = data.SelectTokens($"$.[?(@..{paramList[0]})]");
                JArray results = new JArray(res);
                foreach (JToken obj in results)
                {
                    JObject s = new JObject();
                    s.Add("FileName", obj["file_path"]);
                    s.Add("Name", obj["name"]);
                    s.Add("Type", obj["type"]);
                    smells.Add(s);
                }
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }
        /// <summary>
        /// This method search animation with too many key frame
        /// </summary>
        /// <param name="data">The dataset to analyze</param>
        /// <param name="paramList">The list of parameters</param>
        public static JObject TooManyKeyFrames(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search Too Many Key Frame Smell...");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Too Many Key Frame");
            if(paramList.Count == 1)
            {
                int threshold = 0;
                try
                {
                    threshold = int.Parse(paramList[0]);
                }
                catch (Exception)
                {
                    Logger.Log(Logger.LogLevel.Debug, "Threshold is not a number, no threshold set");
                }
                var res = data.SelectTokens("$..[?(@.type == 'anim')]");
               
                foreach (JToken obj in res)
                {
                    res = obj.SelectTokens("$..m_Curve");
                    if (res.Count() > 0)
                    {
                        JToken animData = res.ElementAt(0);
                        var res1 = animData.SelectTokens("$..time");
                        int numKey = res1.Count();
                        if(numKey >= threshold)
                        {
                            JObject s = new JObject();
                            s.Add("FileName", obj["file_path"]);
                            s.Add("Name", obj["name"]);
                            s.Add("Type", obj["type"]);
                            s.Add("KeyFrames", numKey);
                            smells.Add(s);
                        }
                    }
                }
                
            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Security.Policy;

namespace MetaSmellDetector
{
    public class MetaExtractor
    {
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
        /// This method using reflection for invoke specificed functions for research smell
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="data"></param>
        public static JObject InvokeMethods(string lines, JArray data) {

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
        /// this functions search smell "Heavy Phisics Computation", this function search all smell with activate rigibody
        /// and m_CollisionDetection must be grater than 1 or 2
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject HeavyPhisics(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Heavy Phisics Computation ");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Heavy Phisics Computation ");
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

                            jo.Add("file_path", c["file_path"].ToString());
                            jo.Add("name", c["name"].ToString());
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
        /// this functions search smell "SubOptimal", this function search all smell with activate rbaked light if object dynamic
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject SubOptimal(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "SubOptimal Expensive Lights ");
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
                         
                            jo.Add("file_path", c["file_path"].ToString());
                            jo.Add("name", c["name"].ToString());
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
        /// this functions search smell "SubOptimal1", this function search all smell with activate real-time light if object static
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>
        /// 
       public static JObject SubOptimal1(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "SubOptimal Expensive Lights with enable LightRealTime.....");
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
                            jo.Add("file_path", c["file_path"].ToString());
                            jo.Add("name", c["name"].ToString());
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
        /// this function search smell with light real time activate and greater than a threshold
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject LightSmell(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Lack of optimization when drawing/rendering.....");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Lack of optimization when drawing/rendering");
            if (paramList.Count == 3)
            {

                foreach (JObject c in data)
                {

                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]} {paramList[1]} {paramList[2]})]");
                    foreach (JToken p in token)
                    {
                        JObject jo = new JObject();
                        jo.Add("file_path", c["file_path"].ToString());
                        jo.Add("name", c["name"].ToString());
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
        /// this function search smell with activate Animator 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject MultipleAnimator(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Multiple animators for a single object.....");
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
                    if (st.Count() > 0)
                    {
                        JObject jo = new JObject();
                        jo.Add("file_path", c["file_path"].ToString());
                        jo.Add("name", c["name"].ToString());
                        jo.Add("Occurrency", st.Count());
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
        /// this function search smell "static coupling" with guid associates with other gameobject
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject InstanceCounter(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search Static Coupling Smells.....");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Static Coupling Smells");
            if (paramList.Count == 1)
            {
               
                foreach (JObject c in data)
                {
                    
                    List<string> st = new List<string>();
                    var token = c.SelectTokens($"$..COMPONENTS[?(@..{paramList[0]})]..{paramList[0]}");
                    foreach (JToken p in token)
                    {
                        st.Add(p.ToString());

                    }

                    if (st.Count() > 0)
                    {
                        JObject jo = new JObject();
                        jo.Add("file_path", c["file_path"].ToString());
                        jo.Add("name", c["name"].ToString());
                        jo.Add("Occurrency", st.Count());
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
        /// this function search smell with Anystate in the animator 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject AnyState(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search AnyState Smells.....");
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
                            jo.Add("file_path", c["file_path"].ToString());
                            jo.Add("name", c["name"].ToString());

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
        /// this function search with num_components or file_size greather than treshold
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject QuerySearch(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search Bloated Assets Smells.....");
            JArray smells = new JArray();
            JObject result = new JObject();
            result.Add("Name", "Bloated Assets Smells");
            if (paramList.Count == 3){
            int result1 = 0;

            try
            {

                result1 = Int32.Parse(paramList[2]);

            }
            catch (FormatException)
            {
                Console.WriteLine("Not Valid");
            }

            List<JToken> tokens = data.SelectTokens($"$.[?(@.{paramList[0]} {paramList[1]} {result1})]").ToList();



            foreach (JToken c in tokens)
            {
                JObject jo = new JObject();
                jo.Add("file_path", c["file_path"].ToString());
                jo.Add("name", c["name"].ToString());
                jo.Add("num_components", c["num_components"].ToString());

                smells.Add(jo);

            }

            }
            Logger.Log(Logger.LogLevel.Debug, "Done!!");
            result.Add("Occurrency", smells.Count());
            result.Add("Smells", smells);
            return result;
        }

        /// <summary>
        /// this function search smell contains meshcollider
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paramList"></param>

        public static JObject SearchSmellByParam(JArray data, List<string> paramList)
        {
            Logger.Log(Logger.LogLevel.Debug, "Search Mesh Collider Smells.....");
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
    
        
    }
}


using UnityAnalyzer;

namespace UnityFileProva
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UnityData d = new UnityData("Files\\Futuristic.fbx.meta");
            d.SaveDataToJsonFile(@"Files\\Prova.json");
        }
    }
}

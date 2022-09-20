using System;
namespace FileProcess
{
    public class LoadFile
    {
        public static void Main()
        {
            try
            {
                string [] dir =  {".meta", ".fbx" };
                // prendiamo i file con estensione .rtf
            List<string> files = Directory.GetFiles(@"/Users/patriziadecristofaro/Desktop/UnlimitedArena/Assets", "*.*", SearchOption.AllDirectories).Where(files => dir.Any(files.ToLower().EndsWith)).ToList();

                foreach(string a in files)
                {
                    Console.WriteLine(a);
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }

}



using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        public int s = 10;
        public int t = 5;

        void Update()
        {
            t = Time.deltaTime;
        }

        static void Main(string[] args)
        {
            Update();
            for(int j = 1; j < 10; j++)
            {
                j = j;
            }
            int c = 1;
            int a = 1;
            int b = 2;
            a = c;
            c = 3;
            b = c;
            a = s;
            b *= t;
            Stampa(b);
        }

        public void Stampa(int param)
        {
            int outside = t;
            Console.WriteLine(param);
        }

        public string Method1()
        { 
            int var1 = 10; 
        }

    }
}
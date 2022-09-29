using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Security.Permissions;

namespace RoslynExample
{
    public interface IStrunz
    {
        string Name { get; }
        public void Metodo();
    }

    public class Class23
    {

    }

    public class Class1
    {
        protected float vel = 0.0f;
        int finalVel = 0;
        Class2 c2 = new Class2();
        string name;


        public Class1(string name)
        {
            this.name = name;
        }

        void Update()
        {
            vel = c2.Method2();
        }

        void FixedUpdate()
        {
            c2.Method1();
            c2.Method5(1);
        }

        void Awake()
        {
            int number = 0;
            List<int> numbers = new List<int> { 0, 2, 5 };
            switch (number)
            {
                case 0: 
                    foreach(int n in numbers) Console.WriteLine(n);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default: 
                    break;
            }
        }

    }

    public static class Class3
    {
        public static void Method4(string s, int i = 1) { }
    }

}


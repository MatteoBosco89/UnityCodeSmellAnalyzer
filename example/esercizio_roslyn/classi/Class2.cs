using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RoslynExample
{
    class Class2 
    {
        public Class2() { }


        public void Method1()
        {
            Console.WriteLine("Method1 called");
        }

        public int Method2()
        {
            Method1();
            return 0;
        }

    }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RoslynExample
{
    [Serializable]
    class Class2 
    {

        protected int field1 = 1;

        public int Field1
        {
            get { return field1; }
        }

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
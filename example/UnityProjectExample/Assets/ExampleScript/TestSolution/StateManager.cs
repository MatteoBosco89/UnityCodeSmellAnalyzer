using System;
using System.Text.Json.Serialization;

namespace Test
{
    public abstract class StateManager
    {
        protected int a;
        protected int b;
        protected int c;

        [JsonIgnore]
        public int A { get { return a; } set { a = value; } }
        public int B
        {
            get { return b; }
            set { b = value; while (b < 0) b++; }
        }
        public int C { get; set; } = 0;

        public void SwitchMeth()
        {
            int aa = 10;
            switch (Meth22())
            {
                case 0:
                case 2:
                    Console.WriteLine(0); break;
                case 1: while(B > 10) B -= 1; break;
                default: Console.WriteLine(10); break;
            }
        }

        public int Meth22() => 10;
        public void Meth23() { }

        public int MethExp() => a + Meth22();
        public string Ciaone() => "Ciaone" + "";
        

        public StateManager(int a)
        {
            this.a = a;
        }
    }
}
using System;
using System.Runtime.ExceptionServices;
using System.Text.Json.Serialization;

namespace Test
{

    public interface ITest
    {
        int P { get; }
    }

    public abstract class StateManager : ITest
    {
        protected int a;
        protected int b;
        protected int c;
        protected int d;

        public int D => d;

        [JsonIgnore]
        public int A { get { return a; } set { a = value; } }
        public int B
        {
            get { return b; }
            set { b = value; while (Meth22() < 0 && Meth22() > 0) b++; }
        }
        public int C { get; set; } = 0;

        public int P => a + d;

        public void MethodArg(int a, bool b, State s)
        {
            while ((a = c) == 0) Console.WriteLine(a);
        }
        public void SwitchMeth()
        {
            
            MethodArg(1, false, new State(1, 2));
            int bbb = 0;
            if(A > 0 && bbb < 0 || c > 1) { Console.WriteLine("A"); }

            int aa = A;
            switch (A)
            {
                case 0:
                case 2:
                    Console.WriteLine(0); break;
                case 1: while(B > 10) B -= 1; break;
                default: Console.WriteLine(10); break;
            }

            try
            {
                Meth23();
            }
            catch (Exception) when (A == 1)
            {
                Meth22();
                try
                {
                    MethExp();
                }
                catch (Exception) { Meth23(); }
            }

            try
            {
                Meth23();
            }
            finally
            {
                Meth22();
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
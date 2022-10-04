using System;
using System.Text.Json.Serialization;

namespace Test
{
    public class StateInState<T> where T : new()
    {

        protected int aaaaa;

        public StateInState(int b) => aaaaa = 1;

        public StateInState() { }

        public void Method1(string s)
        {
            InnerState i = new InnerState();
            i.Ciaone2().Method3();
        }

        public class InnerState
        {
            protected int d;
            public int D { get { return d; } protected set { d = value; } }
            public InnerState()
            {
                this.d = 1;
                Method2();
                this.d = Method3();

                while (Method3() < 0 && D > 0) { Method2(); }
                
            }
            
            public int M() { return 12; }
            public void Method2() { }
            public int Method3() { return D; }
            public InnerState(int d) => this.d = Method3();
            public InnerState Ciaone2() => new InnerState();
        }



    }
}



using System;

namespace Test
{
    public class StateInState<T> where T : new()
    {
        public StateInState() { }

        public void Method1(string s) { }

        public class InnerState
        {
            protected int d;
            public InnerState()
            {
                this.d = 1;
                Method2();
                this.d = Method3();

                while (true) { Method2(); }
                
            }
            public void Method2() { }
            public int Method3() { return 10; }
            public InnerState(int d) => this.d = Method3();
            public InnerState Ciaone2() => new InnerState();
        }



    }
}



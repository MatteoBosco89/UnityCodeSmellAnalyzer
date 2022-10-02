using System;

namespace Test
{
    public class State : StateManager
    {
        protected int b;
        [Obsolete] public State(int a, int b) : base(a) => this.b = b;

        public State(int a, int b, int c) : base(a)        {
            this.b = b;
        }

        

    }
}


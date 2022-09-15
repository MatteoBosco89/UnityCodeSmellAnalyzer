using System;

namespace Element
{
    public class SimpleElement:Element
    {
        protected string value;

        public string Value { get { return value; } }

        public SimpleElement(string valueng)        {
            this.value = valueng;
        }

    }
}


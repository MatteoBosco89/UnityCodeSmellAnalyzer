using System;

namespace Element
{
    /*
     * This represent a simple element inside the unity file
     */
    public class SimpleElement:Element
    {
        protected string value;

        public string Value { get { return value; } }

        public SimpleElement(string valueng)        {
            this.value = valueng;
        }

    }
}


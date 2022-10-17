using System;

namespace Element
{
    /// <summary>
    /// This class represent a single entry inside a unity meta data dictionary
    /// </summary>
    [Serializable]
    public class SimpleElement : Element
    {
        protected string value;

        public string Value { get { return value; } }

        public SimpleElement(string valueng){
            this.value = valueng;
        }

    }
}


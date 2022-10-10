using System;

namespace Element
{
    /// <summary>
    /// This class is the super class representing the element inside the unity meta data file
    /// </summary>
    public class Element
    {
 
        virtual public void Print() { }
        virtual public int LoadNormalDictionary(string[] lines, int i, string cmpId) { return 0; }
        virtual public void LoadParenthesisDictionary(string line) { }
        virtual public int LoadSpecialDictionary(string[] lines, int i) { return 0; }
        virtual public int LoadDictionaryWithSpecialElements(string[] lines, int i, string cmpId) { return 0; }

    }
}


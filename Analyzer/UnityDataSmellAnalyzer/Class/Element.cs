using System;

/*
 * This namespace contains the rappresentation of the elements inside all unity files
 */
namespace Element
{
    /*
     * The element class is the base class representing all the elements inside the unity file
     */
    public class Element
    {
 
        virtual public void Print() { }
        virtual public int LoadNormalDictionary(string[] lines, int i, string cmpId) { return 0; }
        virtual public void LoadParenthesisDictionary(string line) { }
        virtual public int LoadSpecialDictionary(string[] lines, int i) { return 0; }
        virtual public int LoadDictionaryWithSpecialElements(string[] lines, int i, string cmpId) { return 0; }

    }
}


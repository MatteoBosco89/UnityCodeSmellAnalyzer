using System;
using System.Collections;
using System.Collections.Generic;
using Test.Input.TestInput;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
public class Inputs : MonoBehaviour, InterfacesTest
{
    [SerializeField] Animation anim;
    [SerializeField] GameObject o;

    public int Number { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Number1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Number2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Print()
    {
        List<string> list = new List<string> { "hello", "smell", "computer" };
        foreach (string s in list) Console.WriteLine(s);
        for (int i = 0; i < list.Count; i++)
        {
            list[i] += "we";
            list[i].Split('i');
        }
    }

    public void PrintError()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim.PlayQueued("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        o = Instantiate(o);
        o.transform.position = o.transform.position + new Vector3(10, 10, 10);
        Destroy(o);
    }

    private void FixedUpdate()
    {
        StateManager s = new StateManager();
        s.Test2();
    }
}

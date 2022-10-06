using MyGame;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Input.TestInput;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using a;
using Sphere;

namespace a
{
    public class Inputs : MeshCollider, InterfacesTest
    {
        [SerializeField] Animation anim;
        [SerializeField] GameObject o;
        MeshCollider mesh;
        Rigidbody r = new Rigidbody();

        public int Number { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Number1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Number2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Print() { }

        public void Print(SpehereManager s)
        {
            List<string> list = new List<string> { "hello", "smell", "computer" };
            foreach (string s1 in list) Console.WriteLine(s);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] += "we";
                list[i].Split('i');
            }
            MeshCollider m = new MeshCollider();
            m = GetComponent<MeshCollider>();
            r.velocity = new Vector3(0, 0, 0);
            r.angularVelocity = new Vector3(0, 0, 0);
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
            o = GameObject.Find("Player");
        }

        private void FixedUpdate()
        {
            StateManager s = new StateManager();
            CubeManager c = new CubeManager();
            c.Smellami();
        }
    }
}
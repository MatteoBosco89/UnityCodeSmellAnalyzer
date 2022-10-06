using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;
namespace Test
{
    using UnityEngine.Animations;
    using System.IO;



    namespace Input
    {
        public abstract class Prova: Inputs
        {
            protected GameObject o;

            public abstract GameObject O { get; set; }
            public abstract string Name { get; }
            public abstract string Description { get; }

            public abstract void Print(int x, string j);
            public abstract void Update(GameObject o);
        }

        namespace TestInput
        {
            using UnityEngine.Rendering;
            using System.Linq;
            using System;
            using Sphere;

            public class StateManager : SuperClass
            {
                protected bool isJumping = false;
                protected bool noDamage = false;
                protected int dir = 0;
                protected bool forward = false;
                protected bool backward = false;
                protected bool left = false;
                protected bool right = false;
                [SerializeField] SphereCollider s;
                [SerializeField] SpehereManager spe;
                Inputs mesh;
                // Start is called before the first frame update
                public bool NoDamage { get { return noDamage; } }
                void Start()
                {
                    GetComponent<CubeManager>();
                    GetComponent<Inputs>();
                    do
                    {
                        Console.WriteLine("Hello");
                    } while (true);
                }

                // Update is called once per frame
                void Update()
                {
                    if (!isJumping) noDamage = true;
                    else if(noDamage) noDamage = false;
                    Prova();

                    switch (dir)
                    {
                        case 0:
                            forward = false;
                            backward = false;
                            left = false;
                            right = false;
                            if (NoDamage) Console.WriteLine("");
                            break;
                        case 1:
                            forward = true;
                            backward = false;
                            left = false;
                            right = false;
                            break;
                        case 2:
                            forward = false;
                            backward = false;
                            left = true;
                            right = false;
                            break;
                    }
                }

                public override void Test()
                {
                    base.Test();
                    int i = 10;
                    int j = 25;
                    int result = 1;
                    for(int k = 0; k < j; k++)
                    {
                        result = i * i;
                    }
                    MyGame.Cube.CubeHolder o = new MyGame.Cube.CubeHolder(result);
                    Instantiate(o.Cube);
                    GameObject z = GameObject.FindGameObjectWithTag("Player");
                    z.SetActive(false);
                }
                public Inputs Prova()
                {
                    if (NoDamage) return null;
                    switch (dir)
                    {
                        case 0:
                            break;
                       
                    }
                    return new Inputs();
                }

                public override void Test2()
                {
                    base.Test2();
                    Test();
                    while (isJumping)
                    {
                        int i = 0;
                        i = 10;
                        int k = 30;
                        i *= k;

                        k = i * i * k;

                        k.ToString();
                        Console.WriteLine(i * k);
                    }
                    
                }
                
            }
        }
    }
}


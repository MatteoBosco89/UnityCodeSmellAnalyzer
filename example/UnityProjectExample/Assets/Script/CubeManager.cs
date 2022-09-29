using MyGame.Cube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    namespace Cube
    {
        public class CubeHolder
        {
            [SerializeField] protected GameObject cube;
            public int num = 0;

            public GameObject Cube { get { return cube; } }
            public CubeHolder(int c) { num = c; }

        }

        public sealed class CuboPiccolo : InterfacesTest
        {
            [SerializeField, HideInInspector] private Animation anim;

            public Animation Anim { get { return anim; } set { anim = value; } }
            public int Number { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public int Number1 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
            public int Number2 { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public void Print()
            {
                throw new System.NotImplementedException();
            }

            public void PrintError()
            {
                throw new System.NotImplementedException();
            }
        }
    }
    public class CubeManager : MonoBehaviour
    {
        protected CubeHolder cube;
        // Start is called before the first frame update
        void Start()
        {
            int x = 0;
            cube = new CubeHolder(x);
        }

        // Update is called once per frame
        void Update()
        {
            GameObject o = Instantiate(cube.Cube);
            o.SetActive(true);
        }

        public void GenerateCube()
        {
            GameObject o = Instantiate(cube.Cube);
        }
    }
}


using MyGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sphere
{
    public class SpehereManager : MonoBehaviour
    {
        CubeManager cubeManager;
        // Start is called before the first frame update
        void Start()
        {
            cubeManager = FindObjectOfType<CubeManager>();
        }

        // Update is called once per frame
        void Update()
        {
            cubeManager.GenerateCube();
            cubeManager = FindObjectOfType<CubeManager>();
        }

        public GameObject GiveMeCube(string s)
        {
            Console.WriteLine(s);
            cubeManager.GenerateCube();
            return cubeManager.gameObject;
        }
    }
}


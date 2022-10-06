using MyGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;
namespace Sphere
{
    public class SpehereManager : Inputs
    {
        [SerializeField] CubeManager c;
        CubeManager cubeManager;
        protected static SpehereManager sphere;
        // Start is called before the first frame update

        SpehereManager() { }

        void Start()
        {
            cubeManager = FindObjectOfType<CubeManager>();
            GetComponent<Collider>();
            GetComponent<Rigidbody>();
            GetComponent<SpehereManager>();
            GetComponent<CubeManager>();
            MeshCollider m = new MeshCollider();
            
            
        }

        public SpehereManager GetInstance()
        {
            return sphere;
        }

        private void FixedUpdate()
        {
            float x = Time.time;
            float y = Time.deltaTime;
            y = Time.time;
            GiveMeCube("hello");
            gameObject.transform.Rotate(Time.time, 0, 0);
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
            float x = Time.time;
            gameObject.transform.Rotate(Time.time, 0, 0);
        }
    }
}


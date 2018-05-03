using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using UnityEngine;
namespace UGP
{
    public class XRPlayerController : InputController
    {

        public Transform LeftHand;
        public Transform RightHand;
        public Transform Head;
        public Transform LeftEye;
        public Transform RightEye;

        public bool XRActive;

        public override void Move(float x, float y)
        {
            throw new NotImplementedException();
        }
        
        public override void Rotate(float xRot, float yRot, float zRot)
        {
            throw new NotImplementedException();
        }

        // Use this for initialization
        void Start()
        {



        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
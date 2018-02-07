using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UGP
{
    public class MirrorRotateBehaviour : MonoBehaviour
    {
        public Button Leftbutton;
        public Button Rightbutton;
        public Transform model;
        public float turnspeed;
        public Canvas ClosetCanvas;
        public PlayerMovementBehaviour playerstater;




        public Transform GarageDoor;
        public float OpenSpeed;


        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "DoorStopper")
            {

            }

        }
        public void RotateLeft()
        {
            if (playerstater.state == PlayerState.standing)
            {
                ClosetCanvas.enabled = true;
               
              
                    var left = new Vector3(0.0f, -1 * turnspeed, 0.0f);
                    model.Rotate(left * Time.deltaTime);
                


            }



        }

     
        public void RotateRight()
        {
            if (playerstater.state == PlayerState.standing)
            {
                ClosetCanvas.enabled = true;
               
                    var right = new Vector3(0.0f, 1 * turnspeed, 0.0f);
                    model.Rotate(right * Time.deltaTime);
                
            }
        }
        void Start()
        {
            ClosetCanvas.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            ClosetCanvas.enabled = false;
            RotateLeft();
            RotateRight();
        }
    }
}
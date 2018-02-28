using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Networking;
#endif
using UnityEngine.SceneManagement;

namespace UGP
{
    public class GarageDoorBehaviour : MonoBehaviour
    {
        public PlayerMovementBehaviour playerstater;
        public PlayerState state;
        public Network Net;

        public bool opengaragedoor;
        public Transform GarageDoor;
        public float OpenSpeed;


        public Transform DoorStopper;





        public void GarageDoorOpen()
        {
            var up = new Vector3(0.0f, 0.0f, 1 * OpenSpeed);

            if (playerstater.state == PlayerState.viewing)
            {
                opengaragedoor = true;
                GarageDoor.Translate(up * Time.deltaTime);
            }
        }
        public void GarageDoorClose()
        {
            var up = new Vector3(0.0f, 0.0f, -1 * OpenSpeed);

            if (playerstater.state == PlayerState.viewing)
            {
                opengaragedoor = false;
                GarageDoor.Translate(up * Time.deltaTime);
            }
        }

      
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var MaxHeight = new Vector3(0, 10, 0);
            var MinHeight = new Vector3(0, -0.75f, 0);

          
            if(GarageDoor.position.y >= MaxHeight.y)
            {
                var maxHeight = GarageDoor.position;
                maxHeight.y = MaxHeight.y;
                GarageDoor.position = maxHeight;
                SceneManager.LoadScene("03.GarageTestTrack");
                
            }
            if (GarageDoor.position.y <= MinHeight.y)
            {
                var minHeight = GarageDoor.position;
                minHeight.y = MinHeight.y;
                GarageDoor.position = minHeight;
            }
          if(opengaragedoor == true)
            {
                GarageDoorOpen();
            }
          if(opengaragedoor == false)
            {
                GarageDoorClose();
            }
        }
    }
}
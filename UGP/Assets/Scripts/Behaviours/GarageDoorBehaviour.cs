using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.Networking;
#endif

namespace UGP
{
    public class GarageDoorBehaviour : MonoBehaviour
    {
        public GameObject playerstater;
        public GameObject triggerEventPos;

        public bool opengaragedoor;
        public Transform GarageDoor;
        public float OpenSpeed;


        public Transform DoorStopper;
        [Range(-99999.0f, 9999.0f)] public float MaxDoorOpen;
        [Range(-99999.0f, 9999.0f)] public float MinDoorOpen;


        public void GarageDoorOpen()
        {
            var door_z_value = GarageDoor.transform.position.z;
            if(door_z_value < MaxDoorOpen)
            {
                GarageDoor.Translate(Vector3.forward * Time.deltaTime);
            }

            //var up = new Vector3(0.0f, 0.0f, 0.25f * OpenSpeed);

            //if (playerstater.transform.position == triggerEventPos.transform.position)
            //{
            //    opengaragedoor = true;
            //    GarageDoor.Translate(up * Time.deltaTime);
            //}
        }
        public void GarageDoorClose()
        {

            var door_z_value = GarageDoor.transform.position.z;
            if (door_z_value > MinDoorOpen)
            {
                GarageDoor.Translate(-Vector3.forward * Time.deltaTime);
            }


            //var up = new Vector3(0.0f, 0.0f, -0.25f * OpenSpeed);

            //if (playerstater.transform.position != triggerEventPos.transform.position)
            //{
            //    opengaragedoor = false;
            //    GarageDoor.Translate(up * Time.deltaTime);
            //}
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


        void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                GarageDoorOpen();
            }
        }

        void OnTriggerExit(Collider other)
        {
            GarageDoorClose();
        }

    }
}
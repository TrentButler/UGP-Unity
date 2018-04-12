using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor.Networking;
#endif

namespace UGP
{
    public class GarageDoorBehaviour : MonoBehaviour
    {
        public GameObject playerstater;
        public GameObject triggerEventPos;

        //[SyncVar(hook = "OnOpenGarageDoorChange")]
        public bool opengaragedoor;
        public Transform GarageDoor;
        public float OpenSpeed;
        public Vector3 Direction;

        public GameObject DoorStop;

        public Vector3 MaxDoorBounds;
        public Vector3 MinDoorBounds;

        //public void OnOpenGarageDoorChange(bool doorChange)
        //{
        //    opengaragedoor = doorChange;
        //    CmdGarageDoorChange(doorChange);
        //}

        //[Command]
        //public void CmdGarageDoorChange(bool doorChange)
        //{
        //    RpcGarageDoorChange(doorChange);
        //}

        //[ClientRpc]
        //public void RpcGarageDoorChange(bool doorChange)
        //{
        //    opengaragedoor = doorChange;
        //}


        public void GarageDoorOpen()
        {
            Debug.Log("OPENING DOOR");

            //CONVERT TO LOCAL SPACE
            var current_x = GarageDoor.transform.localPosition.x;
            var current_y = GarageDoor.transform.localPosition.y;
            var current_z = GarageDoor.transform.localPosition.z;

            //var convertedPosition = GarageDoor.InverseTransformPoint(new Vector3(current_x, current_y, current_z));

            var target_x = MaxDoorBounds.x;
            var target_y = MaxDoorBounds.y;
            var target_z = MaxDoorBounds.z;

            var slerp_x = Mathf.Lerp(current_x, target_x, Time.smoothDeltaTime * OpenSpeed);
            var slerp_y = Mathf.Lerp(current_y, target_y, Time.smoothDeltaTime * OpenSpeed);
            var slerp_z = Mathf.Lerp(current_z, target_z, Time.smoothDeltaTime * OpenSpeed);
            var slerp_position = new Vector3(slerp_x, slerp_y, slerp_z);

            //GarageDoor.transform.position = slerp_position;
            GarageDoor.transform.localPosition = slerp_position;

            //var door_z_value = GarageDoor.transform.position.z;
            //if(door_z_value < MaxDoorOpen)
            //{
            //    GarageDoor.Translate(Direction.normalized * Time.deltaTime);
            //}

            //var up = new Vector3(0.0f, 0.0f, 0.25f * OpenSpeed);

            //if (playerstater.transform.position == triggerEventPos.transform.position)
            //{
            //    opengaragedoor = true;
            //    GarageDoor.Translate(up * Time.deltaTime);
            //}
        }
        public void GarageDoorClose()
        {
            Debug.Log("CLOSING DOOR");

            //CONVERT TO LOCAL SPACE
            var current_x = GarageDoor.transform.localPosition.x;
            var current_y = GarageDoor.transform.localPosition.y;
            var current_z = GarageDoor.transform.localPosition.z;

            //var convertedPosition = GarageDoor.InverseTransformPoint(new Vector3(current_x, current_y, current_z));

            var target_x = MinDoorBounds.x;
            var target_y = MinDoorBounds.y;
            var target_z = MinDoorBounds.z;

            var slerp_x = Mathf.Lerp(current_x, target_x, Time.smoothDeltaTime * OpenSpeed);
            var slerp_y = Mathf.Lerp(current_y, target_y, Time.smoothDeltaTime * OpenSpeed);
            var slerp_z = Mathf.Lerp(current_z, target_z, Time.smoothDeltaTime * OpenSpeed);
            var slerp_position = new Vector3(slerp_x, slerp_y, slerp_z);

            //GarageDoor.transform.position = slerp_position;
            GarageDoor.transform.localPosition = slerp_position;

            //var door_z_value = GarageDoor.transform.position.z;

            //if (door_z_value > MinDoorOpen)
            //{
            //    GarageDoor.Translate(-Direction * Time.deltaTime);
            //}

            //if(GarageDoor.transform.position == DoorStop.transform.position)
            //{
            //    opengaragedoor = false;
            //}

            //var up = new Vector3(0.0f, 0.0f, -0.25f * OpenSpeed);

            //if (playerstater.transform.position != triggerEventPos.transform.position)
            //{
            //    opengaragedoor = false;
            //    GarageDoor.Translate(up * Time.deltaTime);
            //}
        }

        public void ToggleDoorOpen()
        {
            if (opengaragedoor == true)
            {
                opengaragedoor = false;
            }
            else
            {
                opengaragedoor = true;
            }
        }

        void LateUpdate()
        {
            //var MaxHeight = new Vector3(0, 10, 0);
            //var MinHeight = new Vector3(0, -0.75f, 0);


            //if(GarageDoor.position.y >= MaxHeight.y)
            //{
            //    var maxHeight = GarageDoor.position;
            //    maxHeight.y = MaxHeight.y;
            //    GarageDoor.position = maxHeight;

            //}
            //if (GarageDoor.position.y <= MinHeight.y)
            //{
            //    var minHeight = GarageDoor.position;
            //    minHeight.y = MinHeight.y;
            //    GarageDoor.position = minHeight;
            //}

            //if (!isServer)
            //{
            //    return;
            //}

            if (opengaragedoor == true)
            {
                GarageDoorOpen();
            }
            if (opengaragedoor == false)
            {
                GarageDoorClose();
            }

        }

        void OnTriggerEnter(Collider other)
        {
            //if (!isServer)
            //{
            //    return;
            //}

            //DISPLAY OPEN DOOR UI
            //IF I KEY PRESS, TOGGLE DOOR OPENING


            Debug.Log(other.name);

            if (other.tag == "Player" || other.tag == "Hand")
            {
                Debug.Log("PLAYER ATTEMPT TO OPEN DOOR");

                ToggleDoorOpen();
            }
        }
    }
}
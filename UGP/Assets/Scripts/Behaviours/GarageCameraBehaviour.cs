using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trent
{

    public class GarageCameraBehaviour : MonoBehaviour
    {

        public Transform Player;

        public Transform SecurityCam1;

        public Transform SecurityPost;

        public Transform ToolStationCam1;

        public Transform ComputerCam1;
        public Transform PlayerSeat;
        public Collider ComputerChair;

        public Transform ToolStation1;
        public Transform ToolStationStand;
        public Collider ToolStationCollider;

        public Vector3 Offset;

        // Use this for initialization
        void Start()
        {
            transform.position = SecurityCam1.position;
            transform.rotation = SecurityCam1.rotation;
            var player = FindObjectsOfType<PlayerMovementBehaviour>();

        }

        public void PlayerSitCameraChange()
        {
            var player = FindObjectOfType<PlayerMovementBehaviour>();
            if (player.IsSitting == true)
            {
                transform.position = ComputerCam1.position;
                transform.rotation = ComputerCam1.rotation;

            }

            if (player.IsSitting == false)
            {
                transform.position = SecurityPost.position;
                transform.rotation = SecurityPost.rotation;
            }
        }
        public void PlayerStandCameraChange()
        {
            var player = FindObjectOfType<PlayerMovementBehaviour>();
            if (player.IsStanding == true)
            {
                transform.position = ToolStationCam1.position;
                transform.rotation = ToolStationCam1.rotation;
            }
            if (player.IsStanding == false)
            {
                transform.position = SecurityPost.position;
                transform.rotation = SecurityPost.rotation;
            }


        }
        void FixedUpdate()
        {
            PlayerStandCameraChange();
            PlayerSitCameraChange();
            //var player = FindObjectOfType<PlayerMovementBehaviour>();
            //if(player.IsSitting == true)
            //{
            //    transform.position = ComputerCam1.position;
            //    transform.rotation = ComputerCam1.rotation;

            //}

            //if(player.IsSitting == false)
            //{
            //    transform.position = SecurityPost.position;
            //    transform.rotation = SecurityPost.rotation;
            //}
            //if(player.IsStanding == true)
            //{
            //    transform.position = ToolStationCam1.position;
            //    transform.rotation = ToolStationCam1.rotation;
            //}
            //if(player.IsStanding == false)
            //{
            //    transform.position = SecurityPost.position;
            //    transform.rotation = SecurityPost.rotation;
            //}

            //if (player.transform.position == ComputerChair.transform.position &&  Input.GetKeyDown(KeyCode.E))
            //{
            //    player.position = PlayerSeat.position;
            //    transform.position = ComputerCam1.position;
            //    transform.rotation = ComputerCam1.rotation;
            //    GameObject.FindObjectOfType<PlayerMovementBehaviour>().enabled = false;
            //    if(Input.GetKeyDown(KeyCode.X))
            //    {
            //        GameObject.FindObjectOfType<PlayerMovementBehaviour>().enabled = true;
            //    }
            //}
            //if (player.transform.position == ToolStationCollider.transform.position)
            //{
            //    player.position = ToolStationStand.position;
            //    transform.position = ComputerCam1.position;
            //    transform.rotation = ComputerCam1.rotation;
            //}
        }
        void Update()
        {

        }
    }
}



// Update is called once per frame










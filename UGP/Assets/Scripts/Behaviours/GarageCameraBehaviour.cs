using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{

    public class GarageCameraBehaviour : MonoBehaviour
    {
        #region Camera
        public Transform SecurityCam1;

        public Transform SecurityPost;

        public Transform ToolStationCam1;

        public Transform ComputerCam1;

        public Transform HoverCarCam1;


        #endregion Camera
        #region PlayerInteraction
        private PlayerMovementBehaviour player;

        public Transform Player;
        public Transform PlayerSeat;
        public Transform ToolStationStand;
        public Transform ToolStation1;

        public Collider ToolStationCollider;
        public Collider ComputerChair;
        public Collider HoverCarCollider;
        #endregion PlayerInteraction
        public Vector3 Offset;

        // Use this for initialization
        void Start()
        {
            transform.position = SecurityCam1.position;
            transform.rotation = SecurityCam1.rotation;
            player = FindObjectOfType<PlayerMovementBehaviour>();

        }





        void FixedUpdate()
        {

            player = FindObjectOfType<PlayerMovementBehaviour>();
        
            switch (player.state)
            {
                case PlayerState.idle:
                    {
                        transform.position = SecurityCam1.position;
                        transform.rotation = SecurityCam1.rotation;
                        break;
                    }
                case PlayerState.move:
                    {
                        transform.position = SecurityCam1.position;
                        transform.rotation = SecurityCam1.rotation;
                        SecurityCam1.position = SecurityPost.position;
                        SecurityCam1.rotation = SecurityPost.rotation;
                        break;
                    }
                case PlayerState.sitting:
                    {
                        transform.position = ComputerCam1.position;
                        transform.rotation = ComputerCam1.rotation;

                        break;
                    }
                case PlayerState.standing:
                    {
                        transform.position = ToolStationCam1.position;
                        transform.rotation = ToolStationCam1.rotation;
                        break;
                    }
                case PlayerState.viewing:
                    {
                        transform.position = HoverCarCam1.position;
                        transform.rotation = HoverCarCam1.rotation;
                        break;
                    }
            }

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
        void LateUpdate()
        {
        }
    }
}





// Update is called once per frame










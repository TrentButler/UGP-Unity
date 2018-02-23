using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class InGameCameraBehaviour : NetworkBehaviour
    {
        public Cinemachine.CinemachineVirtualCamera vehicleCam;
        public Cinemachine.CinemachineFreeLook playerCam;

        private PlayerBehaviour localPlayer;

        private void Awake()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
        }

        void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            //DO A SEARCH FOR THE LOCAL PLAYER IF THE VARIABLE 'localPlayer' IS NULL
            if (localPlayer == null)
            {
                var players = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList(); //GATHER EACH PLAYER

                players.ForEach(p =>
                {
                    if (p.localPlayerAuthority)
                    {
                        localPlayer = p;
                    }
                });
            }

            else
            {
                var driving = localPlayer.isDriving;

                if (driving)
                {
                    //ENABLE THE VEHICLE CAMERA, DISABLE THE PLAYER CAMERA
                    vehicleCam.gameObject.SetActive(true);
                    playerCam.gameObject.SetActive(false);

                    //ASSIGN THE CAMERA'S FOLLOW AND LOOK AT TARGETS TO THE TRANSFORM FROM 'localPlayer.vehicle'

                    vehicleCam.Follow = localPlayer.vehicle.transform;
                    vehicleCam.LookAt = localPlayer.vehicle.transform;
                }
                else
                {
                    //ENABLE THE PLAYER CAMERA, DISABLE THE VEHICLE CAMERA
                    playerCam.gameObject.SetActive(true);
                    vehicleCam.gameObject.SetActive(false);

                    //ASSIGN THE CAMERA'S FOLLOW AND LOOK AT TARGETS TO THE TRANSFORM FROM 'playerMovement'
                    playerCam.Follow = localPlayer.playerMovement.transform;
                    playerCam.LookAt = localPlayer.playerMovement.transform;
                }
            }

        }
    }
}
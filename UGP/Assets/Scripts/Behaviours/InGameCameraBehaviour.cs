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


        private void Awake()
        {
            if (!localPlayerAuthority)
            {
                return;
            }
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                return;
            }
        }
        
        void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                return;
            }

            var players = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();

            players.ForEach(player =>
            {
                if (player.localPlayerAuthority)
                {
                    var driving = player.isDriving;

                    if(driving)
                    {
                        vehicleCam.Follow = player.vehicleMovement.transform;
                        vehicleCam.LookAt = player.vehicleMovement.transform;

                        playerCam.gameObject.SetActive(false);
                    }
                    else
                    {
                        playerCam.Follow = player.playerMovement.transform;
                        playerCam.LookAt = player.playerMovement.transform;

                        vehicleCam.gameObject.SetActive(false);
                    }
                }
            });
        }
    }
}
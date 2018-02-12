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

            var players = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();

            PlayerBehaviour player = null;

            players.ForEach(p =>
            {
                if (p.localPlayerAuthority)
                {
                    player = p;
                }
            });

            if(player != null)
            {
                var driving = player.isDriving;

                if (driving)
                {
                    vehicleCam.Follow = player.vehicle.transform;
                    vehicleCam.LookAt = player.vehicle.transform;

                    vehicleCam.gameObject.SetActive(true);
                    playerCam.gameObject.SetActive(false);
                }
                else
                {
                    playerCam.Follow = player.playerMovement.transform;
                    playerCam.LookAt = player.playerMovement.transform;

                    playerCam.gameObject.SetActive(true);
                    vehicleCam.gameObject.SetActive(false);
                }
            }
        }
    }
}
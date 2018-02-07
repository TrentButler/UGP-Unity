using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace UGP
{
    public class OfflineCameraBehaviour : MonoBehaviour
    {
        public Cinemachine.CinemachineVirtualCamera vehicleCam;
        public Cinemachine.CinemachineFreeLook playerCam;

        void FixedUpdate()
        {

            var players = GameObject.FindObjectsOfType<OfflinePlayerBehaviour>().ToList();

            players.ForEach(player =>
            {
                var driving = player.isDriving;

                if (driving)
                {
                    vehicleCam.Follow = player.vehicle.transform;
                    vehicleCam.LookAt = player.vehicle.transform;

                    //vehicleCam.gameObject.SetActive(true);
                    //playerCam.gameObject.SetActive(false);

                    playerCam.enabled = false;
                    vehicleCam.enabled = true;
                }
                else
                {
                    playerCam.Follow = player.playerMovement.transform;
                    playerCam.LookAt = player.playerMovement.transform;

                    //playerCam.gameObject.SetActive(true);
                    //vehicleCam.gameObject.SetActive(false);

                    playerCam.enabled = true;
                    vehicleCam.enabled = false;
                }

            });
        }
    }
}
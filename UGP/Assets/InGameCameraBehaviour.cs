using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class InGameCameraBehaviour : NetworkBehaviour
    {
        public Cinemachine.CinemachineVirtualCamera cam;

        // Use this for initialization
        void Start()
        {
            if (isLocalPlayer)
            {
               
                var players = GameObject.FindObjectsOfType<InGameVehicleMovementBehaviour>().ToList();

                players.ForEach(x =>
                {
                    if (x.localPlayerAuthority)
                    {
                        cam.Follow = x.transform;
                        cam.LookAt = x.transform;
                    }
                });
            }
            if(!isLocalPlayer)
            {
                return;
            }
        }

        // Update is called once per frame
        void Update()
        {
           

            var players = GameObject.FindObjectsOfType<InGameVehicleMovementBehaviour>().ToList();

            players.ForEach(x =>
            {
                if (x.localPlayerAuthority)
                {
                    cam.Follow = x.transform;
                    cam.LookAt = x.transform;
                }
            });
        }
    }
}
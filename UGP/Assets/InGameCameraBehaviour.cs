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
            if (!localPlayerAuthority)
            {
                return;
            }

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

        // Update is called once per frame
        void Update()
        {
            if (!localPlayerAuthority)
            {
                return;
            }

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
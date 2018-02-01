using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        public Player player;
        private Player p;

        [SyncVar] public bool isDriving;
        public VehicleBehaviour vehicle;
        public InGamePlayerMovementBehaviour playerMovement;

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

            if (player != null)
                p = player;

            else
            {
                p = ScriptableObject.CreateInstance<Player>();
                p.Health = 100;
                p.MaxHealth = 100;
            }
        }

        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            if (vehicle == null)
            {
                isDriving = false;
            }
            else
            {
                isDriving = true;
            }

            if (isDriving)
            {
                vehicle.enabled = true;
                playerMovement.enabled = false;


                //NEEDS WORK
                if(Input.GetKeyDown(KeyCode.F))
                {
                    //GET OUT OF VEHICLE
                    vehicle = null;
                }
            }
            else
            {
                vehicle = null;
                playerMovement.enabled = true;
            }
        }
    }
}
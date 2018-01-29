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
        public InGameVehicleMovementBehaviour vehicleMovement;
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

            if (vehicleMovement == null)
            {
                isDriving = false;
            }
            else
            {
                isDriving = true;
            }

            if (isDriving)
            {
                vehicleMovement.enabled = true;
                playerMovement.enabled = false;
            }
            else
            {
                vehicleMovement = null;
                playerMovement.enabled = true;
            }
        }
    }
}
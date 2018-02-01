using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    //NEEDS WORK
    public class VehicleBehaviour : NetworkBehaviour
    {
        public InGameVehicleMovementBehaviour vehicleMovement;
        public VehicleShootBehaviour shootBehaviour;

        [SyncVar] public bool vehicleActive;
        public Transform seat;


        public void SetVehicleActive(bool active)
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
            vehicleActive = active;
        }

        private void Awake()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            vehicleActive = false;
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            vehicleActive = false;
        }

        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            switch (vehicleActive)
            {
                case true:
                    {
                        vehicleMovement.enabled = true;
                        shootBehaviour.enabled = true;
                        break;
                    }

                case false:
                    {
                        vehicleMovement.enabled = false;
                        shootBehaviour.enabled = false;
                        break;
                    }
            }
        }
    }

}
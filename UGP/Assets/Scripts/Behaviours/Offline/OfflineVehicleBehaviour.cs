using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{


    public class OfflineVehicleBehaviour : MonoBehaviour {


    public OfflineVehicleMovementBehaviour vehicleMovement;
    public OfflineShootingBehaviour shootBehaviour;


        public bool vehicleActive;
        public Transform seat;


        public void SetVehicleActive(bool active)
        {

            vehicleActive = active;
        }

        private void Awake()
        {


            vehicleActive = false;
        }

        private void Start()
        {


            vehicleActive = false;
        }

        private void FixedUpdate()
        {


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
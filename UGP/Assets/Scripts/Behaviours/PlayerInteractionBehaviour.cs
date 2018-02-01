using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace UGP
{
    //NEEDS WORK
    public class PlayerInteractionBehaviour : NetworkBehaviour
    {
        public PlayerBehaviour p;

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

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerStay(Collider other)
        {

            //CHECK FOR THE LOCAL PLAYER??????
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            if (other.tag == "Vehicle")
            {
                var v = other.GetComponentInParent<VehicleBehaviour>();

                var vActive = v.vehicleActive;


                if (!vActive) //CHECK IF THE VEHICLE IS ALREADY IN USE
                {
                    Debug.Log("PRESS F TO ENTER VEHICLE");
                    //F KEY PRESS TO ENTER THE VEHICLE
                    if (Input.GetKeyDown(KeyCode.F))
                    {

                        p.vehicle = v;
                        v.SetVehicleActive(true);
                        //GET IN THE VEHICLE
                    }
                }
            }
        }

    }
}
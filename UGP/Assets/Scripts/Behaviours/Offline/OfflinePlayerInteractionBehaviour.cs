using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{

    public class OfflinePlayerInteractionBehaviour : MonoBehaviour
    {
        public OfflinePlayerBehaviour p;

        private void Awake()
        {
        }

        private void Start()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerStay(Collider other)
        {

            //CHECK FOR THE LOCAL PLAYER??????
           

            if (other.tag == "Vehicle")
            {
                var v = other.GetComponentInParent<OfflineVehicleBehaviour>();

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
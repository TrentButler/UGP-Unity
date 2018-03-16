using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class VehicleSeatBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                var v = GetComponentInParent<VehicleBehaviour>();
                var vActive = v.vehicleActive;
                var vehicleIdentity = v.GetComponent<NetworkIdentity>();

                var p = other.GetComponent<PlayerBehaviour>();

                if (vActive)
                {
                    return;
                }

                if (!vActive && p.vehicle == null) //CHECK IF THE VEHICLE IS ALREADY IN USE
                {
                    //GET IN THE VEHICLE
                    p.vehicle = v;
                    p.interaction.CmdEnterVehicle(vehicleIdentity);
                    v.SetVehicleActive(true);
                    //v.ani.SetBool("PlayerInSeat", true);
                    v.ani.SetTrigger("CloseDoor");
                }
            }
        }
    }
}
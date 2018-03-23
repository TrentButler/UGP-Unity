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
        [SyncVar] public bool isHolding = false;

        [Command] public void CmdSetHolding(bool holding)
        {
            isHolding = holding;
        }

        [Command] private void CmdEnterVehicle(NetworkIdentity identity)
        {
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = identity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.AssignClientAuthority(localPlayerConn);
            //localPlayerNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (other.tag == "Vehicle")
            {
                var v = other.GetComponentInParent<VehicleBehaviour>();
                var vActive = v.vehicleActive;
                var vehicleIdentity = v.GetComponent<NetworkIdentity>();

                if (!vActive && p.vehicle == null) //CHECK IF THE VEHICLE IS ALREADY IN USE
                {
                    Debug.Log("PRESS F TO ENTER VEHICLE");

                    //F KEY PRESS TO ENTER THE VEHICLE
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        //GET IN THE VEHICLE
                        p.vehicle = v;
                        v.SetVehicleActive(p.name, true);
                        v.playerInSeat = true;
                        CmdEnterVehicle(vehicleIdentity);
                    }
                }
            }
        }
    }
}
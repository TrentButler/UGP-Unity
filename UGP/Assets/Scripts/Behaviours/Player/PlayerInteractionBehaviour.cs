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
            if (!isLocalPlayer)
            {
                return;
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                return;
            }
        }

        public void NewEvent()
        {

        }

        [Command] public void CmdEnterVehicle(NetworkIdentity identity)
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

                if(vActive)
                {
                    return;
                }

                if (!vActive && p.vehicle == null) //CHECK IF THE VEHICLE IS ALREADY IN USE
                {
                    Debug.Log("PRESS F TO OPEN VEHICLE DOOR");

                    //F KEY PRESS TO ENTER THE VEHICLE
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        //OPEN VEHICLE DOOR
                        v.ani.SetTrigger("OpenDoor");
                    }
                }
            }
        }
    }
}
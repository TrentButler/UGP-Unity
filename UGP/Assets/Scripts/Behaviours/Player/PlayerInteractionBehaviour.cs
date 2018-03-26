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
        public Transform HoldingItemPosition;
        [SyncVar] public bool isHolding = false;

        public GameObject ItemModel;

        #region COMMAND_FUNCTIONS
        [Command] public void CmdSetHolding(bool holding)
        {
            isHolding = holding;
        }
        [Command] public void CmdSetItemBeingHeld(bool holding, NetworkIdentity item)
        {
            item.GetComponent<ItemBehaviour>().isBeingHeld = holding;
        }
        [Command] private void CmdEnterVehicle(NetworkIdentity identity)
        {
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = identity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.AssignClientAuthority(localPlayerConn);
        }
        [Command] public void CmdSetVehicleActive(bool active, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().vehicleActive = active;
        }
        [Command] public void CmdSetPlayerInSeat(bool inSeat, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().playerInSeat = inSeat;
        }
        [Command] public void CmdSetVehicleColor(Color color, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().vColor = color;
        }
        [Command] public void CmdAssignItemAuthority(NetworkIdentity itemIdentity)
        {
            //Debug.Log(player.gameObject.name + " ASSIGN AUTHORITY TO: " + gameObject.name);
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var itemNetworkIdentity = itemIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            itemNetworkIdentity.AssignClientAuthority(localPlayerConn);
            //localPlayerNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        [Command] public void CmdRemoveItemAuthority(NetworkIdentity itemIdentity)
        {
            //Debug.Log(player.gameObject.name + " REMOVE AUTHORITY FROM: " + gameObject.name);

            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var itemNetworkIdentity = itemIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            itemNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        #endregion
        
        //NEEDS WORK
        public void UseItemOnVehicle(string itemType, ItemBehaviour item, VehicleBehaviour vehicle)
        {
            switch (itemType)
            {
                case "UGP.RepairKit":
                    {
                        var repair_item = item._I as RepairKit;
                        vehicle.CmdTakeHealth(repair_item.RepairFactor);

                        //CmdRemoveItemAuthority(itemIdentity);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(item.gameObject); //DESTROY THE GAMEOBJECT WHEN USED
                        break;
                    }

                case "UGP.Fuel":
                    {
                        var refuel_item = item._I as Fuel;
                        vehicle.CmdRefuel(refuel_item.RefuelFactor);

                        //CmdRemoveItemAuthority(item);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(item.gameObject); //DESTROY THE GAMEOBJECT WHEN USED
                        break;
                    }

                case "UGP.AmmoBox":
                    {
                        var ammo_item = item._I as AmmoBox;
                        vehicle.CmdTakeAmmunition(ammo_item.Assault, ammo_item.Shotgun, ammo_item.Sniper, ammo_item.Rocket);

                        //CmdRemoveItemAuthority(itemIdentity);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(item.gameObject);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
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

                if(!isHolding) //DO NOT ENTER VEHICLE WHILE HOLDING AN ITEM
                {
                    if (!vActive && p.vehicle == null) //CHECK IF THE VEHICLE IS ALREADY IN USE
                    {
                        Debug.Log("PRESS F TO ENTER VEHICLE");

                        //F KEY PRESS TO ENTER THE VEHICLE
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            //GET IN THE VEHICLE
                            p.CmdSetDriving(true);
                            p.SetVehicle(v);
                            CmdSetVehicleActive(true, vehicleIdentity);
                            CmdSetPlayerInSeat(true, vehicleIdentity);
                            CmdSetVehicleColor(p.vehicleColor, vehicleIdentity);
                            CmdEnterVehicle(vehicleIdentity);
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if(isHolding)
            {
                ItemModel.SetActive(true);
            }
            else
            {
                ItemModel.SetActive(false);
            }
        }
    }
}
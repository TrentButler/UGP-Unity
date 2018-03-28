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
        public Animator Ani;

        #region COMMAND_FUNCTIONS
        [Command] public void CmdSetHolding(bool holding)
        {
            isHolding = holding;
        }
        [Command] public void CmdSetItemBeingHeld(bool holding, NetworkIdentity item)
        {
            item.GetComponent<ItemBehaviour>().isBeingHeld = holding;
        }
        [Command] public void CmdAssignVehicleAuthority(NetworkIdentity vehicleIdentity)
        {
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = vehicleIdentity;

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

        public void PickUpItem()
        {
            Ani.SetTrigger("PickUpItem");
        }

        public void DropItem()
        {
            Ani.SetTrigger("DropItem");
        }

        //NEEDS WORK
        public void UseItemOnVehicle(string itemType, NetworkIdentity itemIdentity, NetworkIdentity vehicleIdentity)
        {
            switch (itemType)
            {
                case "UGP.RepairKit":
                    {
                        //CmdAssignVehicleAuthority(vehicleIdentity);

                        var repair_item = itemIdentity.GetComponent<ItemBehaviour>()._I as RepairKit;
                        vehicleIdentity.GetComponent<VehicleBehaviour>().CmdTakeHealth(repair_item.RepairFactor);
                        Debug.Log("VEHICLE TAKE REPAIR");

                        //p.CmdRemoveVehicleAuthority(vehicleIdentity);
                        

                        //CmdRemoveItemAuthority(itemIdentity);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(itemIdentity.gameObject); //DESTROY THE GAMEOBJECT WHEN USED
                        break;
                    }

                case "UGP.Fuel":
                    {
                        //CmdAssignVehicleAuthority(vehicleIdentity);

                        var refuel_item = itemIdentity.GetComponent<ItemBehaviour>()._I as Fuel;
                        Debug.Log("VEHICLE TAKE FUEL");
                        vehicleIdentity.GetComponent<VehicleBehaviour>().CmdRefuel(refuel_item.RefuelFactor);
                        //var current_fuel = vehicleIdentity.GetComponent<VehicleBehaviour>().vehicleFuel;

                        //p.CmdRemoveVehicleAuthority(vehicleIdentity);

                        //CmdRemoveItemAuthority(item);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(itemIdentity.gameObject); //DESTROY THE GAMEOBJECT WHEN USED
                        break;
                    }

                case "UGP.AmmoBox":
                    {
                        //CmdAssignVehicleAuthority(vehicleIdentity);

                        var ammo_item = itemIdentity.GetComponent<ItemBehaviour>()._I as AmmoBox;
                        vehicleIdentity.GetComponent<VehicleBehaviour>().CmdTakeAmmunition(ammo_item.Assault, ammo_item.Shotgun, ammo_item.Sniper, ammo_item.Rocket);
                        Debug.Log("VEHICLE TAKE AMMO");

                        //p.CmdRemoveVehicleAuthority(vehicleIdentity);

                        //CmdRemoveItemAuthority(itemIdentity);
                        //Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + itemIdentity.hasAuthority.ToString());
                        CmdSetHolding(false);
                        //CmdSetItemBeingHeld(false, itemIdentity);

                        Destroy(itemIdentity.gameObject);
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
                            CmdAssignVehicleAuthority(vehicleIdentity);
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
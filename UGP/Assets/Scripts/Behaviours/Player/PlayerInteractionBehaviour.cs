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
        [SyncVar(hook = "OnisHoldingChange")] public bool isHolding = false;
        [SyncVar(hook = "OnItemChange")] public string current_item = "";

        private void OnItemChange(string itemChange)
        {
            current_item = itemChange;
        }
        private void OnisHoldingChange(bool holdingChange)
        {
            isHolding = holdingChange;
            Ani.SetBool("HoldingItem", isHolding);
        }

        private GameObject CurrentItemModel;
        public GameObject EmptyItem;
        public GameObject fuelModel;
        public GameObject ammoModel;
        public GameObject repairKitModel;
        public Animator Ani;
        public NetworkAnimator NetworkAni;

        public ItemBehaviour item;

        #region COMMAND_FUNCTIONS
        [Command]
        public void CmdSetHolding(bool holding, string item_type)
        {
            isHolding = holding;
            current_item = item_type;
            //switch (current_item)
            //{
            //    case "":
            //        {
            //            CurrentItemModel = EmptyItem;
            //            //CurrentItemModel.SetActive(false);
            //            break;
            //        }

            //    case "UGP.AmmoBox":
            //        {
            //            CurrentItemModel = ammoModel;
            //            break;
            //        }

            //    case "UGP.Fuel":
            //        {
            //            CurrentItemModel = fuelModel;
            //            break;
            //        }

            //    case "UGP.RepairKit":
            //        {
            //            CurrentItemModel = repairKitModel;
            //            break;
            //        }

            //    default:
            //        {
            //            break;
            //        }
            //}
            //RpcSetCurrentItem(current_item);
        }
        [Command]
        public void CmdSetItemBeingHeld(bool holding, NetworkIdentity item)
        {
            item.GetComponent<ItemBehaviour>().isBeingHeld = holding;
        }
        [Command]
        public void CmdAssignVehicleAuthority(NetworkIdentity vehicleIdentity)
        {
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = vehicleIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.AssignClientAuthority(localPlayerConn);
        }
        [Command]
        public void CmdSetVehicleActive(bool active, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().vehicleActive = active;
        }
        [Command]
        public void CmdSetPlayerInSeat(bool inSeat, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().playerInSeat = inSeat;
        }
        [Command]
        public void CmdSetVehicleColor(Color color, NetworkIdentity vehicleIdentity)
        {
            vehicleIdentity.GetComponent<VehicleBehaviour>().vColor = color;
        }
        [Command]
        public void CmdAssignItemAuthority(NetworkIdentity itemIdentity)
        {
            //Debug.Log(player.gameObject.name + " ASSIGN AUTHORITY TO: " + gameObject.name);
            var localPlayerNetworkIdentity = p.GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var itemNetworkIdentity = itemIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            itemNetworkIdentity.AssignClientAuthority(localPlayerConn);
            //localPlayerNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        [Command]
        public void CmdRemoveItemAuthority(NetworkIdentity itemIdentity)
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
            if(!isHolding)
            {
                NetworkAni.SetTrigger("PickUpItem");
            }
        }

        public void DropItem()
        {
            if(isHolding)
            {
                NetworkAni.SetTrigger("DropItem");
            }
        }

        //NEEDS WORK
        public void UseItemOnVehicle(string itemType, NetworkIdentity itemIdentity, NetworkIdentity vehicleIdentity)
        {
            switch (itemType)
            {
                case "UGP.AmmoBox":
                    {
                        var ammo_item = itemIdentity.GetComponent<ItemBehaviour>()._I as AmmoBox;

                        var vehicle_destroyed = vehicleIdentity.GetComponent<VehicleBehaviour>().isDestroyed;
                        if(!vehicle_destroyed)
                        {
                            vehicleIdentity.GetComponent<VehicleBehaviour>().CmdTakeAmmunition(ammo_item.Assault, ammo_item.Shotgun, ammo_item.Sniper, ammo_item.Rocket);
                            Debug.Log("VEHICLE TAKE AMMO");
                        }

                        CmdSetHolding(false, "");
                        break;
                    }

                case "UGP.Fuel":
                    {
                        var refuel_item = itemIdentity.GetComponent<ItemBehaviour>()._I as Fuel;

                        var vehicle_destroyed = vehicleIdentity.GetComponent<VehicleBehaviour>().isDestroyed;
                        if(!vehicle_destroyed)
                        {
                            vehicleIdentity.GetComponent<VehicleBehaviour>().CmdRefuel(refuel_item.RefuelFactor);
                            Debug.Log("VEHICLE TAKE FUEL");
                        }

                        CmdSetHolding(false, "");
                        break;
                    }

                case "UGP.RepairKit":
                    {
                        var repair_item = itemIdentity.GetComponent<ItemBehaviour>()._I as RepairKit;

                        var vehicle_destroyed = vehicleIdentity.GetComponent<VehicleBehaviour>().isDestroyed;
                        if(!vehicle_destroyed)
                        {
                            vehicleIdentity.GetComponent<VehicleBehaviour>().CmdTakeHealth(repair_item.RepairFactor);
                            Debug.Log("VEHICLE TAKE REPAIR");
                        }

                        CmdSetHolding(false, "");
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

                if (!isHolding && !v.isDestroyed) //DO NOT ENTER VEHICLE WHILE HOLDING AN ITEM, OR IF THE VEHICLE IS DESTROYED
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

                            //var player_network_identity = GetComponent<NetworkIdentity>();
                            //v.CmdSetPlayer(player_network_identity);
                            v.seatedPlayer = p;
                        }
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!isLocalPlayer)
            {
                return;
            }

            var contact_points = collision.contacts.ToList();
            contact_points.ForEach(contact =>
            {
                if(contact.thisCollider.CompareTag("Hand"))
                {
                    var item_behaviour = contact.otherCollider.GetComponent<ItemBehaviour>();

                    if(item_behaviour != null && !item_behaviour.isBeingHeld)
                    {
                        item_behaviour.PickUp(this);
                    }
                }
            });
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                return;
            }
        }
        
        private void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                return;
            }

            if (isHolding)
            {
                //REFACTOR TO INPUTCONTROLLER.BUTTONYOUNEED
                //INPUTCONTROLLER.HOLDOUTITEM
                if (Input.GetMouseButton(1))
                {
                    Ani.SetBool("UsingItem", true);
                }
                else
                {
                    Ani.SetBool("UsingItem", false);
                }
            }
        }

        private void LateUpdate()
        {
            if (isHolding)
            {
                //CurrentItemModel.SetActive(true);
                //var old_item = CurrentItemModel;

                switch (current_item)
                {
                    case "":
                        {
                            EmptyItem.SetActive(true);

                            ammoModel.SetActive(false);
                            fuelModel.SetActive(false);
                            repairKitModel.SetActive(false);
                            break;
                        }

                    case "UGP.AmmoBox":
                        {
                            ammoModel.SetActive(true);

                            EmptyItem.SetActive(false);
                            fuelModel.SetActive(false);
                            repairKitModel.SetActive(false);
                            break;
                        }

                    case "UGP.Fuel":
                        {
                            fuelModel.SetActive(true);

                            ammoModel.SetActive(false);
                            EmptyItem.SetActive(false);
                            repairKitModel.SetActive(false);
                            break;
                        }

                    case "UGP.RepairKit":
                        {
                            repairKitModel.SetActive(true);

                            fuelModel.SetActive(false);
                            ammoModel.SetActive(false);
                            EmptyItem.SetActive(false);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                EmptyItem.SetActive(true);

                ammoModel.SetActive(false);
                fuelModel.SetActive(false);
                repairKitModel.SetActive(false);
            }
        }
    }
}
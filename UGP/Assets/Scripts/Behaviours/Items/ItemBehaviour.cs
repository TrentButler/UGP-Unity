using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace UGP
{
    //NEEDS WORK
    //DISABLE THE ITEM GAMEOBJECT'S MODEL ACROSS ALL CLIENTS IF IT IS BEING HELD
    public class ItemBehaviour : NetworkBehaviour
    {
        public Text ItemName;
        public GameObject ItemCanvas;
        public Item ItemConfig;
        [HideInInspector]
        public Item _I;

        public GameObject model;
        private Rigidbody rb;
        private PlayerInteractionBehaviour player;
        private NetworkIdentity item_network_identity;
        [SyncVar(hook = "OnisBeingHeldChange")] public bool isBeingHeld = false;

        [Command] public void CmdSetHolding(bool holding)
        {
            isBeingHeld = holding;
        }

        public void OnisBeingHeldChange(bool beingHeldChange)
        {
            isBeingHeld = beingHeldChange;
        }

        public void PickUp(PlayerInteractionBehaviour interaction)
        {   
            player = interaction;
            player.item = this;
            var string_type = _I.GetType().ToString();
            player.CmdSetHolding(true, string_type);
            player.CmdSetItemBeingHeld(true, item_network_identity);

            player.CmdAssignItemAuthority(item_network_identity);
            Debug.Log("ATTEMPT TO ASSIGN AUTHORITY: EXPECTED = True, RESULT = " + item_network_identity.hasAuthority.ToString());

            rb.MovePosition(player.HoldingItemPosition.position);
            
            var colliders = GetComponents<Collider>().ToList();
            colliders.ForEach(collider =>
            {
                if (!collider.isTrigger)
                {
                    collider.enabled = false;
                }
                //collider.enabled = false;
            });
            rb.useGravity = false;
        }

        public void Drop()
        {
            player.CmdRemoveItemAuthority(item_network_identity);
            Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + item_network_identity.hasAuthority.ToString());
            
            player.CmdSetHolding(false, "");
            player.CmdSetItemBeingHeld(false, item_network_identity);
            player.item = null;

            var colliders = GetComponents<Collider>().ToList();
            colliders.ForEach(collider =>
            {
                collider.enabled = true;
            });

            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.MovePosition(player.HoldingItemPosition.position);

            player = null; //REMOVE REFRENCE TO PLAYER
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                var player_identity = other.GetComponentInParent<NetworkIdentity>();

                if(player_identity.isLocalPlayer && !isBeingHeld)
                {
                    Debug.Log("Press F To Pick Up " + _I.name);
                    ItemName.text = ItemConfig.name;
                    ItemCanvas.SetActive(true);
                }

                //var player_holding_transform = other.transform.Find("ItemHoldPosition");

                var player_interaction = other.GetComponentInParent<PlayerInteractionBehaviour>();

                if (player_interaction != null)
                {
                    player = player_interaction;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!player_interaction.isHolding && !isBeingHeld && !isServer)
                    {
                        player.PickUpItem();
                        PickUp(player);
                    }
                }
            }

            if (other.tag == "Vehicle")
            {
                Debug.Log("COLLISION WITH: " + other.gameObject.name);

                var vehicle_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                var vehicle_identity = other.GetComponentInParent<NetworkIdentity>();

                if (isBeingHeld && player.isLocalPlayer)
                {
                    //TYPE CAST THE ITEM CONFIG AS A REPAIR KIT
                    //INVOKE THE FUNCTION 'TakeHealth' ON THE VEHICLE
                    var string_type = _I.GetType().ToString();
                    player.CmdAssignVehicleAuthority(vehicle_identity);

                    if (vehicle_identity.hasAuthority)
                    {
                        player.DropItem();
                        player.UseItemOnVehicle(string_type, item_network_identity, vehicle_identity);
                        player.p.CmdRemoveVehicleAuthority(vehicle_identity);
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            ItemCanvas.SetActive(false);
            player = null;
        }

        void Start()
        {
            if (_I == null)
            {
                if (ItemConfig == null)
                {
                    ItemConfig = Resources.Load("Assets\\Resources\\ScriptableObjects\\Items\\DefaultItem") as Item;
                }
                else
                {
                    _I = Instantiate(ItemConfig);
                }
            }

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            item_network_identity = GetComponent<NetworkIdentity>();
            if(item_network_identity == null)
            {
                item_network_identity = gameObject.AddComponent<NetworkIdentity>();
            }
        }

        void FixedUpdate()
        {
            //if(isServer)
            //{
            //    UpdateItem();
            //}

            if (isBeingHeld && !isServer && hasAuthority)
            {
                ItemCanvas.SetActive(false);
                //var item_pos = _parent.position;
                //rb.MovePosition(item_pos);
                //rb.MovePosition(player.HoldingItemPosition.position);
                //rb.position = _parent.position;
                //model.SetActive(false);
                //rb.isKinematic = true;
                rb.MovePosition(player.HoldingItemPosition.position);
                rb.velocity = Vector3.zero;
                rb.MoveRotation(player.HoldingItemPosition.rotation);
            }

            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                Debug.Log("RIGHT ALT KEY PRESS");
                if (isBeingHeld && !isServer && hasAuthority)
                {
                    //var player_interaction = other.GetComponent<PlayerInteractionBehaviour>();
                    player.DropItem();
                    Drop();
                }
            }
        }

        private void LateUpdate()
        {
            if(isBeingHeld)
            {
                model.SetActive(false);
            }
            else
            {
                model.SetActive(true);
            }
        }
    }
}

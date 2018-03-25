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
        [SyncVar(hook = "OnValueChange")] public bool isBeingHeld = false;

        public void OnValueChange(bool valueChange)
        {
            isBeingHeld = valueChange;
        }

        [Command] public void CmdOnPickUp()
        {
            isBeingHeld = true;
            model.SetActive(false);
        }
        [Command] public void CmdOnDrop()
        {
            isBeingHeld = false;
            model.SetActive(true);
        }

        public void UpdateItem()
        {
            RpcSetItemActive(!isBeingHeld);
        }

        [ClientRpc] public void RpcSetItemActive(bool active)
        {
            model.SetActive(active);
        }

        public void PickUp(PlayerInteractionBehaviour interaction)
        {
            player = interaction;
            player.CmdSetHolding(true);

            player.CmdAssignItemAuthority(item_network_identity);
            Debug.Log("ATTEMPT TO ASSIGN AUTHORITY: EXPECTED = True, RESULT = " + item_network_identity.hasAuthority.ToString());

            //PARENT THE 'ITEM' TO THE PLAYER
            //TURN GRAVITY OFF FOR THE ITEM
            //PlayerHand = parent;
            //transform.SetParent(PlayerHand);
            //interaction.Child.target = transform;
            //var item_pos = Vector3.zero;
            //var item_pos = _parent.TransformPoint(Vector3.zero);
            //rb.MovePosition(item_pos);
            //rb.MovePosition(PlayerHand.position);
            //rb.position = _parent.position;

            model.SetActive(false);
            //CmdDisableItemModel();
            rb.MovePosition(player.HoldingItemPosition.position);

            //rb.constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<BoxCollider>().enabled = false;
            rb.useGravity = false;
            isBeingHeld = true;
            //CmdOnPickUp();
        }

        public void Drop()
        {
            player.CmdRemoveItemAuthority(item_network_identity);
            Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + item_network_identity.hasAuthority.ToString());
            
            player.CmdSetHolding(false);

            isBeingHeld = false;
            CmdOnDrop();
            //transform.parent = null;


            //transform.parent = null;
            //var item_pos = _parent.position;
            //var item_pos = _parent.position;
            //rb.MovePosition(item_pos);
            model.SetActive(true);
            //CmdEnableItemModel();
            GetComponent<BoxCollider>().enabled = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.MovePosition(player.HoldingItemPosition.position);

            player = null; //REMOVE REFRENCE TO PLAYER
        }

        //public void UseItem()
        //{

        //}

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                var player_identity = other.GetComponent<NetworkIdentity>();

                if(player_identity.isLocalPlayer && !isBeingHeld)
                {
                    Debug.Log("Press F To Pick Up " + _I.name);
                    ItemName.text = ItemConfig.name;
                    ItemCanvas.SetActive(true);
                }

                //var player_holding_transform = other.transform.Find("ItemHoldPosition");

                var player_interaction = other.GetComponent<PlayerInteractionBehaviour>();

                if (player_interaction != null)
                {
                    player = player_interaction;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!player_interaction.isHolding && !isBeingHeld && !isServer)
                    {
                        PickUp(player);
                    }
                }
            }

            if (other.tag == "Vehicle")
            {
                if (isBeingHeld)
                {
                    //TYPE CAST THE ITEM CONFIG AS A REPAIR KIT
                    //INVOKE THE FUNCTION 'TakeHealth' ON THE VEHICLE
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            ItemCanvas.SetActive(false);
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
            if(isServer)
            {
                UpdateItem();
            }

            if (isBeingHeld && !isServer && hasAuthority)
            {
                ItemCanvas.SetActive(false);
                //var item_pos = _parent.position;
                //rb.MovePosition(item_pos);
                //rb.MovePosition(player.HoldingItemPosition.position);
                //rb.position = _parent.position;
                model.SetActive(false);
                //rb.isKinematic = true;
                rb.MovePosition(player.HoldingItemPosition.position);
                rb.velocity = Vector3.zero;
                rb.MoveRotation(player.HoldingItemPosition.rotation);
            }

            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                if (isBeingHeld && !isServer && hasAuthority)
                {
                    //var player_interaction = other.GetComponent<PlayerInteractionBehaviour>();
                    Drop();
                }
            }
        }
    }
}

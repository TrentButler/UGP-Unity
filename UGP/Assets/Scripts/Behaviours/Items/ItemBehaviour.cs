using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace UGP
{
    //NEEDS WORK
    //IMPLEMENT PLAYER THROW ITEM TO VEHICLE MECHANIC
    public class ItemBehaviour : NetworkBehaviour
    {
        public GameObject ItemCanvas;
        public Item ItemConfig;
        [HideInInspector]
        public Item _I;

        public GameObject model;
        private Rigidbody rb;
        private PlayerInteractionBehaviour player;
        private NetworkIdentity item_network_identity;
        [SyncVar(hook = "OnisBeingHeldChange")] public bool isBeingHeld = false;

        [Range(0, 999.0f)] public float DropItemPower = 1.5f;
        [Range(0, 999.0f)] public float DropItemOffset = 1.5f;
        private Vector3 nonTriggerColliderSize;

        [Command] public void CmdSetHolding(bool holding)
        {
            isBeingHeld = holding;
        }

        public void OnisBeingHeldChange(bool beingHeldChange)
        {
            isBeingHeld = beingHeldChange;
        }

        //FUNCTION TO CHECK IF ITEM IS GROUNDED
        private bool CheckItemGrounded()
        {
            return Physics.BoxCast(rb.centerOfMass, nonTriggerColliderSize / 2, -Vector3.up);
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
            rb.velocity = Vector3.zero;
        }

        public void Drop()
        {
            player.CmdRemoveItemAuthority(item_network_identity);
            Debug.Log("ATTEMPT TO REMOVE AUTHORITY: EXPECTED = False, RESULT = " + item_network_identity.hasAuthority.ToString());
            
            player.CmdSetHolding(false, "");
            player.CmdSetItemBeingHeld(false, item_network_identity);
            isBeingHeld = false;
            player.item = null;

            var colliders = GetComponents<Collider>().ToList();
            var trigger_size = Vector3.zero;
            colliders.ForEach(collider =>
            {
                if (!collider.isTrigger)
                {
                    collider.enabled = true;
                }
                else
                {
                    collider.enabled = true;
                    trigger_size = collider.bounds.size;
                    collider.enabled = false;
                }
            });

            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;

            var drop_position = Vector3.zero;

            var isUsingItem = player.Ani.GetBool("UsingItem");
            if(isUsingItem)
            {
                //DROP ITEM INFRONT OF PLAYER
                var front_drop_position = trigger_size;
                front_drop_position.x = 0;
                front_drop_position.y = 0;
                front_drop_position.z = front_drop_position.z * DropItemOffset;

                drop_position = player.transform.TransformPoint(front_drop_position);
            }
            else
            {
                //DROP ITEM ON SIDE OF PLAYER
                var side_drop_position = trigger_size;
                side_drop_position.x = side_drop_position.x * DropItemOffset;
                side_drop_position.y = 0;
                side_drop_position.z = 0;

                drop_position = player.transform.TransformPoint(side_drop_position);
            }

            drop_position.y = transform.position.y;

            //var drop_position = player.HoldingItemPosition.position;
            //trigger_size.y = 0;
            //trigger_size.x = 0;
            //trigger_size.z = -trigger_size.z;

            //drop_position += trigger_size * DropItemOffset;

            RaycastHit hit;
            if(Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                //PUT THE ITEM ON THE GROUND
                drop_position.y -= hit.distance;
            }
            
            rb.transform.position = (drop_position);
            //rb.MovePosition(drop_position);
            rb.velocity = -Vector3.up * DropItemPower;

            //rb.MovePosition(drop_position);

            player = null; //REMOVE REFRENCE TO PLAYER
        }

        private void ItemUILookat(Transform target)
        {
            ItemCanvas.transform.LookAt(target);
        }

        private void OnTriggerStay(Collider other)
        {
            if(isServer)
            {
                return;
            }

            if (other.tag == "Player")
            {
                var player_identity = other.GetComponentInParent<NetworkIdentity>();
                if (player_identity.isLocalPlayer && !isBeingHeld)
                {
                    ItemCanvas.SetActive(true);
                    var player_behaviour = player_identity.GetComponent<PlayerBehaviour>();
                    //var camTransform = player_behaviour.VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(1).transform;
                    var cinemachine_rig = player_behaviour.VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().LiveChildOrSelf;
                    var camTransform = cinemachine_rig.VirtualCameraGameObject.transform;
                    ItemUILookat(camTransform);
                }
                
                var player_interaction = other.GetComponentInParent<PlayerInteractionBehaviour>();
                if (player_interaction != null)
                {
                    player = player_interaction;
                }
            }

            if (other.CompareTag("Hand"))
            {
                var player_interaction = other.GetComponentInParent<PlayerInteractionBehaviour>();
                if (player_interaction != null)
                {
                    if (!player_interaction.isHolding && !isBeingHeld)
                    {
                        PickUp(player_interaction);
                    }
                }
            }

            if (other.tag == "Vehicle")
            {
                Debug.Log("COLLISION WITH: " + other.gameObject.name);

                var vehicle_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                var vehicle_identity = other.GetComponentInParent<NetworkIdentity>();

                if (isBeingHeld && player.isLocalPlayer && !vehicle_behaviour.isDestroyed)
                {
                    var string_type = _I.GetType().ToString(); //GET THE TYPE OF ITEM
                    player.CmdAssignVehicleAuthority(vehicle_identity); //ASSIGN THE VEHICLE 'AUTHORITY'

                    if (vehicle_identity.hasAuthority)
                    {
                        player.UseItemOnVehicle(string_type, item_network_identity, vehicle_identity); //USE THE ITEM ON THE VEHICLE
                        player.playerBrain.CmdRemoveVehicleAuthority(vehicle_identity); //REMOVE THE AUTHORITY FROM THE VEHICLE
                        player._DropItem(); //REMOVE THE ITEM FROM THE PLAYER

                        var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                        net_companion.Server_Destroy(gameObject);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ItemCanvas.SetActive(false);

            if(!isBeingHeld)
            {
                player = null;
            }
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

            var colliders = GetComponents<Collider>().ToList();
            colliders.ForEach(col =>
            {
                if (!col.isTrigger)
                {
                    nonTriggerColliderSize = col.bounds.extents;
                }
            });
        }

        void FixedUpdate()
        {
            if (isBeingHeld && !isServer && hasAuthority)
            {
                ItemCanvas.SetActive(false);

                //CLEAN THIS UP,
                //CONSIDER THE 'THROW ITEM TO VEHICLE MECHANIC'
                //MIGHT NEED TO DISABLE THIS/RE-ENABLE THE ITEM MODEL WHEN THE PLAYER THROWS IT, 
                //VEHICLE CANNOT DRIVE OVER AN ITEM AND USE IT
                rb.MovePosition(player.HoldingItemPosition.position);
                rb.velocity = Vector3.zero;
                rb.MoveRotation(player.HoldingItemPosition.rotation);
            }
            else
            {
                if(CheckItemGrounded())
                {
                    var colliders = GetComponents<Collider>().ToList();
                    colliders.ForEach(collider =>
                    {
                        collider.enabled = true;
                    });

                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
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
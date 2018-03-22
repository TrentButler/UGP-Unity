using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UGP
{

    public class ItemBehaviour : MonoBehaviour
    {
        public Text ItemName;
        public GameObject ItemCanvas;
        public Item ItemConfig;
        [HideInInspector]
        public Item _I;

        private Rigidbody rb;
        public Transform _parent;
        private OfflinePlayerInteractionBehaviour player;
        //private PlayerInteractionBehaviour player;
        public bool isBeingHeld = false;

        public void PickUp(Transform parent, OfflinePlayerInteractionBehaviour interaction)
        {
            //PARENT THE 'ITEM' TO THE PLAYER
            //TURN GRAVITY OFF FOR THE ITEM
            _parent = parent;
            transform.SetParent(parent);
            rb.MovePosition(Vector3.zero);
            rb.constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<BoxCollider>().enabled = false;
            rb.useGravity = false;
            isBeingHeld = true;

            player = interaction;
            player.SetHolding(true);
        }

        public void Drop(Transform parent)
        {
            //UN-PARENT THE 'ITEM' FROM THE PLAYER
            //TURN GRAVITY BACK ON FOR THE ITEM
            transform.parent = null;
            rb.MovePosition(parent.position);
            _parent = null;
            rb.constraints = RigidbodyConstraints.None;
            GetComponent<BoxCollider>().enabled = true;
            rb.useGravity = true;
            isBeingHeld = false;
            player.SetHolding(false);
            player = null;
        }

        //public void UseItem()
        //{

        //}

        private void OnTriggerStay(Collider other)
        {
            //CHECK FOR THE LOCAL PLAYER??????
            if (other.tag == "Player")
            {
                Debug.Log("Press F To Pick Up " + _I.name);
                ItemName.text = ItemConfig.name;
                ItemCanvas.SetActive(true);

                var player_holding_transform = other.transform.Find("ItemHoldPosition");

                var player_interaction = other.GetComponent<OfflinePlayerInteractionBehaviour>();
                //var player_interaction = other.GetComponent<PlayerInteractionBehaviour>();
                if (player_interaction != null)
                {
                    player = player_interaction;
                }
                
                
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if(!player_interaction.isHolding)
                    {
                        PickUp(player_holding_transform, player);
                    }
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
        }

        void Update()
        {
            if (isBeingHeld)
            {
                ItemCanvas.SetActive(false);
                rb.MovePosition(_parent.TransformPoint(Vector3.zero));
                rb.rotation = _parent.rotation;
            }

            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                if (isBeingHeld)
                {
                    //var player_interaction = other.GetComponent<PlayerInteractionBehaviour>();
                    Drop(_parent);
                }
            }
        }
    }
}

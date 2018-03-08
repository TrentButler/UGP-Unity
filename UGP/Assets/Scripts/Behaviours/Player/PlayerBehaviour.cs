using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        public GameObject VirtualCamera;
        public GameObject model;
        public Player player;
        [HideInInspector] public Player p;

        [SyncVar] public bool isDriving;
        public VehicleBehaviour vehicle;
        public float TimeToExitVehicle;
        private float exitTimer = 0.0f;
        public InGamePlayerMovementBehaviour playerMovement;
        public PlayerInteractionBehaviour interaction;

        [Command] private void CmdExitVehicle(NetworkIdentity identity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = identity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.RemoveClientAuthority(localPlayerConn);
            //localPlayerNetworkIdentity.AssignClientAuthority(localPlayerConn);
        }

        private void ExitVehicle()
        {
            //NEEDS WORK

            if (Input.GetKey(KeyCode.F))
            {
                exitTimer += Time.fixedDeltaTime; //INCREMENT TIME WHILE THE 'F' KEY IS HELD
            }
            else
            {
                exitTimer = 0; //RESET TIMER
            }
        }

        private void Awake()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            p = Instantiate(player);
            p.Alive = true;
            p.Health = p.MaxHealth;
        }
        
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                //if(isClient)
                //{
                //    if (isDriving)
                //    {
                //        model.SetActive(false);
                //    }
                //    else
                //    {
                //        model.SetActive(true);
                //    }
                //}

                VirtualCamera.SetActive(false);
                return;
            }

            if (vehicle == null)
            {
                isDriving = false;
            }
            else
            {
                isDriving = true;
            }
            
            var animator = playerMovement.Ani;

            if (isDriving)
            {
                VirtualCamera.SetActive(false);
                vehicle.enabled = true;
                playerMovement.enabled = false;
                interaction.enabled = false;
                model.SetActive(false);
                animator.SetFloat("Forward", 0.0f);

                animator.SetTrigger("EnterVehicle"); 

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                ExitVehicle(); //EXIT THE VEHICLE

                if(exitTimer >= TimeToExitVehicle)
                {
                    //GET OUT OF VEHICLE
                    vehicle.SetVehicleActive(false);
                    var vehicleIdentity = vehicle.GetComponent<NetworkIdentity>();
                    CmdExitVehicle(vehicleIdentity);
                    vehicle = null;
                    isDriving = false;
                    exitTimer = 0.0f; //RESET THE TIMER
                }
            }
            else
            {
                VirtualCamera.SetActive(true);
                vehicle = null;
                playerMovement.enabled = true;
                interaction.enabled = true;
                model.SetActive(true);

                animator.SetTrigger("ExitVehicle");
            }
        }
    }
}
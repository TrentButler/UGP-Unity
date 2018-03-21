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
        public InputController ic;
        public PlayerInteractionBehaviour interaction;

        public Animator ani;

        [HideInInspector] public bool needsToEnterVehicle;

        [Command] private void CmdExitVehicle(NetworkIdentity identity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = identity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.RemoveClientAuthority(localPlayerConn);
            //localPlayerNetworkIdentity.AssignClientAuthority(localPlayerConn);
        }

        
        public void DisablePlayerModel()
        {
            model.SetActive(false);
        }
        
        public void EnablePlayerModel()
        {
            model.SetActive(true);
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

            if(ani == null)
            {
                ani = GetComponent<Animator>();
            }
            
            needsToEnterVehicle = false;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                if (isClient && !isLocalPlayer)
                {
                    if (isDriving)
                    {
                        DisablePlayerModel();
                    }
                    else
                    {
                        EnablePlayerModel();
                    }
                }

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

            if (isDriving)
            {
                VirtualCamera.SetActive(false);
                vehicle.enabled = true;
                ic.enabled = false;
                interaction.enabled = false;
                DisablePlayerModel();
                //ani.SetFloat("Walk", 0.0f);

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                ExitVehicle(); //EXIT THE VEHICLE

                if (exitTimer >= TimeToExitVehicle)
                {
                    //GET OUT OF VEHICLE
                    //vehicle.ani.SetTrigger("OpenDoor");

                    //var get_out_position = vehicle.seat.position;
                    //get_out_position.x = get_out_position.x - 1.5f;
                    //get_out_position.y += 0.1f;
                    //get_out_position.z += 0.1f;

                    //GetComponent<CharacterController>().Move(get_out_position);

                    //vehicle.ani.SetTrigger("CloseDoor");

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
                ic.enabled = true;
                interaction.enabled = true;
                EnablePlayerModel();
            }
        }
    }
}
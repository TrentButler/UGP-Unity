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

                animator.SetTrigger("EnterVehicle"); 

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                ExitVehicle(); //EXIT THE VEHICLE

                if(exitTimer >= TimeToExitVehicle)
                {
                    //GET OUT OF VEHICLE
                    vehicle.SetVehicleActive(false);
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

                animator.SetTrigger("ExitVehicle");
            }
        }
    }
}
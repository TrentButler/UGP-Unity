using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class PlayerBehaviour : NetworkBehaviour
    {
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
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            p = Instantiate(player);
            p.Alive = true;
            p.Health = p.MaxHealth;
        }
        
        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
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

            var model1 = transform.Find("Model-Head");
            var model2 = transform.Find("Model-Body");

            var rb = GetComponent<Rigidbody>();

            var animator = playerMovement.Ani;

            if (isDriving)
            {
                vehicle.enabled = true;
                playerMovement.enabled = false;
                interaction.enabled = false;

                animator.SetTrigger("EnterVehicle");

                //DISABLE THE PLAYER COLLIDER(S) IF DRIVING
                if (model1 != null)
                {
                    model1.gameObject.SetActive(false);
                }
                if(model2 != null)
                {
                    model2.gameObject.SetActive(false);
                }

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;
                
                rb.isKinematic = true;

                ExitVehicle(); //EXIT THE VEHICLE

                if(exitTimer >= TimeToExitVehicle)
                {
                    //GET OUT OF VEHICLE
                    vehicle._v.ammunition = null;
                    vehicle.SetVehicleActive(false);
                    vehicle = null;
                    isDriving = false;
                    exitTimer = 0.0f; //RESET THE TIMER
                }
            }
            else
            {
                vehicle = null;
                playerMovement.enabled = true;
                interaction.enabled = true;

                animator.SetTrigger("ExitVehicle");

                //ENABLE THE PLAYER COLLIDER(S) IF NOT DRIVING
                if (model1 != null)
                {
                    model1.gameObject.SetActive(true);
                }
                if (model2 != null)
                {
                    model2.gameObject.SetActive(true);
                }
                
                rb.isKinematic = false;
            }
        }
    }
}
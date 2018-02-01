using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{
    public class OfflinePlayerBehaviour : MonoBehaviour
    {
        public Player player;
        private Player p;

       
        public bool isDriving;
        public OfflineVehicleBehaviour vehicle;
        public OfflinePlayerMovementBehaviour playerMovement;
        public OfflinePlayerInteractionBehaviour interaction;

        private void Awake()
        {
    
        }

        private void Start()
        {
            

            if (player != null)
                p = player;

            else
            {
                p = ScriptableObject.CreateInstance<Player>();
                p.Health = 100;
                p.MaxHealth = 100;
            }
        }

        private void FixedUpdate()
        {
            

            if (vehicle == null)
            {
                isDriving = false;
            }
            else
            {
                isDriving = true;
            }

            var col1 = transform.Find("Model-Head").GetComponent<SphereCollider>();
            var col2 = transform.Find("Model-Body").GetComponent<CapsuleCollider>();
            var rb = GetComponent<Rigidbody>();

            if (isDriving)
            {
                vehicle.enabled = true;
                playerMovement.enabled = false;
                interaction.enabled = false;

                //DISABLE THE PLAYER COLLIDER(S) IF DRIVING
                col1.enabled = false;
                col2.enabled = false;

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                rb.isKinematic = true;

                //NEEDS WORK
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //GET OUT OF VEHICLE
                    vehicle.SetVehicleActive(false);
                    vehicle = null;
                    isDriving = false;
                }
            }
            else
            {
                vehicle = null;
                playerMovement.enabled = true;
                interaction.enabled = true;

                //ENABLE THE PLAYER COLLIDER(S) IF NOT DRIVING
                col1.enabled = true;
                col2.enabled = true;

                rb.isKinematic = false;
            }
        }
    }
}

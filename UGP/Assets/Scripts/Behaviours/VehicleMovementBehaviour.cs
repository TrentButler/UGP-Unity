using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Trent
{
    public class VehicleMovementBehaviour : MonoBehaviour
    {
        public float MaxSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;

        private Rigidbody rb;
        
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            var throttle = Input.GetAxis("Vertical");
            var turnVehicle = Input.GetAxis("Horizontal");

            Vector3 movementVector = new Vector3(turnVehicle * TurnSpeed, 0.0f, throttle * MaxSpeed);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                var vertForce = (TargetHeight - hit.distance) / TargetHeight;
                Vector3 hoverVector = Vector3.up * vertForce * hoverStrength;
                Debug.Log(hoverVector);

                rb.AddForce(hoverVector);
            }

            rb.AddRelativeForce(movementVector);
        }
    }
}
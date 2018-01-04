using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Trent
{
    //NEEDS WORK
    public class VehicleMovementBehaviour : MonoBehaviour
    {
        public float MaxSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;
        public float JumpStrength = 1.0f;
        public Vector3 offset;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
                //NEEDS WORK
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        }

        //NEEDS WORK
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

            rb.AddRelativeForce(movementVector + transform.forward); //TRYING TO FOLLOW THE VEHICLE FORWARD

            //NEEDS WORK
            //JUMP
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);
                rb.AddRelativeForce(jumpVector);
            }

            //NEEDS WORK
            #region VehicleAim
            GameObject.FindObjectOfType<MainCameraBehaviour>().transform.position = transform.position + offset;
            GameObject.FindObjectOfType<MainCameraBehaviour>().transform.LookAt(transform);

            var deltaX = Input.GetAxis("Mouse X"); //GET THE MOUSE DELTA X
            var deltaY = Input.GetAxis("Mouse Y"); //GET THE MOUSE DELTA Y

            Vector3 rotX = new Vector3(-deltaY, 0, 0);
            Vector3 rotY = new Vector3(0, deltaX, 0);

            Quaternion rot = Quaternion.Euler(rotX); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            GameObject.FindObjectOfType<MainCameraBehaviour>().transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION


            rot = Quaternion.Euler(rotY); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            GameObject.FindObjectOfType<MainCameraBehaviour>().transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION


            
            #endregion
        }
    }
}
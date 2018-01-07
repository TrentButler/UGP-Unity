using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Trent
{
    public enum VehicleState
    {
        DRIVE = 0,
        COMBAT = 1,
    }

    //NEEDS WORK
    public class VehicleMovementBehaviour : MonoBehaviour
    {
        public VehicleState mode;

        public float MaxSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float TurnStrength = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;

        public float BoostStrength = 1.0f;
        public float JumpStrength = 1.0f;

        public Transform ThrusterPosition;

        private Rigidbody rb;

        public void Steer(Vector3 force)
        {
            //var rot = Quaternion.Euler(new Vector3(0, force, 0));

            //var newRot = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

            //rb.rotation = newRot;

            var rot = Quaternion.Euler(force);

            transform.Rotate(force);
            //rb.MoveRotation(rot);
        }

        public void Aim()
        {
            var deltaX = Input.GetAxis("Mouse X"); //GET THE MOUSE DELTA X
            var deltaY = Input.GetAxis("Mouse Y"); //GET THE MOUSE DELTA Y

            Vector3 rotX = new Vector3(-deltaY, 0, 0);
            Vector3 rotY = new Vector3(0, deltaX, 0);

            Quaternion rot = Quaternion.Euler(rotX); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
            //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

            var firstRot = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            transform.SetPositionAndRotation(transform.position, firstRot);


            rot = Quaternion.Euler(rotY); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
            //transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

            var secondRot = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            transform.SetPositionAndRotation(transform.position, secondRot);
        }

        public void Jump()
        {
            Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);

            rb.AddForce(jumpVector);
        }


        void Start()
        {
            mode = VehicleState.DRIVE;

            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();

            //NEEDS WORK
            //rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        }

        //NEEDS WORK
        void FixedUpdate()
        {
            var throttle = Input.GetAxis("Vertical");
            var turnVehicle = Input.GetAxis("Horizontal");

            Vector3 accelerationVector = new Vector3(0.0f, 0.0f, throttle * MaxSpeed);
            Vector3 steerVector = new Vector3(0.0f, turnVehicle * TurnSpeed, 0.0f);
            
            if(Input.GetKey(KeyCode.LeftControl))
            {
                mode = VehicleState.COMBAT;
            }
            else
            {
                mode = VehicleState.DRIVE;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            switch (mode)
            {
                case VehicleState.DRIVE:
                    {
                        #region HOVERVECTORCALCULATION
                        //PERFORM A RAYCAST DOWNWARD, 
                        //CALCULATE THE DISTANCE FROM BOTTOM OF VEHICLE TO THE GROUND
                        //GENERATE A 'hoverVector' BASED ON THIS CALCULATION
                        RaycastHit hit;
                        if (Physics.Raycast(rb.worldCenterOfMass, -Vector3.up, out hit))
                        {
                            var vertForce = (TargetHeight - hit.distance) / TargetHeight;
                            Vector3 hoverVector = Vector3.up * vertForce * hoverStrength;

                            //Debug.Log(hoverVector); //DELETE THIS

                            rb.AddForce(hoverVector);
                        }
                        #endregion


                        Steer(steerVector);

                        transform.Translate((accelerationVector + ThrusterPosition.forward) * Time.fixedDeltaTime);

                        //rb.AddForceAtPosition(accelerationVector, ThrusterPosition.forward);
                        //rb.AddRelativeTorque(movementVector);
                        break;
                    }

                case VehicleState.COMBAT:
                    {
                        #region HOVERVECTORCALCULATION
                        //PERFORM A RAYCAST DOWNWARD, 
                        //CALCULATE THE DISTANCE FROM BOTTOM OF VEHICLE TO THE GROUND
                        //GENERATE A 'hoverVector' BASED ON THIS CALCULATION
                        RaycastHit hit;
                        if (Physics.Raycast(rb.worldCenterOfMass, -Vector3.up, out hit))
                        {
                            var vertForce = (TargetHeight - hit.distance) / TargetHeight;
                            Vector3 hoverVector = Vector3.up * vertForce * hoverStrength;

                            //Debug.Log(hoverVector); //DELETE THIS

                            rb.AddForce(hoverVector);
                        }
                        #endregion

                        Aim();
                        
                        transform.Translate((accelerationVector + ThrusterPosition.forward) * Time.fixedDeltaTime);

                        //rb.AddForceAtPosition(accelerationVector, ThrusterPosition.position);
                        //rb.AddRelativeTorque(movementVector);
                        break;
                    }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public enum VehicleState
    {
        DRIVE = 0,
        COMBAT = 1,
    }

    //NEEDS WORK
    public class VehicleMovementBehaviour : NetworkBehaviour
    {
        [SyncVar]
        public VehicleState mode;

        public float MaxSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;

        //public float BoostStrength = 1.0f;
        public float JumpStrength = 1.0f;
        public float StrafeSpeed = 1.0f;

        public Transform ThrusterPosition;
        private Rigidbody rb;
        private VehicleShootBehaviour shootBehaviour;

        #region VehicleMovement

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
            //MATCH THE FORWARD FROM THE CAMERA
            //transform.forward = cam.forward;
        }
        public void Aim(Quaternion rot)
        {
            transform.rotation = rot;
        }

        public void Jump()
        {
            Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);

            rb.AddForce(jumpVector);
        }

        public void ResetVehicleRotation()
        {

            var x = Mathf.Abs(transform.up.x);
            var y = Mathf.Abs(transform.up.y);
            var z = Mathf.Abs(transform.up.z);

            rb.isKinematic = true;

            var currRot = transform.rotation; //GET THE CURRENT ROTATION OF THE VEHICLE
            currRot[0] = 0.0f; //RESET THE X ROTATION OF THE VEHICLE
            currRot[2] = 0.0f; //RESET THE Z ROTATION OF THE VEHICLE

            rb.rotation = currRot;
            transform.rotation = currRot;

            rb.isKinematic = false;
        }

        private void KeepVehicleUpright()
        {
            //DETERMINE THE DELTA OF THE CURRENT X AND Z ROTATION OF THE VEHICLE
            //APPLY THE INVERSE OF THE DELTA TO EACH ROTATION

            var rot = transform.rotation;
            var dX = 0 - rot[0];
            var dZ = 0 - rot[2];

            if (dX > 0.0f || dX < 0.0f || dZ > 0.0f || dZ < 0.0f)
                rot[0] = Mathf.LerpAngle(dX, 0.0f, 1.0f);
            rot[2] = Mathf.LerpAngle(dZ, 0.0f, 1.0f);

            rb.rotation = rot;
            transform.rotation = rot;
        }
        #endregion

        void Start()
        {
            if (!isLocalPlayer)
                return;
            

            mode = VehicleState.DRIVE;

            shootBehaviour = GetComponentInParent<VehicleShootBehaviour>();

            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
        }

        //NEEDS WORK
        void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;


            var throttle = Input.GetAxis("Vertical");
            var turnVehicle = Input.GetAxis("Horizontal");

            Vector3 accelerationVector = new Vector3(0.0f, 0.0f, throttle * MaxSpeed);
            Vector3 steerVector = new Vector3(0.0f, turnVehicle * TurnSpeed, 0.0f);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                mode = VehicleState.COMBAT;
            }
            else
            {
                mode = VehicleState.DRIVE;
            }

            if (Input.GetKeyDown(KeyCode.Space))
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

                        //KeepVehicleUpright();
                        shootBehaviour.enabled = false;

                        Steer(steerVector);

                        if (Input.GetKeyDown(KeyCode.LeftAlt))
                            ResetVehicleRotation();

                        transform.Translate((accelerationVector + ThrusterPosition.forward) * Time.fixedDeltaTime);
                        

                        break;
                    }

                case VehicleState.COMBAT:
                    {
                        #region HOVERVECTORCALCULATION
                        ////PERFORM A RAYCAST DOWNWARD, 
                        ////CALCULATE THE DISTANCE FROM BOTTOM OF VEHICLE TO THE GROUND
                        ////GENERATE A 'hoverVector' BASED ON THIS CALCULATION
                        //RaycastHit hit;
                        //if (Physics.Raycast(rb.worldCenterOfMass, -Vector3.up, out hit))
                        //{
                        //    var vertForce = (TargetHeight - hit.distance) / TargetHeight;
                        //    Vector3 hoverVector = Vector3.up * vertForce * hoverStrength;

                        //    //Debug.Log(hoverVector); //DELETE THIS

                        //    rb.AddForce(hoverVector);
                        //}
                        #endregion

                        //KeepVehicleUpright();

                        shootBehaviour.enabled = true;

                        Aim();

                        if (Input.GetKeyDown(KeyCode.LeftAlt))
                            ResetVehicleRotation();

                        transform.Translate(((new Vector3(turnVehicle, 0, throttle) * StrafeSpeed) * Time.fixedDeltaTime));
                        
                        break;
                    }
            }
        }

        private void LateUpdate()
        {
            if (!isLocalPlayer)
                return;

            KeepVehicleUpright();
        }
    }
}
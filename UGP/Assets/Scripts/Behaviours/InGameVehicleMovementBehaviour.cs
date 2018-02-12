using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    //NEEDS WORK
    public class InGameVehicleMovementBehaviour : NetworkBehaviour
    {
        public float MaxSpeed = 1.0f;
        private float originalSpeed;
        public float TurnSpeed = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;

        public float BoostSpeed = 1.0f;
        public float JumpStrength = 1.0f;
        public float StrafeSpeed = 1.0f;
        public float GravityFactor = 1.0f;

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

        public void Steer()
        {
            var dX = Input.GetAxis("Mouse X");

            transform.Rotate(new Vector3(0, dX, 0), Space.Self);
            //rb.rotation = transform.rotation;
        }
        public void Steer(Quaternion rot)
        {
            transform.rotation = rot;
        }

        public void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);

                rb.AddForce(jumpVector);
            }
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

            //rb.rotation = rot;
            transform.rotation = rot;
        }

        public void UseBooster()
        {
            //DEPLETE THE VEHICLE'S 'FUEL' VALUE

            if (Input.GetKey(KeyCode.LeftShift))
            {
                //BOOST
                MaxSpeed = BoostSpeed;
            }
            else
            {
                MaxSpeed = originalSpeed;
            }
        }

        public void Move()
        {
            var throttle = Input.GetAxis("Vertical");
            var turnVehicle = Input.GetAxis("Horizontal");

            //Debug.Log(throttle);

            Vector3 accelerationVector = new Vector3(0.0f, 0.0f, throttle * MaxSpeed);
            Vector3 steerVector = new Vector3(0.0f, turnVehicle * TurnSpeed, 0.0f);

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

            if (accelerationVector.magnitude > 0 || steerVector.magnitude > 0)
            {
                //if(Mathf.Abs(throttle) == 0)
                //{
                //    rb.isKinematic = true;
                //}
                //else
                //{
                //    rb.isKinematic = false;
                //}

                

                //Steer(steerVector);

                //if (Input.GetKeyDown(KeyCode.LeftAlt))
                //    ResetVehicleRotation();

                //transform.Translate((accelerationVector + ThrusterPosition.forward) * Time.fixedDeltaTime);

                //if (Input.GetKeyDown(KeyCode.LeftAlt))
                //    ResetVehicleRotation();
                
                UseBooster();
                Jump();

                //var accel = (accelerationVector + ThrusterPosition.forward);
                var accel = (accelerationVector + ThrusterPosition.forward);
                var strafe = (new Vector3(turnVehicle, 0, 0) * StrafeSpeed);

                transform.Translate((accel + strafe) * Time.fixedDeltaTime);
            }

            Steer();
        }
        #endregion

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

            //gameObject.name = netId.ToString();

            //Cursor.visible = false;

            shootBehaviour = GetComponentInParent<VehicleShootBehaviour>();

            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();

            originalSpeed = MaxSpeed;
        }

        //NEEDS WORK
        void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            Move();
        }

        private void LateUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            KeepVehicleUpright();
        }
    }
}
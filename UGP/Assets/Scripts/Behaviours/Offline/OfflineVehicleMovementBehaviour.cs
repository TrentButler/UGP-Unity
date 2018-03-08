using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineVehicleMovementBehaviour : MonoBehaviour
    {
        public float MaxSpeed = 1.0f;
        private float originalSpeed;
        public float TurnSpeed = 1.0f;
        public float hoverStrength = 1.0f;
        public float TargetHeight = 4.0f;

        public float BoostSpeed = 1.0f;
        public float JumpStrength = 1.0f;
        public float StrafeSpeed = 1.0f;

        public Transform ThrusterPosition;
        private Rigidbody rb;

        #region VehicleMovement
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
            {
                rot[0] = Mathf.LerpAngle(dX, 0.0f, Time.fixedDeltaTime);
                rot[2] = Mathf.LerpAngle(dZ, 0.0f, Time.fixedDeltaTime);
            }

            transform.rotation = rot;
        }

        public void Steer()
        {
            var dX = Input.GetAxis("Mouse X");

            Vector3 mouseYRot = new Vector3(0, dX, 0);

            if (mouseYRot.magnitude > 0)
            {
                transform.Rotate(mouseYRot, Space.Self);
            }
            rb.MoveRotation(transform.rotation);
        }

        public void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);

                rb.AddForce(jumpVector);
            }
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
            var strafeVehicle = Input.GetAxis("Horizontal");

            Vector3 accelerationVector = new Vector3(0.0f, 0.0f, throttle * MaxSpeed);
            Vector3 strafeVector = new Vector3(strafeVehicle * TurnSpeed, 0, 0.0f);

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

            if (accelerationVector.magnitude > 0 || strafeVector.magnitude > 0)
            {
                UseBooster();
                Jump();

                //MOVE FORWARD BASED ON THE TRANSFORM'S FORWARD
                var currentVelocity = rb.velocity;
                if (currentVelocity.magnitude < MaxSpeed)
                {
                    var accel = (accelerationVector + ThrusterPosition.forward);
                    var strafe = (new Vector3(strafeVehicle, 0, 0) * StrafeSpeed);

                    transform.Translate((accel + strafe) * Time.fixedDeltaTime);
                    rb.MovePosition(transform.position);
                    //rb.AddForce(0, 0, throttle, ForceMode.Impulse);
                    //rb.AddForceAtPosition()
                }
            }
            //else
            //{
            //    rb.velocity = Vector3.zero;
            //    rb.rotation = transform.rotation;
            //}
            Steer();
        }
        #endregion

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();

            originalSpeed = MaxSpeed;
        }

        void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            KeepVehicleUpright();
        }
    }
}
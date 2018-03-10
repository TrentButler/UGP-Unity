using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineVehicleMovementBehaviour : MonoBehaviour
    {
        #region VehicleHover
        public Vector3 CurrentHoverVector;
        public float TargetHeight = 4.0f;
        public float EvasionHoverHeight = 1.0f;
        public float HoverStrength = 1.0f;
        public float EvasionHoverStrength = 1.0f; 
        private float originalHoverStrength;
        private float originalTargetHeight;
        #endregion

        #region VehicleOrientation
        public Vector3 maxVehicleRotation;
        public Vector3 minVehicleRotation;
        public float vehicleRotateSpeed;
        #endregion

        public float MaxSpeed = 1.0f;
        public float StrafeSpeed = 1.0f;
        public float VehicleSteerSpeed = 1.0f;
        public float BoostSpeed = 1.0f;
        private float originalSpeed;
        private float originalStrafeSpeed;
        
        public Transform ThrusterPosition;
        private Rigidbody rb;

        [HideInInspector] public float currentVehicleThrottle;
        [HideInInspector] public float currentVehicleStrafe;

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

        private void CheckVehicleRotation()
        {
            //GET THE VEHICLE'S CURRENT ROTATION
            //COMPARE EACH ROTATION AMOUNT TO THE MIN/MAX ROTATION
            //IF ANY ROTATION IS PAST THE BOUNDARY, 
            //LERP BETWEEN THE VALUES
            var currentRot = transform.rotation;
            var currentX = currentRot[0];
            var currentY = currentRot[1];
            var currentZ = currentRot[2];

            //CHECK THE X ROTATION
            if (currentX > maxVehicleRotation.x)
            {
                currentX = Mathf.LerpAngle(currentX, maxVehicleRotation.x, vehicleRotateSpeed * Time.smoothDeltaTime);
            }
            if (currentX < minVehicleRotation.x)
            {
                currentX = Mathf.LerpAngle(currentX, minVehicleRotation.x, vehicleRotateSpeed * Time.smoothDeltaTime);
            }

            //CHECK THE Y ROTATION
            if (currentY > maxVehicleRotation.y)
            {
                currentY = Mathf.LerpAngle(currentY, maxVehicleRotation.y, vehicleRotateSpeed * Time.smoothDeltaTime);
            }
            if (currentY < minVehicleRotation.y)
            {
                currentY = Mathf.LerpAngle(currentY, minVehicleRotation.y, vehicleRotateSpeed * Time.smoothDeltaTime);
            }

            //CHECK THE Z ROTATION
            if (currentZ > maxVehicleRotation.z)
            {
                currentZ = Mathf.LerpAngle(currentZ, maxVehicleRotation.z, vehicleRotateSpeed * Time.smoothDeltaTime);
            }
            if (currentZ < minVehicleRotation.z)
            {
                currentZ = Mathf.LerpAngle(currentZ, minVehicleRotation.z, vehicleRotateSpeed * Time.smoothDeltaTime);
            }

            currentRot[0] = currentX;
            currentRot[1] = currentY;
            currentRot[2] = currentZ;

            transform.rotation = currentRot;
        }

        public void Steer()
        {
            var dX = Input.GetAxis("Mouse X");

            Vector3 mouseYRot = new Vector3(0, dX, 0);

            if (mouseYRot.magnitude > 0)
            {
                transform.Rotate(mouseYRot * VehicleSteerSpeed, Space.Self);
            }
            rb.MoveRotation(transform.rotation);
        }

        //NEEDS WORK
        //HOLDING THE 'Break' BUTTON WHILE HOLDING THE 'Hover' BUTTON GIVES THE DESIRED RESULT
        //SEPERATE THIS, SO ONLY THE 'Hover' BUTTON IS NEEDED TO 'HOVER' IN PLACE
        public void Hover()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rb.useGravity = false;
                TargetHeight = EvasionHoverHeight;
                HoverStrength = EvasionHoverStrength;
            }
            else
            {
                rb.useGravity = true;
                TargetHeight = originalTargetHeight;
                HoverStrength = originalHoverStrength;
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

        public void ApplyBreak()
        {
            if (Input.GetKey(KeyCode.C)) //SLOW DOWN TO A STOP
            {
                MaxSpeed = 0.0f;
                StrafeSpeed = 0.0f;
                rb.MoveRotation(transform.rotation);
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime);
            }
            else
            {
                MaxSpeed = originalSpeed;
                StrafeSpeed = originalStrafeSpeed;
            }
        }

        //NEEDS WORK
        //WHEN APPLYING A FORCE BACKWARDS (S KEY PRESS), VEHICLE MOVES UPWARD/BACKWARD TOWARD THE CAMERA
        public void Move()
        {
            var throttle = Input.GetAxis("Vertical");
            var strafeVehicle = Input.GetAxis("Horizontal");

            currentVehicleThrottle = throttle;
            currentVehicleStrafe = strafeVehicle;

            Vector3 accelerationVector = new Vector3(0.0f, 0.0f, throttle * MaxSpeed);
            Vector3 strafeVector = new Vector3(strafeVehicle * StrafeSpeed, 0, 0.0f);

            #region HOVERVECTORCALCULATION
            //PERFORM A RAYCAST DOWNWARD, 
            //CALCULATE THE DISTANCE FROM BOTTOM OF VEHICLE TO THE GROUND
            //GENERATE A 'hoverVector' BASED ON THIS CALCULATION
            RaycastHit hit;
            //var world_point = transform.TransformPoint(point.position);
            if (Physics.Raycast(rb.worldCenterOfMass, -Vector3.up, out hit))
            {
                var vertForce = (TargetHeight - hit.distance) / TargetHeight;
                Vector3 hoverVector = Vector3.up * vertForce * HoverStrength;

                //Debug.Log(hoverVector); //DELETE THIS
                CurrentHoverVector = hoverVector;

                rb.AddForce(hoverVector);
                //rb.AddForceAtPosition(hoverVector, point.position);
            }
            #endregion

            if (accelerationVector.magnitude > 0 || strafeVector.magnitude > 0)
            {
                UseBooster();

                var cam_transform = Camera.main.transform;
                var move_direction = cam_transform.TransformDirection(((accelerationVector) + strafeVector));

                rb.AddForce(move_direction, ForceMode.Impulse);
            }

            Steer();
            Hover();
            ApplyBreak();   
        }
        #endregion

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();

            originalSpeed = MaxSpeed;
            originalStrafeSpeed = StrafeSpeed;
            originalTargetHeight = TargetHeight;
            originalHoverStrength = HoverStrength;
        }

        void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            //KeepVehicleUpright();
            CheckVehicleRotation();
        }
    }
}
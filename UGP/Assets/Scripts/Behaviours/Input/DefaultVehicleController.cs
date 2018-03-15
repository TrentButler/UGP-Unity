using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class DefaultVehicleController : InputController
    {
        #region VehicleHover
        public Vector3 CurrentHoverVector;
        public float TargetHeight = 4.0f;
        public float EvasionHoverHeight = 20.0f;
        public float HoverStrength = 14.0f;
        public float EvasionHoverStrength = 50.0f;
        private float originalHoverStrength;
        private float originalTargetHeight;
        #endregion

        #region VehicleOrientation
        public Vector3 maxVehicleRotation;
        public Vector3 minVehicleRotation;
        public float vehicleRotateSpeed = 1.5f;
        #endregion

        public float MaxSpeed = 50.0f;
        public float VehicleDecelerateRate = 3.0f;
        public float StrafeSpeed = 5.0f;
        public float VehicleSteerSpeed = 1.0f;
        public float BoostSpeed = 100.0f;
        private float originalSpeed;
        private float originalStrafeSpeed;

        public bool isTesting = false;
        
        private Rigidbody rb;

        [HideInInspector] public float currentVehicleThrottle;
        [HideInInspector] public float currentVehicleStrafe;

        #region VehicleMovement
        public void ResetVehicleRotation()
        {
            var currRot = transform.rotation; //GET THE CURRENT ROTATION OF THE VEHICLE
            currRot[0] = 0.0f; //RESET THE X ROTATION OF THE VEHICLE
            currRot[2] = 0.0f; //RESET THE Z ROTATION OF THE VEHICLE

            rb.rotation = currRot;
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
            //var currentRot = transform.rotation;
            var currentRot = rb.rotation;
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

            //transform.rotation = currentRot;
            rb.rotation = currentRot;
        }
        
        public void Hover()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                //INCREASE THE 'TargetHeight' AND THE 'HoverStrength'
                //PREVENT VEHICLE ACCELERATION OR STRAFING
                MaxSpeed = 0.0f;
                StrafeSpeed = 0.0f;
                rb.useGravity = false;
                TargetHeight = EvasionHoverHeight;
                HoverStrength = EvasionHoverStrength;

                var current_velocity = rb.velocity;
                current_velocity[0] = 0.0f;
                current_velocity[2] = 0.0f;
                rb.velocity = current_velocity;
            }
            else
            {
                if(isTesting)
                {
                    return;
                }
                //RETURN THE 'TargetHeight' AND THE 'HoverStrength' TO THEIR ORIGINAL VALUES
                MaxSpeed = originalSpeed;
                StrafeSpeed = originalStrafeSpeed;
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
                if (isTesting)
                {
                    return;
                }
                MaxSpeed = originalSpeed;
            }
        }

        public void ApplyBreak()
        {
            if (Input.GetKey(KeyCode.C))
            {
                //SLOW DOWN TO A STOP, PREVENT VEHICLE FROM ACCELERATING OR STRAFING
                MaxSpeed = 0.0f;
                StrafeSpeed = 0.0f;

                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, VehicleDecelerateRate * Time.fixedDeltaTime);
            }
            //else
            //{
            //    MaxSpeed = originalSpeed;
            //    StrafeSpeed = originalStrafeSpeed;
            //}
        }
        #endregion

        public override void Move(float x, float y)
        {
            var throttle = y;
            var strafeVehicle = x;

            currentVehicleThrottle = throttle;
            currentVehicleStrafe = strafeVehicle;

            Hover();
            UseBooster();
            ApplyBreak();

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
                //OLD
                ////GET THE TRANSFORM OF THE VEHICLE CAMERA
                //var cam_transform = Camera.main.transform;
                //var cam_rotation = cam_transform.rotation;
                //cam_rotation[0] = 0.0f; //ZERO OUT THE X-AXIS ROTATION
                //cam_rotation[2] = 0.0f; //ZERO OUT THE Z-AXIS ROTATION
                //cam_transform.rotation = cam_rotation;

                //DERIVE A MOVEMENT DIRECTION BY USING THE CAMERA'S TRANSFORM
                var move_direction = transform.TransformDirection((accelerationVector + strafeVector));
                //APPLY FORCES
                rb.AddForce(move_direction, ForceMode.Impulse);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, VehicleDecelerateRate * Time.smoothDeltaTime); //DECELERATE IF THERE IS NO MOVEMENT INPUT
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
        }

        public override void Rotate(float xRot, float yRot, float zRot)
        {
            var mouseYRot = new Vector3(0, xRot, 0);
            var mouseXZRot = new Vector3(yRot, 0, xRot);

            if (mouseYRot.magnitude > 0)
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    rb.constraints = RigidbodyConstraints.None; //REMOVE ALL CONSTRAINTS FROM THE RIGIDBODY
                    var z_rotation = Quaternion.Euler(mouseXZRot * VehicleSteerSpeed);
                    rb.MoveRotation(rb.rotation * z_rotation);
                    return;
                }

                rb.constraints = RigidbodyConstraints.None; //REMOVE ALL CONSTRAINTS FROM THE RIGIDBODY
                var new_rotation = Quaternion.Euler(mouseYRot * VehicleSteerSpeed);
                rb.MoveRotation(rb.rotation * new_rotation);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationY; //FREEZE THE Y-AXIS ROTATION
            }
        }

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
    }
}
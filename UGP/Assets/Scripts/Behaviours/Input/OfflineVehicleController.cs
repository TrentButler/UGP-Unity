using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineVehicleController : InputController
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
        private bool isRotating = false;
        #endregion

        public float MaxSpeed = 50.0f;
        public float VehicleDecelerateRate = 3.0f;
        public float StrafeSpeed = 5.0f;
        public float VehicleSteerSpeed = 1.0f;
        public float BoostSpeed = 100.0f;
        private float originalSpeed;
        private float originalStrafeSpeed;

        public bool isTesting = false;
        public bool useDownforce = true;
        public Vector3 Downforce;

        private Rigidbody rb;

        [HideInInspector] public float currentVehicleThrottle;
        [HideInInspector] public float currentVehicleStrafe;
        public float currentFuelConsumption;
        [Range(0.001f, 1.0f)] public float FuelBurnRate = 0.5f;
        [Range(0.001f, 2.0f)] public float BoostingFuelBurnRate = 1.0f;

        private float originalFuelBurnRate;

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
            var rot = rb.rotation;
            var euler_rot = rb.rotation.eulerAngles;

            var target_rotation = new Vector3(0, euler_rot[1], 0);

            var move_rotation = Quaternion.Euler(target_rotation);
            var new_rot = Quaternion.Slerp(rb.rotation, move_rotation, Time.smoothDeltaTime);
            new_rot[1] = rot[1];

            rb.rotation = new_rot;
        }

        private void CheckVehicleRotation()
        {
            var currentRot = rb.rotation.eulerAngles;
            var currentX = currentRot[0];
            var currentZ = currentRot[2];

            //CHECK THE X ROTATION
            if (currentX > maxVehicleRotation.x)
            {
                KeepVehicleUpright();
            }
            if (currentX < minVehicleRotation.x)
            {
                KeepVehicleUpright();
            }

            //CHECK THE Z ROTATION
            if (currentZ > maxVehicleRotation.z)
            {
                KeepVehicleUpright();
            }
            if (currentZ < minVehicleRotation.z)
            {
                KeepVehicleUpright();
            }
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
                if (isTesting)
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
                FuelBurnRate = BoostingFuelBurnRate;
            }
            else
            {
                if (isTesting)
                {
                    return;
                }
                MaxSpeed = originalSpeed;
                FuelBurnRate = originalFuelBurnRate;
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
            
            currentFuelConsumption = Mathf.Abs(throttle + strafeVehicle) * FuelBurnRate;

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

            if (rb.centerOfMass.y > TargetHeight)
            {
                var displacement_y = TargetHeight - rb.transform.position.y;
                var force_vector = (new Vector3(0.0f, displacement_y, 0.0f) + (-Vector3.up)) * EvasionHoverStrength;
                Downforce = force_vector;
                rb.AddForce(force_vector);
            }

            if (accelerationVector.magnitude > 0 || strafeVector.magnitude > 0)
            {
                var move_direction = transform.TransformDirection((accelerationVector + strafeVector));
                //APPLY FORCES
                rb.isKinematic = false;
                rb.AddForce(move_direction, ForceMode.Impulse);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, VehicleDecelerateRate * Time.smoothDeltaTime); //DECELERATE IF THERE IS NO MOVEMENT INPUT
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);

            if (!isRotating)
            {
                CheckVehicleRotation();
            }
        }

        public override void Rotate(float xRot, float yRot, float zRot)
        {
            var x_rot = new Vector3(yRot, 0, 0);
            var y_rot = new Vector3(0, xRot, 0);
            var z_rot = new Vector3(0, 0, -xRot);


            if (Input.GetKey(KeyCode.LeftAlt))
            {
                MaxSpeed = 0.0f;
                StrafeSpeed = 0.0f;
                isRotating = true;

                rb.constraints = RigidbodyConstraints.None; //REMOVE ALL CONSTRAINTS FROM THE RIGIDBODY
                var z_rotation = Quaternion.Euler((z_rot + x_rot) * VehicleSteerSpeed);
                rb.MoveRotation(rb.rotation * z_rotation);
                return;
            }
            else
            {
                MaxSpeed = originalSpeed;
                StrafeSpeed = originalStrafeSpeed;
                isRotating = false;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; //FREEZE THE Y-AXIS ROTATION
            }

            if (y_rot.magnitude > 0.0f)
            {
                rb.constraints = RigidbodyConstraints.None; //REMOVE ALL CONSTRAINTS FROM THE RIGIDBODY
                var new_rotation = Quaternion.Euler(y_rot * VehicleSteerSpeed);
                rb.MoveRotation(rb.rotation * new_rotation);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation; //FREEZE THE Y-AXIS ROTATION    
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
            originalFuelBurnRate = FuelBurnRate;
        }
    }
}
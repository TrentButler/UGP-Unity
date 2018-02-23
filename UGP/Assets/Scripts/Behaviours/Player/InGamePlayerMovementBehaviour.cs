using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class InGamePlayerMovementBehaviour : NetworkBehaviour
    {
        #region MemeberVariables
        public float WalkSpeed = 1.0f;
        public float RunSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        private float OriginalSpeed;
        public float JumpStrength = 150.0f;
        private bool HasJumped;
        private bool HasSprinted;
        private float SprintTimer = 1.0f;
        private float JumpTimer = 1.0f;
        public bool isGrounded;
        
        public Animator Ani;
        private Rigidbody rb;
        #endregion

        public void Sprint()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //BOOST
                WalkSpeed = RunSpeed;
                Ani.SetBool("Sprinting", true);
            }
            else
            {
                WalkSpeed = OriginalSpeed;
                Ani.SetBool("Sprinting", false);
            }
        }
        public void Jump()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);
                Ani.SetTrigger("Jump");

                rb.AddForce(jumpVector);
            }
        }
        //private void GroundCheck()
        //{
        //    previouslyGrounded = isGrounded;
        //    RaycastHit hitInfo;
        //    if (Physics.SphereCast(transform.position, Capsule.radius, Vector3.down, out hitInfo,
        //                           ((Capsule.height / 2f) - Capsule.radius) + advancedSettings.groundCheckDistance))
        //    {
        //        isGrounded = true;
        //        groundContactNormal = hitInfo.normal;
        //    }
        //    else
        //    {
        //        isGrounded = false;
        //        groundContactNormal = Vector3.up;
        //    }
        //    if (!previouslyGrounded && isGrounded && jumping)
        //    {
        //        jumping = false;
        //    }
        //}

        private void KeepPlayerUpright()
        {
            //DETERMINE THE DELTA OF THE CURRENT X AND Z ROTATION OF THE VEHICLE
            //APPLY THE INVERSE OF THE DELTA TO EACH ROTATION

            var rot = transform.rotation;
            var dX = 0 - rot[0];
            var dZ = 0 - rot[2];

            if (dX > 0.0f || dX < 0.0f || dZ > 0.0f || dZ < 0.0f)
            {
                rot[0] = Mathf.LerpAngle(dX, 0.0f, 1.0f);
                rot[2] = Mathf.LerpAngle(dZ, 0.0f, 1.0f);
            }

            transform.rotation = rot;
        }

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

            OriginalSpeed = WalkSpeed;
            rb = GetComponent<Rigidbody>();
            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }


            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            Ani.SetFloat("Forward", v);

            Vector3 moveForward = new Vector3(0.0f, 0.0f, v * WalkSpeed);
            Vector3 YRot = new Vector3(0.0f, h * TurnSpeed, 0.0f);

            if (moveForward.magnitude > 0)
            {
                var move = (moveForward + transform.forward);
                transform.Translate(move * Time.fixedDeltaTime);
            }

            if (YRot.magnitude > 0)
            {
                transform.Rotate(YRot);
            }
            Sprint();
            Jump();
        }
        //    if (isGrounded)
        //    {
        //        RigidBody.drag = 5f;

        //        if (jump)
        //        {
        //            RigidBody.drag = 0f;
        //            RigidBody.velocity = new Vector3(RigidBody.velocity.x, 0f, RigidBody.velocity.z);
        //            RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
        //            jumping = true;
        //        }

        //        if (!jumping && input.x == 0f && input.y == 0f && RigidBody.velocity.magnitude < 1f)
        //        {
        //            RigidBody.Sleep();
        //        }
        //    }
        //    else
        //    {
        //        RigidBody.drag = 0f;
        //        if (previouslyGrounded && !jumping)
        //        {
        //            StickToGroundHelper();
        //        }
        //    }
        //    jump = false;
        //}

        private void LateUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            KeepPlayerUpright();
        }
    }
}
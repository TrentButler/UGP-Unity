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

        public float JumpStrength = 1.0f;

        public Animator Ani;
        private Rigidbody rb;
        #endregion

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
        }

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
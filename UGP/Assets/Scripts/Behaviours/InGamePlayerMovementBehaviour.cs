using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public enum InGamePlayerState
    {
        IDLE = 0,
        WALK = 1,
        SPRINT = 2,
        DRIVE = 3,
    }

    public class InGamePlayerMovementBehaviour : MonoBehaviour
    {
        #region MemeberVariables
        public InGamePlayerState state;

        public float WalkSpeed = 1.0f;
        public float RunSpeed = 1.0f;
        public float TurnSpeed = 1.0f;

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
                rot[0] = Mathf.LerpAngle(dX, 0.0f, 1.0f);
            rot[2] = Mathf.LerpAngle(dZ, 0.0f, 1.0f);

            rb.rotation = rot;
            transform.rotation = rot;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (!rb)
                rb = gameObject.AddComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            Vector3 moveVector = new Vector3(0.0f, 0.0f, v * WalkSpeed);

            //transform.Rotate(new Vector3(0.0f, h * TurnSpeed, 0.0f));

            //transform.Translate((moveVector + transform.forward) * Time.fixedDeltaTime);

            transform.Translate(((new Vector3(h, 0, v) * WalkSpeed) * Time.fixedDeltaTime));
        }

        private void LateUpdate()
        {
            KeepPlayerUpright();
        }
    }
}
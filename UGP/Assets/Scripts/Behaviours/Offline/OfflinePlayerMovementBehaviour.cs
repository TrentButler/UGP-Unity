using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{


    public class OfflinePlayerMovementBehaviour : MonoBehaviour
    {

        #region MemeberVariables
        public float WalkSpeed = 1.0f;
        public float RunSpeed = 1.0f;
        public float TurnSpeed = 1.0f;

        public float JumpStrength = 1.0f;
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
            
        }

        private void FixedUpdate()
        {

            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            //NEEDS WORK
            //PLAYER MOVE FORWARD WITHOUT BUTTON PRESS

            Vector3 moveForward = new Vector3(0.0f, 0.0f, v * WalkSpeed);
            var move = (moveForward + transform.forward);

            var YRot = new Vector3(0.0f, h * TurnSpeed, 0.0f);
            //Debug.Log(move);
            //Debug.Log(YRot);

            transform.Rotate(YRot);
            transform.Translate(move * Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
           
            KeepPlayerUpright();
        }
    }
}
// Use this for initialization

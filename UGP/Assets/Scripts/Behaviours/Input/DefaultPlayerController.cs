using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class DefaultPlayerController : InputController
    {
        #region MemberVariables
        public float MovementSpeed = 1.0f;
        public float RunSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float JumpStrength = 2.0f;
        private float OriginalSpeed;

        public Animator Ani;
        public CharacterController controller;
        #endregion

        public void Jump()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                var half_velocity = controller.velocity / 2;
                var jump_vector = (Vector3.up * JumpStrength) + half_velocity;
                controller.Move(jump_vector);
            }
        }

        public void Sprint()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MovementSpeed = RunSpeed;
                Ani.SetBool("Running", true);
            }
            else
            {
                MovementSpeed = OriginalSpeed;
                Ani.SetBool("Running", false);
            }
        }

        private void KeepPlayerUpright()
        {
            var rot = transform.rotation;
            rot[0] = 0.0f;
            rot[2] = 0.0f;

            controller.transform.rotation = rot;
        }

        public override void Move(float x, float y)
        {
            Sprint();
            Jump();
            KeepPlayerUpright();

            //DO NOT WALK BACKWARDS
            if (y < 0.0f)
            {
                y = 0;
            }

            var move_vector = new Vector3(0, 0, y);
            var move = controller.transform.TransformDirection(move_vector);

            controller.SimpleMove(move * MovementSpeed);
        }

        public override void Rotate(float xRot, float yRot, float zRot)
        {
            var rotate_vector = new Vector3(0, xRot, 0);
            controller.transform.Rotate(rotate_vector * TurnSpeed);
        }

        private void Start()
        {
            OriginalSpeed = MovementSpeed;
        }
    }
}

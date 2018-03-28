using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class ShannonSharpePlayerController : InputController
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
            if (Input.GetKeyDown(KeyCode.Space))
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

            //ATTEMPT AT PLAYER MOVEMENT SIMULAR TO A TWIN STICK SHOOTER
            //Ani.SetFloat("Move", Mathf.Abs(x + y));
            //var rotate_direction = new Vector3(x, 0, y);
            //controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate_direction), Time.smoothDeltaTime * TurnSpeed);

            //DO NOT WALK BACKWARDS
            if (y < 0.0f)
            {
                //sy = y / 2;
                var move_vector = new Vector3(0, 0, y);
                var move = controller.transform.TransformDirection(move_vector);
                controller.SimpleMove(move * 1);
            }
            else
            {
                Ani.SetFloat("Move", Mathf.Abs(y));
                var move_vector = new Vector3(0, 0, y);
                var move = controller.transform.TransformDirection(move_vector);
                controller.SimpleMove(move * MovementSpeed);
            }
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

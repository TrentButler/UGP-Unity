﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class ShannonSharpePlayerController : InputController
    {
        #region MemberVariables
        public float MovementSpeed = 1.0f;
        public float StrafeSpeed = 1.0f;
        public float RunSpeed = 1.0f;
        public float TurnSpeed = 1.0f;
        public float JumpStrength = 2.0f;
        private float OriginalSpeed;
        public Animator Ani;
        public NetworkAnimator NetworkAni;
        public CharacterController controller;
        //public Rigidbody rb;
        #endregion

        public void Melee()
        {
            if (Input.GetMouseButton(1))
            {
                var colliders = GetComponents<Collider>().ToList();
                colliders.ForEach(collider =>
                {
                    if (collider.CompareTag("Hand"))
                    {
                        collider.enabled = true;
                    }
                });


                Ani.SetBool("Fighting", true);
                if(Input.GetMouseButtonDown(0))
                {
                    NetworkAni.SetTrigger("Punch");
                }
            }
            else
            {
                Ani.SetBool("Fighting", false);
            }
        }
        public void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //var half_velocity = controller.velocity / 2;
                //var jump_vector = (Vector3.up * JumpStrength) + half_velocity;
                //controller.Move(jump_vector);
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

            //rb.rotation = rot;
            controller.transform.rotation = rot;
        }

        public override void Move(float x, float y)
        {
            Sprint();
            //Jump();
            Melee();
            KeepPlayerUpright();

            var accelerationVector = new Vector3(0, 0, y * MovementSpeed);
            var strafeVector = new Vector3(x * StrafeSpeed, 0, 0);
            var moveVector = accelerationVector + strafeVector;

            if (moveVector.magnitude > 0)
            {
                Ani.SetFloat("Move", Mathf.Abs(y));
                var move_vector = moveVector;
                var move = transform.TransformDirection(move_vector);
                controller.SimpleMove(move);
                //rb.AddForce(move * MovementSpeed, ForceMode.Impulse);
            }
            else
            {
                Ani.SetFloat("Move", Mathf.Abs(y));
                //rb.isKinematic = true;
                //rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 3 * Time.smoothDeltaTime); //DECELERATE IF THERE IS NO MOVEMENT INPUT
            }

            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, MovementSpeed);
        }

        public override void Rotate(float xRot, float yRot, float zRot)
        {
            var rotate_vector = new Vector3(0, xRot * TurnSpeed, 0);
            
            if(rotate_vector.magnitude > 0.0f)
            {
                controller.transform.Rotate(rotate_vector * TurnSpeed);
            }
        }

        private void Start()
        {
            OriginalSpeed = MovementSpeed;
            //rb = GetComponent<Rigidbody>();
        }
    }
}
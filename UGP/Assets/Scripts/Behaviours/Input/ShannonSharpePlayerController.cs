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
        //public CharacterController controller;
        public Rigidbody rb;
        #endregion

        public void Melee()
        {
            if (Input.GetMouseButton(1))
            {
                Ani.SetBool("Fighting", true);
                if(Input.GetMouseButtonDown(0))
                {
                    Ani.SetTrigger("Punch");
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

            rb.rotation = rot;
        }

        public override void Move(float x, float y)
        {
            Sprint();
            //Jump();
            Melee();
            KeepPlayerUpright();

            var accelerationVector = new Vector3(0, 0, y);

            if (accelerationVector.magnitude > 0)
            {
                //DO NOT WALK BACKWARDS
                if (y < 0.0f)
                {
                    //sy = y / 2;
                    var move_vector = new Vector3(0, 0, y);
                    var move = transform.TransformDirection(move_vector);
                    //controller.SimpleMove(move * 1);
                    rb.AddForce(move * 1);
                }
                else
                {
                    Ani.SetFloat("Move", Mathf.Abs(y));
                    var move_vector = new Vector3(0, 0, y);
                    var move = transform.TransformDirection(move_vector);
                    //controller.SimpleMove(move * MovementSpeed);
                    rb.AddForce(move * MovementSpeed, ForceMode.Impulse);
                }
            }
            else
            {
                Ani.SetFloat("Move", Mathf.Abs(y));
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 3 * Time.smoothDeltaTime); //DECELERATE IF THERE IS NO MOVEMENT INPUT
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MovementSpeed);
        }

        public override void Rotate(float xRot, float yRot, float zRot)
        {
            var rotate_vector = new Vector3(0, xRot * TurnSpeed, 0);
            //controller.transform.Rotate(rotate_vector * TurnSpeed);
            if(rotate_vector.magnitude > 0.0f)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotate_vector));
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void Start()
        {
            OriginalSpeed = MovementSpeed;
            rb = GetComponent<Rigidbody>();
        }
    }
}
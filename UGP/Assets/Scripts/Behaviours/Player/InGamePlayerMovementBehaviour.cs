using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class InGamePlayerMovementBehaviour : NetworkBehaviour
    {
        public GameObject VirtualCamera;
        #region MemberVariables
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
        public NetworkPlayer Player;
        public GameObject MapCanvas;
        public GameObject SettingMenu;
        public Animator Ani;
        private Rigidbody rb;
        public CharacterController controller;
        #endregion

        private void Start()
        {   
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }
            MapCanvas.SetActive(false);
            VirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>().Follow = transform;
            VirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCameraBase>().LookAt = transform;
            OriginalSpeed = WalkSpeed;
            //rb = GetComponent<Rigidbody>();
            //if (!rb)
            //    rb = gameObject.AddComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;


            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            Ani.SetFloat("Forward", v);

            Vector3 moveForward = new Vector3(0.0f, 0.0f, v * WalkSpeed);
            Vector3 YRot = new Vector3(0.0f, h * TurnSpeed, 0.0f);

            if (moveForward.magnitude > 0)
            {
                var move = (moveForward + transform.forward);
                //transform.Translate(move * Time.fixedDeltaTime);
                //rb.AddForce(move);
                controller.SimpleMove(move);
            }

            if (YRot.magnitude > 0)
            {
                transform.Rotate(YRot);
                //rb.rotation = transform.rotation;
            }
            Sprint();
            Crouch();
            Jump();
            ToggleMap();
            ToggleSettings();
        }

        private void LateUpdate()
        {
            if (!isLocalPlayer)
                return;


            KeepPlayerUpright();
        }

        public void ToggleSettings()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SettingMenu.SetActive(true);
            }
            else
            {
                SettingMenu.SetActive(false);
            }
           
        }
        public void ReturnMenu()
        {
            NetworkManager.Shutdown();
            
            SceneManager.LoadScene("69.OfflineScene");
        }
        public void Exit()
        {
            Application.Quit();
        }
        public void ToggleMap()
        {
            if (Input.GetKey(KeyCode.M))
            {
                MapCanvas.SetActive(true);
            }
            else
            {
                MapCanvas.SetActive(false);
            }
            
        }
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
        public void Crouch()
        {
            if (Input.GetKey(KeyCode.C))
            {
                WalkSpeed = 1.5f;
                Ani.SetBool("Crouch", true);
            }
            else
            {
                WalkSpeed = OriginalSpeed;
                Ani.SetBool("Crouch", false);
            }
        }
        public void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpVector = new Vector3(0, 1 * JumpStrength, 0);
                Ani.SetTrigger("Jump");

                //rb.AddForce(jumpVector);
                controller.Move(jumpVector);
            }
        }

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

    }
}
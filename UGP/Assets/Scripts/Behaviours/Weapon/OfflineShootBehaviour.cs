using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UGP
{
    public class OfflineShootBehaviour : MonoBehaviour
    {
        public List<GameObject> weapons = new List<GameObject>();
        public OfflineWeaponBehaviour weapon;
        public Transform GunTransform;
        public AudioSource audio;

        public GameObject DrivingCamera;
        public GameObject AimCamera;

        public ParticleSystem particle;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public float crosshairDistFromBarrel;
        private float originalcrosshairDistFromBarrel;
        public float AimCooldown = 4.0f;
        public Vector3 crosshairWorldOffset;
        public float MinGunXRot = 10;
        public float MaxGunXRot = 10;
        public Canvas vehicleUI;
        #endregion

        private Vector3 barrelLookAt;

        private void ClampGunRotation()
        {
            var parent_rotation = GunTransform.parent.rotation;
            var current_rotation = GunTransform.rotation;
            var current_quaternion_x = current_rotation.x;

            var max_euler_rot = Vector3.right * MaxGunXRot;
            var max_quaternion_rot = Quaternion.Euler(max_euler_rot);
            var max_quaternion_x = max_quaternion_rot.x;

            var min_euler_rot = Vector3.right * MinGunXRot;
            var min_quaternion_rot = Quaternion.Euler(min_euler_rot);
            var min_quaternion_x = min_quaternion_rot.x;

            if (current_quaternion_x > max_quaternion_x)
            {
                current_quaternion_x = max_quaternion_x;
            }
            if (current_quaternion_x < min_quaternion_x)
            {
                current_quaternion_x = min_quaternion_x;
            }

            parent_rotation[0] = current_quaternion_x;
            parent_rotation[2] = 0.0f;
            GunTransform.rotation = parent_rotation;
        }

        private void ClampCrosshairUI()
        {
            var rectTrans = vehicleUI.GetComponent<RectTransform>();

            //GET BOUNDS OF THE CANVAS
            var _w = rectTrans.rect.width;
            var _h = rectTrans.rect.height;

            //GET BOUNDS OF CROSSHAIR UI ELEMENT
            var _crosshairW = crosshair.rectTransform.rect.width;
            var _crosshairH = crosshair.rectTransform.rect.height;

            var xLimit = _w;
            var yLimit = _h;

            var clampedX = crosshair.rectTransform.position.x;
            var clampedY = crosshair.rectTransform.position.y;

            var currentPos = crosshair.rectTransform.position;

            if (clampedX < 0)
            {
                clampedX = 0;
            }
            if (clampedY < 0)
            {
                clampedY = 0;
            }

            if (clampedX > xLimit)
            {
                clampedX = xLimit;
            }
            if (clampedY > yLimit)
            {
                clampedY = yLimit;
            }

            var clampedPos = new Vector3(clampedX, clampedY, 0.0f);
            crosshair.rectTransform.position = clampedPos;
        }

        private float aimTimer = 0;
        private void Aim()
        {
            //CREATE A LOOK AT VECTOR FOR THE GUNBARREL
            var cam = Camera.main;
            var mouseY = Input.GetAxis("Mouse Y");
            //var aim_input = (-mouseY * crosshairSpeed);
            var aim_input = (-mouseY);

            var aimVector = new Vector3(aim_input, 0, 0);

            if(Input.GetMouseButton(1))
            {
                //CAMERA SWITCH HERE, NO (WEAPON LERP BACK TO CENTER)
                ToggleAimCamera(true);
            }
            else
            {
                ToggleAimCamera(false);
                //NORMAL CAMERA HERE
                if (aimVector.magnitude <= 0)
                {
                    aimTimer += Time.deltaTime;

                    if (aimTimer >= AimCooldown)
                    {
                        //RE-CENTER THE GUN IF NO INPUT
                        var currentRot = GunTransform.rotation;
                        var currentXRot = currentRot[0];

                        var lerpX = Mathf.Lerp(currentXRot, 0, Time.deltaTime);
                        currentRot[0] = lerpX;
                        GunTransform.rotation = currentRot;
                    }
                }
            }

            if (aimVector.magnitude > 0.0f)
            {
                //aim_input = Mathf.Clamp(aim_input, MinGunXRot, MaxGunXRot);
                GunTransform.Rotate(aimVector);
                aimTimer = 0;
            }

            ClampGunRotation();
            var crosshairLookAt = weapon.GunBarrel.TransformPoint(Vector3.forward * weapon.ShotStrength);
            crosshair.rectTransform.position = cam.WorldToScreenPoint(crosshairLookAt + crosshairWorldOffset);
            ClampCrosshairUI();
        }

        private void ToggleAimCamera(bool toggle)
        {
            if(toggle)
            {
                DrivingCamera.SetActive(false);
                AimCamera.SetActive(true);
            }
            else
            {
                DrivingCamera.SetActive(true);
                AimCamera.SetActive(false);
            }
        }

        private void Start()
        {
            ToggleAimCamera(false);
        }

        private void FixedUpdate()
        {
            if (weapon != null)
            {
                Aim();
                weapon.Fire();
                Debug.DrawRay(weapon.GunBarrel.position, weapon.GunBarrel.forward.normalized * weapon.ShotStrength, Random.ColorHSV());
            }
        }
    }
}
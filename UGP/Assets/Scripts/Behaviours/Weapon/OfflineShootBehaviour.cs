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

        public ParticleSystem particle;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public float AimCooldown = 4.0f;
        public Vector3 crosshairWorldOffset;
        public float MaxGunXRot = 25;
        public float MinGunXRot = 25;
        private Canvas c;
        #endregion

        private Vector3 barrelLookAt;

        //NEEDS WORK
        private void ClampCrosshairUI()
        {
            var rectTrans = c.GetComponent<RectTransform>();

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

        private void ClampGunRotation()
        {
            var rot = GunTransform.rotation.eulerAngles;
            var xRot = rot[0];

            if(xRot > MaxGunXRot)
            {
                xRot = MaxGunXRot;
            }
            if(xRot < MinGunXRot)
            {
                xRot = MinGunXRot;
            }

            rot[0] = xRot;
            GunTransform.rotation = Quaternion.Euler(rot);
        }

        //NEEDS WORK
        private float aimTimer = 0;
        private void Aim()
        {
            //CREATE A LOOK AT VECTOR FOR THE GUNBARREL
            var cam = Camera.main;
            var mouseY = Input.GetAxis("Mouse Y");
            var aim_input = (-mouseY * crosshairSpeed);

            var aimVector = new Vector3(0, aim_input, 0);

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
            else
            {
                //aim_input = Mathf.Clamp(aim_input, MinGunXRot, MaxGunXRot);
                GunTransform.Rotate(Vector3.right * aim_input);
                //ClampGunRotation();
                aimTimer = 0;
            }

            var crosshairLookAt = weapon.GunBarrel.TransformPoint(Vector3.zero);
            crosshair.rectTransform.position = cam.WorldToScreenPoint(crosshairLookAt + crosshairWorldOffset);
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
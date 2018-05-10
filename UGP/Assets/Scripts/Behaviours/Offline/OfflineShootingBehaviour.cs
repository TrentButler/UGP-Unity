using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UGP
{
    public class OfflineShootingBehaviour : MonoBehaviour
    {
        public WeaponType w;

        public GameObject bulletModel;
        public Transform GunBarrel;
        public AudioSource audio;

        public ParticleSystem particle;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public Vector3 crosshairWorldOffset;
        public Canvas c;
        #endregion

        [Range(0.1f, 2.0f)] public float FireRate; //ROUNDS FIRED PER MINUTE
        public float WeaponRange;
        public float AimCooldown;
        public int roundsFired = 0;
        private Vector3 barrelLookAt;

        private void Shoot()
        {
            var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
            Destroy(b, 4);

            //RAYCAST FORWARD FROM 'GunBarrel'
            RaycastHit hit;
            if (Physics.Raycast(GunBarrel.position, GunBarrel.forward, out hit, WeaponRange))
            {
                //DETERMINE IF RAYCAST HIT A VEHICLE OR A PLAYER

                var vehicle = hit.transform.GetComponentInParent<VehicleBehaviour>();
                var player = hit.transform.GetComponentInParent<PlayerBehaviour>();

                if (player)
                {
                    Debug.Log("HIT A PLAYER");
                }

                if (vehicle)
                {
                    Debug.Log("HIT A VEHICLE");
                    //vehicle._v.TakeDamage(ammoBehaviour.DamageDealt);
                }
            }
        }

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

        //NEEDS WORK
        private float aimTimer = 0;
        private void Aim()
        {
            var h = Input.GetAxis("Mouse X");
            var v = Input.GetAxis("Mouse Y");

            var aimVector = new Vector3(h, v, 0);

            var vehicleThrottle = GetComponent<OfflineVehicleMovementBehaviour>().currentVehicleThrottle;
            var vehicleStrafe = GetComponent<OfflineVehicleMovementBehaviour>().currentVehicleStrafe;
            Vector3 moveVector = new Vector3(vehicleStrafe, 0, vehicleThrottle);

            if (moveVector.magnitude <= 0)
            {
                //crosshairWorldOffset.z = 58.0f;
                //crosshairYOffset = 82.0f;

                if (aimVector.magnitude <= 0)
                {
                    aimTimer += Time.deltaTime;

                    if (aimTimer >= AimCooldown)
                    {
                        #region UI_CROSSHAIR

                        var rectTrans = c.GetComponent<RectTransform>();

                        var _w = rectTrans.rect.width;
                        var _h = rectTrans.rect.height;

                        var center = new Vector3((_w / 2) + crosshairXOffset, (_h / 2) + crosshairYOffset, 0);
                        var p = crosshair.rectTransform.position;

                        var lerpX = Mathf.Lerp(p.x, center.x, Time.deltaTime);
                        var lerpY = Mathf.Lerp(p.y, center.y, Time.deltaTime);

                        var lerpPos = new Vector3(lerpX, lerpY, 0);

                        //RETURN CROSSHAIR TO CENTER OVER TIME
                        crosshair.rectTransform.position = lerpPos;
                        #endregion
                    }
                }
                else
                {
                    //MOVE THE UI CROSSHAIR BASED ON MOUSE INPUT
                    crosshair.rectTransform.position += (aimVector * crosshairSpeed);
                    ClampCrosshairUI();

                    aimTimer = 0;
                }
            }
            else
            {
                //CENTER THE CROSSHAIR WHEN VEHICLE IS MOVING
                //crosshairWorldOffset.z = 11.8f;
                //crosshairYOffset = 91.0f;


                var rectTrans = c.GetComponent<RectTransform>();

                var _w = rectTrans.rect.width;
                var _h = rectTrans.rect.height;

                var center = new Vector3((_w / 2) + crosshairXOffset, (_h / 2) + crosshairYOffset, 0);

                crosshair.rectTransform.position = center;
            }

            //CREATE A LOOK AT VECTOR FOR THE GUNBARREL
            var cam = Camera.main;
            var uiCrosshair = crosshair.rectTransform.position;
            barrelLookAt = cam.ScreenToWorldPoint(uiCrosshair + crosshairWorldOffset);
            GunBarrel.LookAt(barrelLookAt);
        }

        private float time = 0;
        private void Fire()
        {
            //NEEDS WORK
            //SINGLE-FIRE
            //if(Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    CmdShoot();
            //}

            //AUTOMATIC FIRE
            //LIMIT THE RATE OF FIRE
            if (Input.GetKey(KeyCode.Mouse0))
            {
                time += Time.deltaTime;
                if (time > FireRate)
                {
                    Shoot();
                    time = 0.0f;
                }
            }
            else
            {
                time = 0.0f;
            }
        }
        
        private void Start()
        {
        }

        private void FixedUpdate()
        {
            Aim();
            Fire();

            Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
        }
    }
}
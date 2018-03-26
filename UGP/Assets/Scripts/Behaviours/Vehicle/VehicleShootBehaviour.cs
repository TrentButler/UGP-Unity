using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public enum Weapon
    {
        ASSAULT = 0,
        SHOTGUN = 1,
        SNIPER = 2,
        ROCKET = 3,
    }

    public class VehicleShootBehaviour : NetworkBehaviour
    {
        public Weapon w;

        public GameObject bulletPrefab;
        public Transform GunBarrel;
        public AudioSource audio;

        public ParticleSystem particle;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public Vector3 crosshairWorldOffset;
        private Canvas c;
        #endregion

        [Range(0.1f, 2.0f)] public float FireRate; //ROUNDS FIRED PER MINUTE
        public float WeaponRange;
        public float AimCooldown;
        public int roundsFired = 0;
        private Vector3 barrelLookAt;

        private VehicleBehaviour v;

        [Command] private void CmdShoot()
        {
            switch (w)
            {
                case Weapon.ASSAULT:
                    {
                        var assault = v._v.ammunition.Assault;
                        if (assault > 0)
                        {
                            v._v.ammunition.Assault -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletPrefab, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
                            NetworkServer.Spawn(b);
                            Destroy(b, 4);
                        }
                        else
                        {
                            Debug.Log("OUT OF ASSAULT ROUNDS");
                        }
                        break;
                    }

                case Weapon.SHOTGUN:
                    {
                        var shotgun = v._v.ammunition.Shotgun;
                        if (shotgun > 0)
                        {
                            v._v.ammunition.Shotgun -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletPrefab, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
                            NetworkServer.Spawn(b);
                            Destroy(b, 4);
                        }
                        else
                        {
                            Debug.Log("OUT OF SHOTGUN ROUNDS");
                        }
                        break;
                    }

                case Weapon.SNIPER:
                    {
                        var sniper = v._v.ammunition.Sniper;
                        if (sniper > 0)
                        {
                            v._v.ammunition.Sniper -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletPrefab, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
                            NetworkServer.Spawn(b);
                            Destroy(b, 4);
                        }
                        else
                        {
                            Debug.Log("OUT OF SNIPER ROUNDS");
                        }
                        break;
                    }

                case Weapon.ROCKET:
                    {
                        var rocket = v._v.ammunition.Rocket;
                        if (rocket > 0)
                        {
                            v._v.ammunition.Rocket -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletPrefab, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
                            NetworkServer.Spawn(b);
                            Destroy(b, 4);
                        }
                        else
                        {
                            Debug.Log("OUT OF ROCKETS");
                        }
                        break;
                    }

                default:
                    {
                        break;
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
            //var h = Input.GetAxis("Mouse X");
            var v = Input.GetAxis("Mouse Y");

            var aimVector = new Vector3(0, v, 0);

            var vehicleThrottle = GetComponent<DefaultVehicleController>().currentVehicleThrottle;
            var vehicleStrafe = GetComponent<DefaultVehicleController>().currentVehicleStrafe;
            Vector3 moveVector = new Vector3(vehicleStrafe, 0, vehicleThrottle);
            
            if(moveVector.magnitude <= 0)
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
                    Debug.Log("SHOT FIRED");
                    CmdShoot();
                    time = 0.0f;
                }
            }
            else
            {
                time = 0.0f;
            }
        }

        private void Awake()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    return;
                }

                return;
            }
        }

        public override void OnStartClient()
        {
            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUI.GetComponent<Canvas>();
            }
        }


        private void Start()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    v = GetComponent<VehicleBehaviour>();
                    var vactive = v.vehicleActive;

                    if (vactive)
                    {
                        c = v.vehicleUI.GetComponent<Canvas>();
                    }
                    return;
                }

                return;
            }

            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUI.GetComponent<Canvas>();
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority && !isServer)
                {
                    //Aim();
                    Fire();

                    //Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
                    return;
                }

                return;
            }

            //Aim();
            Fire();

            //Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
        }

        private void LateUpdate()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    var vactive = v.vehicleActive;
                    if (vactive)
                    {
                        c = v.vehicleUI.GetComponent<Canvas>();
                    }
                    return;
                }

                return;
            }

            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUI.GetComponent<Canvas>();
            }
        }
    }
}
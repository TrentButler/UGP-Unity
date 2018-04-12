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

        [Range(0.1f, 2.0f)] public float AutomaticFireRate; //ROUNDS FIRED PER MINUTE
        [Range(0.1f, 2.0f)] public float SemiAutoFireRate = 0.5f; //ROUNDS FIRED PER MINUTE
        private float automatic_timer = 0;
        private float semiauto_timer = 0;
        private bool hasFired = false;

        [Range(0.0f, 999999.0f)] public float ShotStrength = 500.0f;
        public float WeaponRange;
        public float AimCooldown;
        public int roundsFired = 0;
        private Vector3 barrelLookAt;
        public VehicleBehaviour v;

        [Command] private void CmdFireRound(NetworkIdentity owner, Vector3 position, Quaternion rotation, float strength)
        {
            var b = Instantiate(bulletPrefab, position, rotation);
            var bulletBehaviour = b.GetComponent<DefaultRoundBehaviour>();
            bulletBehaviour.owner = owner;

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.forward.normalized * strength;
            //b_rb.rotation = GunBarrel.rotation;
            b_rb.velocity = force;

            NetworkServer.Spawn(b);
            Destroy(b, 4);
        }

        private void Shoot()
        {
            var networkIdentity = GetComponent<NetworkIdentity>();

            switch (w)
            {
                case Weapon.ASSAULT:
                    {
                        var assault = v.Assault;
                        if (assault > 0)
                        {
                            //v._v.ammunition.Assault -= 1;
                            v.CmdUseAmmunition(1, 0, 0, 0);
                            //audio.Play();
                            CmdFireRound(networkIdentity, GunBarrel.position, GunBarrel.rotation, ShotStrength);
                        }
                        else
                        {
                            Debug.Log("OUT OF ASSAULT ROUNDS");
                        }
                        break;
                    }

                case Weapon.SHOTGUN:
                    {
                        var shotgun = v.Shotgun;
                        if (shotgun > 0)
                        {
                            v.CmdUseAmmunition(0, 1, 0, 0);
                            //audio.Play();
                            CmdFireRound(networkIdentity, GunBarrel.position, GunBarrel.rotation, ShotStrength);
                        }
                        else
                        {
                            Debug.Log("OUT OF SHOTGUN ROUNDS");
                        }
                        break;
                    }

                case Weapon.SNIPER:
                    {
                        var sniper = v.Sniper;
                        if (sniper > 0)
                        {
                            v.CmdUseAmmunition(0, 0, 1, 0);
                            CmdFireRound(networkIdentity, GunBarrel.position, GunBarrel.rotation, ShotStrength);
                            //audio.Play();
                        }
                        else
                        {
                            Debug.Log("OUT OF SNIPER ROUNDS");
                        }
                        break;
                    }

                case Weapon.ROCKET:
                    {
                        var rocket = v.Rocket;
                        if (rocket > 0)
                        {
                            v.CmdUseAmmunition(0, 0, 0, 1);
                            //audio.Play();
                            CmdFireRound(networkIdentity, GunBarrel.position, GunBarrel.rotation, ShotStrength);
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
            Vector3 moveVector = new Vector3(0, 0, vehicleThrottle);
            
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
        
        private void Fire()
        {
            //SINGLE-FIRE
            if (Input.GetMouseButtonDown(0))
            {
                if(!hasFired)
                {
                    Shoot();
                    hasFired = true;
                }
            }
            else
            {
                semiauto_timer += Time.deltaTime;
                if(semiauto_timer >= SemiAutoFireRate)
                {
                    hasFired = false;
                    semiauto_timer = 0.0f; //RESET THE TIMER
                }
            }

            //AUTOMATIC FIRE
            //LIMIT THE RATE OF FIRE
            if (Input.GetMouseButton(0))
            {
                automatic_timer += Time.deltaTime;
                if (automatic_timer > AutomaticFireRate)
                {
                    Debug.Log("SHOT FIRED");
                    Shoot();
                    automatic_timer = 0.0f;
                }
            }
            else
            {
                automatic_timer = 0.0f;
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
                    Aim();
                    Fire();

                    Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
                    return;
                }

                return;
            }

            //Aim();
            //Fire();

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
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

        public GameObject bulletModel;
        public Transform GunBarrel;
        public AudioSource audio;

        #region CROSSHAIR_UI
        public Image crosshair;
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

        private void CmdShoot()
        {
            switch(w)
            {
                case Weapon.ASSAULT:
                    {
                        var assault = v._v.ammunition.Assault;
                        if(assault > 0)
                        {
                            v._v.ammunition.Assault -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
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
                        if(shotgun > 0)
                        {
                            v._v.ammunition.Shotgun -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
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
                        if(sniper > 0)
                        {
                            v._v.ammunition.Sniper -= 1;
                            //audio.Play();

                            var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
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

                            var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
                            b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
                            Destroy(b, 4);
                        }
                        else
                        {
                            Debug.Log("OUT OF ROCKETS");
                        }
                        break;
                    }
            }

            #region OLD_SHOOT
            //roundsFired += 1;
            //audio.Play();

            //var b = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);
            //b.GetComponent<Rigidbody>().velocity = GunBarrel.forward * WeaponRange;
            //Destroy(b, 4);

            //Debug.Log("SHOT FIRED");

            //RAYCAST FORWARD FROM 'GunBarrel'
            //RaycastHit hit;
            //if (Physics.Raycast(GunBarrel.position, GunBarrel.forward, out hit, WeaponRange))
            //{
            //    var n = hit.collider.name;
            //    Debug.Log(n);
            //}

            //Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * BulletTravelDist, Color.red);
            #endregion
        }

        //NEEDS WORK
        private float aimTimer = 0;
        private void Aim()
        {
            var h = Input.GetAxis("Mouse X");
            var v = Input.GetAxis("Mouse Y");

            var aimVector = new Vector3(h, v, 0);

            if(aimVector.magnitude <= 0)
            {
                aimTimer += Time.deltaTime;

                if(aimTimer >= AimCooldown)
                {
                    #region UI_CROSSHAIR

                    var rectTrans = c.GetComponent<RectTransform>();

                    var _w = rectTrans.rect.width;
                    var _h = rectTrans.rect.height;

                    var center = new Vector3(_w / 2, (_h / 2) + crosshairYOffset, 0);
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
                aimTimer = 0;
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
            if(Input.GetKey(KeyCode.Mouse0))
            {
                time += Time.deltaTime;
                if(time > FireRate)
                {
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
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
            
            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUI;
            }
        }
        
        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            Aim();
            Fire();

            Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
        }

        private void LateUpdate()
        {
            var vActive = v.vehicleActive;
            if(vActive)
            {
                c = v.vehicleUI;
            }
        }
    }
}
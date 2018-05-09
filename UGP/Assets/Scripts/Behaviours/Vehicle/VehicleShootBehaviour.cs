using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class VehicleShootBehaviour : NetworkBehaviour
    {
        public List<GameObject> weapons = new List<GameObject>();
        public Weapon weapon;
        public Transform GunTransform;
        public AudioSource audio;

        public ParticleSystem particle;
        private GameObject bullet_prefab;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public Vector3 crosshairWorldOffset;
        private Canvas c;
        #endregion

        private Vector3 barrelLookAt;
        public VehicleBehaviour v;

        public GameObject activeWeapon;
        [SyncVar(hook = "OnSWeaponChange")] public string s_weapon;
        [SyncVar(hook = "OnWeaponActiveChange")] public bool weaponActive;

        private void OnSWeaponChange(string weaponChange)
        {
            s_weapon = weaponChange;
        }
        private void OnWeaponActiveChange(bool activeChange)
        {
            weaponActive = activeChange;
        }

        public void OnSpawn(InGameNetworkBehaviour netCompanion)
        {
            if (weapons.Count > 0)
            {
                var randomWeapon = Random.Range(0, weapons.Count);
                activeWeapon = weapons[randomWeapon];
                s_weapon = activeWeapon.name;
                weapon = activeWeapon.GetComponent<Weapon>();
                bullet_prefab = weapon.bulletPrefab;

                weaponActive = true;
                RpcSetWeaponActive(true);
            }
        }

        [ClientRpc]
        public void RpcSetWeaponActive(bool active)
        {
            weaponActive = active;
            CmdSetWeaponActive(active);
        }
        [Command]
        public void CmdSetWeaponActive(bool active)
        {
            weaponActive = active;
        }

        [Command] public void CmdFireRound(Vector3 position, Quaternion rotation, float strength)
        {
            var b = Instantiate(bullet_prefab, position, rotation);

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.TransformDirection(Vector3.forward) * strength;
            b_rb.velocity = force;

            var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
            net_companion.Spawn(b);
            //NetworkServer.Spawn(b);
            //Destroy(b, 4);
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
            #region OLD
            ////var h = Input.GetAxis("Mouse X");
            //var v = Input.GetAxis("Mouse Y");

            //var aimVector = new Vector3(0, 0, 0);

            //var vehicleThrottle = GetComponent<DefaultVehicleController>().currentVehicleThrottle;
            //var vehicleStrafe = GetComponent<DefaultVehicleController>().currentVehicleStrafe;
            //Vector3 moveVector = new Vector3(0, 0, vehicleThrottle);

            //if(moveVector.magnitude <= 0)
            //{
            //    //crosshairWorldOffset.z = 58.0f;
            //    //crosshairYOffset = 82.0f;

            //    if (aimVector.magnitude <= 0)
            //    {
            //        aimTimer += Time.deltaTime;

            //        if (aimTimer >= AimCooldown)
            //        {
            //            #region UI_CROSSHAIR
            //            var rectTrans = c.GetComponent<RectTransform>();

            //            var _w = rectTrans.rect.width;
            //            var _h = rectTrans.rect.height;

            //            var center = new Vector3((_w / 2) + crosshairXOffset, (_h / 2) + crosshairYOffset, 0);
            //            var p = crosshair.rectTransform.position;

            //            var lerpX = Mathf.Lerp(p.x, center.x, Time.deltaTime);
            //            var lerpY = Mathf.Lerp(p.y, center.y, Time.deltaTime);

            //            var lerpPos = new Vector3(lerpX, lerpY, 0);

            //            //RETURN CROSSHAIR TO CENTER OVER TIME
            //            crosshair.rectTransform.position = lerpPos;
            //            #endregion
            //        }
            //    }
            //    else
            //    {
            //        var rectTrans = c.GetComponent<RectTransform>();

            //        var _w = rectTrans.rect.width;
            //        var _h = rectTrans.rect.height;

            //        var center = new Vector3((_w / 2) + crosshairXOffset, (_h / 2) + crosshairYOffset, 0);

            //        crosshair.rectTransform.position = center;

            //        //MOVE THE UI CROSSHAIR BASED ON MOUSE INPUT
            //        crosshair.rectTransform.position = crosshair.rectTransform.TransformPoint(aimVector * crosshairSpeed);
            //        ClampCrosshairUI();

            //        aimTimer = 0;
            //    }
            //}
            //else
            //{
            //    //CENTER THE CROSSHAIR WHEN VEHICLE IS MOVING
            //    //crosshairWorldOffset.z = 11.8f;
            //    //crosshairYOffset = 91.0f;


            //    var rectTrans = c.GetComponent<RectTransform>();

            //    var _w = rectTrans.rect.width;
            //    var _h = rectTrans.rect.height;

            //    var center = new Vector3((_w / 2) + crosshairXOffset, (_h / 2) + crosshairYOffset, 0);

            //    crosshair.rectTransform.position = center;
            //} 
            #endregion

            //CREATE A LOOK AT VECTOR FOR THE GUNBARREL
            //var cam = Camera.main;
            //var uiCrosshair = crosshair.rectTransform.position;
            //barrelLookAt = cam.ScreenToWorldPoint(uiCrosshair + crosshairWorldOffset);
            ////GunBarrel.LookAt(barrelLookAt);
            //GunTransform.LookAt(barrelLookAt);
            //var rot = GunTransform.rotation;
            //GunTransform.rotation = rot;

            //GunTransform.Rotate()
        }

        public override void OnStartClient()
        {
            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                if (hasAuthority)
                {
                    v = GetComponent<VehicleBehaviour>();
                    var vactive = v.vehicleActive;

                    if (vactive)
                    {
                        c = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
                    }
                    return;
                }

                return;
            }

            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                if (hasAuthority && !isServer)
                {
                    Aim();

                    if (weapon != null)
                    {
                        switch (weapon.type)
                        {
                            case WeaponType.ASSAULT:
                                {
                                    var assault = v.Assault;
                                    if (assault > 0)
                                    {
                                        //v._v.ammunition.Assault -= 1;
                                        
                                        //audio.Play();
                                        if(weapon.Fire(this))
                                        {
                                            v.CmdUseAmmunition(1, 0, 0, 0);
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("OUT OF ASSAULT ROUNDS");
                                    }
                                    break;
                                }

                            case WeaponType.SHOTGUN:
                                {
                                    var shotgun = v.Shotgun;
                                    if (shotgun > 0)
                                    {
                                        if(weapon.Fire(this))
                                        {
                                            v.CmdUseAmmunition(0, 1, 0, 0);
                                        }
                                        //audio.Play();
                                    }
                                    else
                                    {
                                        Debug.Log("OUT OF SHOTGUN ROUNDS");
                                    }
                                    break;
                                }

                            case WeaponType.SNIPER:
                                {
                                    var sniper = v.Sniper;
                                    if (sniper > 0)
                                    {
                                        if(weapon.Fire(this))
                                        {
                                            v.CmdUseAmmunition(0, 0, 1, 0);
                                        }
                                        //audio.Play();
                                    }
                                    else
                                    {
                                        Debug.Log("OUT OF SNIPER ROUNDS");
                                    }
                                    break;
                                }

                            case WeaponType.ROCKET:
                                {
                                    var rocket = v.Rocket;
                                    if (rocket > 0)
                                    {
                                        if(weapon.Fire(this))
                                        {
                                            v.CmdUseAmmunition(0, 0, 0, 1);
                                        }   
                                        //audio.Play();
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

                        Debug.DrawRay(weapon.GunBarrel.position, weapon.GunBarrel.forward.normalized * weapon.ShotStrength, Color.red);
                    }

                    return;
                }

                return;
            }

            //Aim();

            //if (weapon != null)
            //{
            //    weapon.Fire();
            //    Debug.DrawRay(weapon.GunBarrel.position, weapon.GunBarrel.forward.normalized * weapon.WeaponRange, Color.red);
            //}
        }

        private void LateUpdate()
        {
            if(bullet_prefab == null)
            {
                if(activeWeapon == null)
                {
                    weapons.ForEach(w =>
                    {
                        if (w.name == s_weapon)
                        {
                            activeWeapon = w;
                            weapon = w.GetComponent<Weapon>();
                            bullet_prefab = weapon.bulletPrefab;
                        }
                    });
                }
                else
                {
                    bullet_prefab = weapon.bulletPrefab;
                }
            }
            if (activeWeapon == null)
            {
                weapons.ForEach(w =>
                {
                    if (w.name == s_weapon)
                    {
                        activeWeapon = w;
                        weapon = w.GetComponent<Weapon>();
                        bullet_prefab = weapon.bulletPrefab;
                    }
                });
            }

            activeWeapon.SetActive(weaponActive);

            if (!isLocalPlayer)
            {
                if (hasAuthority)
                {
                    var vactive = v.vehicleActive;
                    if (vactive)
                    {
                        c = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
                    }
                    return;
                }

                return;
            }

            var vActive = v.vehicleActive;
            if (vActive)
            {
                c = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
            }
        }
    }
}
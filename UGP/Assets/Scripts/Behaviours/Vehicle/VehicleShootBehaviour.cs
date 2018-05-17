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
        public AudioSource OnWeaponFiredSource;
        
        public GameObject bullet_prefab;

        public GameObject DrivingCamera;
        public GameObject AimCamera;

        #region CROSSHAIR_UI
        public Image crosshair;
        public float crosshairXOffset;
        public float crosshairYOffset;
        public float crosshairSpeed;
        public Vector3 crosshairWorldOffset;
        public float AimCooldown = 4.0f;
        public float MinGunXRot = 10;
        public float MaxGunXRot = 10;
        public Canvas vehicleUI;
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
                if (weapon.type == WeaponType.SNIPER)
                {
                    rail_gun = activeWeapon.GetComponent<RailGun>();
                }
                else
                {
                    OnWeaponFiredSource.clip = weapon.OnVehicleShootSound;
                }
                bullet_prefab = weapon.bulletPrefab;

                weaponActive = true;
                RpcSetWeaponActive(true);
            }
        }

        [ClientRpc] public void RpcSetWeaponActive(bool active)
        {
            weaponActive = active;
            CmdSetWeaponActive(active);
        }
        [Command] public void CmdSetWeaponActive(bool active)
        {
            weaponActive = active;
        }

        [Command] public void CmdFireRound(NetworkIdentity shooter, Vector3 position, Quaternion rotation, float strength)
        {
            var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

            if (weapon.MuzzleFlash != null)
            {
                weapon.MuzzleFlash.Stop();
                weapon.MuzzleFlash.Play();
            }

            if (weapon.CartridgeEject != null)
            {
                net_companion.Spawn(weapon.CartridgeEject, weapon.CartridgeEjectPosition.position, weapon.CartridgeEjectPosition.rotation);
            }

            if (weapon.OnVehicleShootSound != null)
            {
                OnWeaponFiredSource.Stop();
                OnWeaponFiredSource.Play();
            }

            RpcRoundFired();

            var b = Instantiate(bullet_prefab, position, rotation);

            var round_behaviour = b.GetComponent<DefaultRoundBehaviour>();
            round_behaviour.owner = shooter;

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.TransformDirection(Vector3.forward) * strength;
            b_rb.AddForce(force, ForceMode.VelocityChange);

            net_companion.Spawn(b);
        }

        [ClientRpc] private void RpcRoundFired()
        {
            if (weapon.MuzzleFlash != null)
            {
                weapon.MuzzleFlash.Stop();
                weapon.MuzzleFlash.Play();
            }
            if (weapon.OnVehicleShootSound != null)
            {
                OnWeaponFiredSource.Stop();
                OnWeaponFiredSource.Play();
            }
        }

        private bool ClampGunRotation()
        {
            var currentRot = GunTransform.localRotation.eulerAngles;
            var currentX = currentRot[0];

            if (currentX < MinGunXRot)
            {
                return false;
            }
            if (currentX > MaxGunXRot)
            {
                return false;
            }
            return true;
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

            if (Input.GetMouseButton(1))
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

            var crosshairLookAt = weapon.GunBarrel.TransformPoint(Vector3.forward * weapon.ShotStrength);
            crosshair.rectTransform.position = cam.WorldToScreenPoint(crosshairLookAt + crosshairWorldOffset);
            ClampCrosshairUI();
        }

        private void ToggleAimCamera(bool toggle)
        {
            if (toggle)
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

        #region RailGun
        [Header("RAILGUN")]
        [Range(0.0001f, 10.0f)]
        public float ChargeTime = 2.0f;
        [Range(0.0001f, 10.0f)] public float ChargeRate = 1.5f;
        [Range(0.0001f, 10.0f)] public float DischargeRate = 0.5f;
        [Range(0.0001f, 10.0f)] public float MaxRailRotateSpeed = 1.0f;
        [Range(0.0001f, 10.0f)] public float RailRotateAccelerationModifier = 1.0f;
        [SyncVar(hook = "OnShootTimerChange")] public float shot_timer = 0.0f;
        public bool can_shoot = true;
        public bool needs_recharge = false;
        public RailGun rail_gun;

        private void OnShootTimerChange(float timerChange)
        {
            shot_timer = timerChange;
        }
        [Command] private void CmdCurrentShotTimer(float currentTimer)
        {
            shot_timer = currentTimer;
        }
        private void Fire_RailGun()
        {
            if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                if (!needs_recharge)
                {
                    shot_timer += (Time.deltaTime * ChargeRate);
                    if (shot_timer >= ChargeTime)
                    {
                        if (rail_gun == null)
                        {
                            rail_gun = activeWeapon.GetComponent<RailGun>();
                        }

                        rail_gun.Discharge();
                        needs_recharge = true;
                    }
                }
                else
                {
                    shot_timer -= (Time.deltaTime * DischargeRate);
                    shot_timer = Mathf.Clamp(shot_timer, 0, ChargeTime);
                    if (shot_timer <= 0.0f)
                    {
                        shot_timer = 0.0f;
                        needs_recharge = false;
                    }
                }
            }
            if (Input.GetMouseButton(1))
            {
                if (!needs_recharge)
                {
                    shot_timer += (Time.deltaTime * ChargeRate);
                    if (shot_timer >= ChargeTime)
                    {
                        if (rail_gun == null)
                        {
                            rail_gun = activeWeapon.GetComponent<RailGun>();
                        }

                        if(Input.GetMouseButtonDown(0))
                        {
                            rail_gun.Discharge();
                            needs_recharge = true;
                        }
                    }
                }
                else
                {
                    shot_timer -= (Time.deltaTime * DischargeRate);
                    shot_timer = Mathf.Clamp(shot_timer, 0, ChargeTime);
                    if (shot_timer <= 0.0f)
                    {
                        shot_timer = 0.0f;
                        needs_recharge = false;
                    }
                }
            }
            else
            {
                shot_timer -= (Time.deltaTime * DischargeRate);
                shot_timer = Mathf.Clamp(shot_timer, 0, ChargeTime);
                if (needs_recharge)
                {
                    if (shot_timer <= 0.0f)
                    {
                        shot_timer = 0.0f;
                        needs_recharge = false;
                    }
                }
            }

            CmdCurrentShotTimer(shot_timer);
        }
        #endregion

        private void Start()
        {
            ToggleAimCamera(false);
        }

        public override void OnStartClient()
        {
            v = GetComponent<VehicleBehaviour>();
            var vActive = v.vehicleActive;
            if (vActive)
            {
                vehicleUI = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
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
                                        if (weapon.Fire(this))
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
                                        if (weapon.Fire(this))
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
                                    Fire_RailGun();
                                    break;
                                }

                            case WeaponType.ROCKET:
                                {
                                    var rocket = v.Rocket;
                                    if (rocket > 0)
                                    {
                                        if (weapon.Fire(this))
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
            if (bullet_prefab == null)
            {
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
                        if (weapon.type == WeaponType.SNIPER)
                        {
                            rail_gun = activeWeapon.GetComponent<RailGun>();
                        }
                        else
                        {
                            OnWeaponFiredSource.clip = weapon.OnVehicleShootSound;
                        }
                        bullet_prefab = weapon.bulletPrefab;
                    }
                });
            }

            if (rail_gun == null)
            {
                if (weapon.type == WeaponType.SNIPER)
                {
                    rail_gun = activeWeapon.GetComponent<RailGun>();
                }
            }

            activeWeapon.SetActive(weaponActive);

            if (!isLocalPlayer)
            {
                if (hasAuthority)
                {
                    var vactive = v.vehicleActive;
                    if (vactive)
                    {
                        vehicleUI = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
                    }
                    return;
                }

                return;
            }

            var vActive = v.vehicleActive;
            if (vActive)
            {
                vehicleUI = v.vehicleUIBehaviour.vehicleUI.GetComponent<Canvas>();
            }
        }
    }
}
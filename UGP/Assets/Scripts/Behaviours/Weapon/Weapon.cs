using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public enum WeaponType
    {
        ASSAULT = 0,
        SHOTGUN = 1,
        SNIPER = 2,
        ROCKET = 3,
    }

    public abstract class Weapon : NetworkBehaviour
    {
        public WeaponType type;
        public GameObject bulletPrefab;
        public Transform GunBarrel;
        public ParticleSystem MuzzleFlash;
        public Transform CartridgeEjectPosition;
        public GameObject CartridgeEject;
        public AudioSource OnVehicleShootSound;

        [Range(0.1f, 2.0f)] public float AutomaticFireRate; //ROUNDS FIRED PER MINUTE
        [Range(0.1f, 2.0f)] public float SemiAutoFireRate = 0.5f; //ROUNDS FIRED PER MINUTE
        private float automatic_timer = 0;
        private float semiauto_timer = 0;
        private bool hasFired = false;

        [Range(0.0f, 999999.0f)] public float ShotStrength = 500.0f;
        public float AimCooldown;
        public int roundsFired = 0;
        public NetworkAnimator weaponAnimator;

        [Command] protected void CmdFireRound(Vector3 position, Quaternion rotation, float strength)
        {
            var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
                MuzzleFlash.Play();
            }

            if (CartridgeEject != null)
            {
                //var cartridge = Instantiate(CartridgeEject);
                //var cartridgePartice = cartridge.GetComponent<ParticleSystem>();
                //cartridge.transform.position = CartridgeEjectPosition.position;
                //cartridge.transform.rotation = CartridgeEjectPosition.rotation;
                //cartridge.Play();
                net_companion.Spawn(CartridgeEject, CartridgeEjectPosition.position, CartridgeEjectPosition.rotation);
            }

            if (OnVehicleShootSound != null)
            {
                OnVehicleShootSound.Stop();
                OnVehicleShootSound.Play();
            }

            RpcRoundFired();

            var b = Instantiate(bulletPrefab, position, rotation);

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.TransformDirection(Vector3.forward) * strength;
            b_rb.AddForce(force, ForceMode.VelocityChange);
            
            net_companion.Spawn(b);
        }

        [ClientRpc] private void RpcRoundFired()
        {
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
                MuzzleFlash.Play();
            }
            if (OnVehicleShootSound != null)
            {
                OnVehicleShootSound.Stop();
                OnVehicleShootSound.Play();
            }
        }
        public bool Fire(VehicleShootBehaviour shootBehaviour)
        {
            //SINGLE-FIRE
            if (Input.GetMouseButtonDown(0))
            {
                if (!hasFired)
                {
                    Shoot(shootBehaviour);
                    hasFired = true;
                    return true;
                }
            }
            else
            {
                semiauto_timer += Time.deltaTime;
                if (semiauto_timer >= SemiAutoFireRate)
                {
                    hasFired = false;
                    semiauto_timer = 0.0f; //RESET THE TIMER
                    return false;
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
                    Shoot(shootBehaviour);
                    automatic_timer = 0.0f;
                    return true;
                }
            }
            else
            {
                automatic_timer = 0.0f;
                return false;
            }

            return false;
        }
        public abstract void Shoot(VehicleShootBehaviour shootBehaviour);
    }
}
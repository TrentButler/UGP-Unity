using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public abstract class OfflineWeaponBehaviour : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform GunBarrel;
        public ParticleSystem MuzzleFlash;
        public Transform CartridgeEjectPosition;
        public ParticleSystem CartridgeEject;
        //public GameObject HitVehiclePrefab;
        //public GameObject HitPlayerPrefab;

        [Range(0.1f, 2.0f)] public float AutomaticFireRate; //ROUNDS FIRED PER MINUTE
        [Range(0.1f, 2.0f)] public float SemiAutoFireRate = 0.5f; //ROUNDS FIRED PER MINUTE
        private float automatic_timer = 0;
        private float semiauto_timer = 0;
        private bool hasFired = false;

        [Range(0.0f, 999999.0f)] public float ShotStrength = 500.0f;
        public int roundsFired = 0;
        public Animator weaponAnimator;
        
        protected void CmdFireRound(Vector3 position, Quaternion rotation, float strength)
        {
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
                MuzzleFlash.Play();
            }
            
            if (CartridgeEject != null)
            {
                var cartridge = Instantiate(CartridgeEject);
                var cartridgePartice = cartridge.GetComponent<ParticleSystem>();
                cartridge.transform.position = CartridgeEjectPosition.position;
                cartridge.transform.rotation = CartridgeEjectPosition.rotation;
                cartridge.Play();

                Destroy(cartridge, 4);
            }
            
            var b = Instantiate(bulletPrefab, position, rotation);

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.TransformDirection(Vector3.forward) * strength;
            b_rb.AddForce(force, ForceMode.VelocityChange);

            Destroy(b, 20);

            #region RAYCASTATTEMPT
            //RaycastHit hit;

            //var target = position;
            //target.z = position.z + strength;

            //if(Physics.Linecast(position, target, out hit))
            //{
            //    Debug.Log(hit.collider.name);

            //    var impact_point = hit.point;
            //    var impact_particle = Instantiate(HitWallPrefab);
            //    impact_particle.transform.position = impact_point;

            //    var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
            //    particles.ForEach(particle =>
            //    {
            //        particle.Play();
            //    });
            //} 
            #endregion
        }
        public void Fire()
        {
            //SINGLE-FIRE
            if (Input.GetMouseButtonDown(0))
            {
                if (!hasFired)
                {
                    Shoot();
                    hasFired = true;
                }
            }
            else
            {
                semiauto_timer += Time.deltaTime;
                if (semiauto_timer >= SemiAutoFireRate)
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
        public abstract void Shoot();
    }
}
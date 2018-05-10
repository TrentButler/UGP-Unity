using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public abstract class OfflineWeaponBehaviour : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform GunBarrel;

        [Range(0.1f, 2.0f)] public float AutomaticFireRate; //ROUNDS FIRED PER MINUTE
        [Range(0.1f, 2.0f)] public float SemiAutoFireRate = 0.5f; //ROUNDS FIRED PER MINUTE
        private float automatic_timer = 0;
        private float semiauto_timer = 0;
        private bool hasFired = false;

        [Range(0.0f, 999999.0f)] public float ShotStrength = 500.0f;
        public float AimCooldown;
        public int roundsFired = 0;
        public Animator weaponAnimator;
        
        protected void CmdFireRound(Vector3 position, Quaternion rotation, float strength)
        {
            var b = Instantiate(bulletPrefab, position, rotation);

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.TransformDirection(Vector3.forward) * strength;
            b_rb.velocity = force;

            //var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
            //net_companion.Spawn(b);
            //NetworkServer.Spawn(b);
            //Destroy(b, 4);
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
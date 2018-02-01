using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class VehicleShootBehaviour : NetworkBehaviour
    {
        public GameObject bulletModel;
        public Transform GunBarrel;
        public float shotTimer;
        public float shotCooldown;
        public float bulletSpeed;
        public bool hasFired;


        private void Awake()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            shotCooldown = shotTimer;
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
            shotCooldown = shotTimer;
        }

        [Command]
        private void CmdShoot()
        {
            if (shotCooldown <= 0.0f)
                hasFired = false;
                shotCooldown = shotTimer;

            //if(!hasFired)
            //{
            //    var bullet = Instantiate(bulletModel, GunBarrel.forward, GunBarrel.rotation);

            //    var rb = bullet.GetComponent<Rigidbody>();
            //    if (!rb)
            //        rb = bullet.AddComponent<Rigidbody>();
            //    rb.AddForce(bullet.transform.forward * bulletSpeed);

            //    Destroy(bullet, 4.0f);

            //    hasFired = true;
            //}   


            var bullet = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);

            var rb = bullet.GetComponent<Rigidbody>();
            if (!rb)
                rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            //rb.AddForce(GunBarrel.forward * bulletSpeed);
            //rb.AddRelativeForce(GunBarrel.forward * bulletSpeed);
            rb.velocity = bullet.transform.forward * bulletSpeed;

            Destroy(bullet, 4.0f);
        }

        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
                CmdShoot();

            if (hasFired)
                shotCooldown -= Time.deltaTime;

            if (shotCooldown <= 0.0f)
                hasFired = false;
                shotCooldown = shotTimer;
        }
    }
}
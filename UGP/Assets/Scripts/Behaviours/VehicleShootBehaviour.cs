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

        public float WeaponRange;
        public int RoundPerSecond;

        public float minXRot;
        public float maxXRot;

        #region OLD
        //[Command]
        //private void CmdShoot()
        //{
        //    if (shotCooldown <= 0.0f)
        //        hasFired = false;
        //        shotCooldown = shotTimer;

        //    //if(!hasFired)
        //    //{
        //    //    var bullet = Instantiate(bulletModel, GunBarrel.forward, GunBarrel.rotation);

        //    //    var rb = bullet.GetComponent<Rigidbody>();
        //    //    if (!rb)
        //    //        rb = bullet.AddComponent<Rigidbody>();
        //    //    rb.AddForce(bullet.transform.forward * bulletSpeed);

        //    //    Destroy(bullet, 4.0f);

        //    //    hasFired = true;
        //    //}   


        //    var bullet = Instantiate(bulletModel, GunBarrel.position, GunBarrel.rotation);

        //    var rb = bullet.GetComponent<Rigidbody>();
        //    if (!rb)
        //        rb = bullet.AddComponent<Rigidbody>();
        //    rb.useGravity = false;
        //    //rb.AddForce(GunBarrel.forward * bulletSpeed);
        //    //rb.AddRelativeForce(GunBarrel.forward * bulletSpeed);
        //    rb.velocity = bullet.transform.forward * bulletSpeed;

        //    Destroy(bullet, 4.0f);
        //}
        #endregion

        //NEEDS WORK
        private void Aim()
        {
            var a = Input.GetAxis("Mouse Y");
            if(a <= 0.0f)
            {
                var rot = transform.rotation;
                rot[0] = 0.0f;
                transform.rotation = rot;
            }
            GunBarrel.Rotate(new Vector3(a, 0.0f, 0.0f));
        }

        private void CmdShoot(float dt)
        {
            //RAYCAST FORWARD FROM 'GunBarrel'
            RaycastHit hit;
            if (Physics.Raycast(GunBarrel.position, GunBarrel.forward, out hit, WeaponRange))
            {
                var n = hit.collider.name;
                Debug.Log(n);
            }



            //Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * BulletTravelDist, Color.red);
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
        }


        public float time = 0.0f;
        private void FixedUpdate()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                time += Time.deltaTime;
                Debug.Log("LEFT MOUSE BUTTON PRESS");
                CmdShoot(time);
            }
            else
            {
                time = 0.0f;
            }

            Debug.Log(time);


            Aim();

            Debug.DrawRay(GunBarrel.position, GunBarrel.forward.normalized * WeaponRange, Color.red);
        }
    }
}
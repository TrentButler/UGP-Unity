using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class AITurretBehaviour : MonoBehaviour
    {
        public GameObject AmmoPrefab;
        [Range(1, 999999)] public float DamageDealt = 25.0f;
        [Range(0.1f, 999.0f)] public float AutomaticFireRate; //ROUNDS FIRED PER MINUTE
        [Range(1f, 999.0f)] public float AimSpeed; //AIM SPEED
        [Range(1f, 999.0f)] public float BulletPower;

        public Transform DEBUGTarget;

        private float automatic_timer = 0;
        public bool isShooting;

        public Transform Gun;
        public Transform Barrel;

        private void Aim(Transform Target)
        {
            //Vector3 direction = Target.position - Gun.position;
            //Quaternion lookhere = Quaternion.LookRotation(direction);
            //Gun.rotation = Quaternion.Slerp(Gun.rotation, lookhere, AimSpeed * Time.smoothDeltaTime);
            Gun.LookAt(Target);
            Barrel.LookAt(Target);
        }

        private void Fire()
        {
            //AUTOMATIC FIRE
            //LIMIT THE RATE OF FIRE
            if (isShooting)
            {
                automatic_timer += Time.deltaTime;
                if (automatic_timer > AutomaticFireRate)
                {
                    Shoot();
                    automatic_timer = 0.0f;
                }
            }
            else
            {
                automatic_timer = 0.0f;
            }
        }

        private void OfflineFire()
        {
            //AUTOMATIC FIRE
            //LIMIT THE RATE OF FIRE
            if (isShooting)
            {
                automatic_timer += Time.deltaTime;
                if (automatic_timer > AutomaticFireRate)
                {
                    Debug.Log("SHOT FIRED");
                    OfflineShoot();
                    automatic_timer = 0.0f;
                }
            }
            else
            {
                automatic_timer = 0.0f;
            }
        }

        private void Shoot()
        {
            var server = FindObjectOfType<InGameNetworkBehaviour>();

            var b = Instantiate(AmmoPrefab, Gun.position, Gun.rotation);
            var bulletBehaviour = b.GetComponent<DefaultRoundBehaviour>();
            bulletBehaviour.s_owner = gameObject.name;

            var b_rb = b.GetComponent<Rigidbody>();

            var force = b_rb.transform.forward.normalized * BulletPower;
            b_rb.velocity = force;

            server.Spawn(b); //NO WAY TO DESTROY WITH A TIMER
        }

        private void OfflineShoot()
        {
            var b = Instantiate(AmmoPrefab, Gun.position, Gun.rotation);

            var b_rb = b.GetComponent<Rigidbody>();

            var force = Gun.transform.forward.normalized * BulletPower;
            b_rb.velocity = force;
            
            Destroy(b, 4);
        }

        private void FixedUpdate()
        {
            //Aim(DEBUGTarget);
            ////isShooting = true;
            //OfflineFire();
        }

        private void OnTriggerStay(Collider other)
        {
            //AIM AND SHOOT AT THE PLAYER
            if(other.CompareTag("Player"))
            {
                var player_behaviour = other.GetComponentInParent<PlayerBehaviour>();
                if(!player_behaviour.isDead)
                {
                    if(player_behaviour.isDriving)
                    {
                        Aim(other.transform);
                        isShooting = true;
                        //OfflineFire();
                        Fire();
                    }
                    else
                    {
                        Aim(player_behaviour.Center);
                        isShooting = true;
                        //OfflineFire();
                        Fire();
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            isShooting = false;
        }
    } 
}

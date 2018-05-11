using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class DefaultRoundBehaviour : NetworkBehaviour
    {
        [Range(1, 999)] public float DamageDealt;
        [Range(1, 999)] public float PlayerDamageMultiplier = 10.0f;
        public NetworkIdentity owner;
        public string s_owner;
        [Range(1, 999)] public float delete_timer = 6.0f;
        public GameObject BulletHitParticle;

        void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        public void DestroyBullet()
        {
            var server = FindObjectOfType<InGameNetworkBehaviour>();
            server.Server_Destroy(gameObject);
        }

        private void LateUpdate()
        {
            delete_timer -= Time.deltaTime;
            if (delete_timer <= 0.0f)
            {
                DestroyBullet();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer)
            {
                return;
            }

            if (collision.collider.tag == "Vehicle")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                net_companion.SpawnParticle(BulletHitParticle, impact_point.point);

                var vehicle_behaviour = collision.collider.GetComponentInParent<VehicleBehaviour>();
                vehicle_behaviour.RpcTakeDamage(DamageDealt);

                //var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
                //particles.ForEach(particle =>
                //{
                //    particle.Play();
                //});

                net_companion.Server_Destroy(gameObject);
            }

            if (collision.collider.tag == "Player")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                net_companion.SpawnParticle(BulletHitParticle, impact_point.point);

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        player_behaviour.RpcTakeDamage(owner, net_identity, DamageDealt * PlayerDamageMultiplier);
                        net_companion.Server_Destroy(gameObject);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();
                    if (owner != net_identity)
                    {
                        player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt * PlayerDamageMultiplier);
                        net_companion.Server_Destroy(gameObject);
                    }
                    else
                    {
                        return;
                    };
                }

                //var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
                //particles.ForEach(particle =>
                //{
                //    particle.Play();
                //});

                net_companion.Server_Destroy(gameObject);
            }

            if (collision.collider.tag == "Head")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                net_companion.SpawnParticle(BulletHitParticle, impact_point.point);

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        player_behaviour.RpcTakeDamage(owner, net_identity, DamageDealt * 9999);
                        net_companion.Server_Destroy(gameObject);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();
                    if (owner != net_identity)
                    {
                        player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt * 9999);
                        net_companion.Server_Destroy(gameObject);
                    }
                    else
                    {
                        return;
                    };
                }

                //var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
                //particles.ForEach(particle =>
                //{
                //    particle.Play();
                //});

                net_companion.Server_Destroy(gameObject);
            }

            if (collision.collider.tag == "Ammo" || collision.collider.name == "Sphere")
            {
                return;
            }
            else
            {
                var impact_point = collision.contacts[0];
                //var impact_particle = Instantiate(BulletHitParticle);
                //impact_particle.transform.position = impact_point.point;

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                net_companion.SpawnParticle(BulletHitParticle, impact_point.point);

                //var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
                //particles.ForEach(particle =>
                //{
                //    particle.Play();
                //});

                //Destroy(gameObject);
                net_companion.Server_Destroy(gameObject);
            }
        }
    }
}
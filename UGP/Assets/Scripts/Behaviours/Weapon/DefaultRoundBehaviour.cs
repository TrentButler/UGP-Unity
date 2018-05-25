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
        public GameObject BulletHitPlayerParticle;

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

            var first_impact_pos = collision.contacts[0].point;
            var impact_velocity = collision.relativeVelocity;
            var player_rb = collision.collider.GetComponentInParent<Rigidbody>();

            if (collision.collider.tag == "Vehicle")
            {
                var impact_point = collision.contacts[0];
                var vehicle_behaviour = collision.collider.GetComponentInParent<VehicleBehaviour>();
                var vehicle_rb = collision.collider.GetComponentInParent<Rigidbody>();

                if (owner != vehicle_behaviour.owner)
                {
                    if(vehicle_rb != null)
                    {
                        vehicle_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                    }

                    var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
                    net_companion.SpawnParticle(BulletHitParticle, impact_point.point);

                    vehicle_behaviour.RpcTakeDamage(DamageDealt);
                    net_companion.Server_Destroy(gameObject);
                    return;
                }
            }

            if (collision.collider.tag == "Player")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        if(player_rb != null)
                        {
                            player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                        }

                        net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                        player_behaviour.RpcTakeDamage(net_identity, owner, DamageDealt * PlayerDamageMultiplier);
                        net_companion.Server_Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (player_rb != null)
                    {
                        player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                    }

                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                    player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt * PlayerDamageMultiplier);
                    net_companion.Server_Destroy(gameObject);
                    return;
                }
            }

            if (collision.collider.tag == "Head")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        if (player_rb != null)
                        {
                            player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                        }

                        net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                        player_behaviour.RpcTakeDamage(net_identity, owner, DamageDealt * 9999);
                        net_companion.Server_Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (player_rb != null)
                    {
                        player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                    }

                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                    player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt * 9999);
                    net_companion.Server_Destroy(gameObject);
                    return;
                }
            }

            if (collision.collider.tag == "Hand")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        if (player_rb != null)
                        {
                            player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                        }

                        net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                        player_behaviour.RpcTakeDamage(net_identity, owner, DamageDealt);
                        net_companion.Server_Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (player_rb != null)
                    {
                        player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                    }

                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                    player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt);
                    net_companion.Server_Destroy(gameObject);
                    return;
                }
            }

            if (collision.collider.tag == "Foot")
            {
                var impact_point = collision.contacts[0];

                var net_companion = FindObjectOfType<InGameNetworkBehaviour>();

                if (owner != null)
                {
                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    if (owner != net_identity)
                    {
                        if (player_rb != null)
                        {
                            player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                        }

                        net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                        player_behaviour.RpcTakeDamage(net_identity, owner, DamageDealt * PlayerDamageMultiplier);
                        net_companion.Server_Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (player_rb != null)
                    {
                        player_rb.AddForceAtPosition(impact_velocity, first_impact_pos, ForceMode.Impulse);
                    }

                    var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();
                    var net_identity = player_behaviour.GetComponent<NetworkIdentity>();

                    net_companion.SpawnParticle(BulletHitPlayerParticle, impact_point.point);
                    player_behaviour.RpcTakeDamage_Other(net_identity, "NULL", DamageDealt * PlayerDamageMultiplier);
                    net_companion.Server_Destroy(gameObject);
                    return;
                }
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
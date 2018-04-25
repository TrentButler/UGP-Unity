using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class DefaultRoundBehaviour : NetworkBehaviour
    {
        [Range(1, 999)] public float DamageDealt;
        public NetworkIdentity owner;
        public string s_owner;
        [Range(1, 999)] public float delete_timer = 6.0f;

        void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void LateUpdate()
        {
            delete_timer -= Time.deltaTime;
            if(delete_timer <= 0.0f)
            {
                var server = FindObjectOfType<InGameNetworkBehaviour>();
                server.Server_Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!isServer)
            {
                return;
            }

            if (collision.collider.tag == "Vehicle")
            {
                var vehicle_behaviour = collision.gameObject.GetComponentInParent<VehicleBehaviour>();
                vehicle_behaviour.RpcTakeDamage(DamageDealt);
                Destroy(gameObject);
            }

            if (collision.collider.tag == "Player")
            {
                var contact_point = collision.contacts[0].point;
                var player_behaviour = collision.collider.GetComponentInParent<PlayerBehaviour>();

                //var player_rb = collision.collider.GetComponentInParent<Rigidbody>(); // GET THE RAGDOLL RIGIDBODY
                //player_rb.AddForceAtPosition(collision.relativeVelocity * 100, contact_point); //ADD THIS FORCE TO THE RAGDOLL

                //var controller = player_behaviour.GetComponent<CharacterController>();
                //controller.Move(collision.relativeVelocity);

                //if (owner != null)
                //{
                //    var player_networkIdentity = player_behaviour.GetComponent<NetworkIdentity>();
                //    player_behaviour.CmdTakeDamage(owner, DamageDealt * 999999);

                //    var server = FindObjectOfType<InGameNetworkBehaviour>();
                //    server.PlayerShot(owner, player_networkIdentity, "DEBUG WEAPON");
                //}

                var player_networkIdentity = player_behaviour.GetComponent<NetworkIdentity>();
                var controller = player_behaviour.GetComponent<CharacterController>();
                controller.Move(transform.forward.normalized * 1.5f);

                player_behaviour.RpcTakeDamage_Other(player_networkIdentity, s_owner, DamageDealt * 999999);

                Destroy(gameObject);
            }
        }
    }
}
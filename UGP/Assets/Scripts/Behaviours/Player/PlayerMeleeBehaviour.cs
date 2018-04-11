using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class PlayerMeleeBehaviour : NetworkBehaviour
    {
        [SyncVar(hook = "OnDamageChange")] [Range(0, 999999)] public float Damage = 10;
        [Range(0, 99999)] public float KnockBack = 10;
        public Collider RightHand;
        public Collider LeftHand;
        public PlayerBehaviour PlayerBrain;
        public Animator Ani;

        public InGameNetworkBehaviour server;

        public void OnDamageChange(float damageChange)
        {
            Damage = Mathf.Clamp(damageChange, 0, 999999);
        }

        [ClientRpc] public void RpcHitPlayer(NetworkIdentity player, Vector3 force)
        {
            if (player.isLocalPlayer)
            {
                var player_behaviour = player.GetComponent<PlayerBehaviour>();
                GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                player_behaviour.CmdTakeDamage(Damage);
            }
        }
        [Command] public void CmdHitPlayer(NetworkIdentity localPlayer, NetworkIdentity otherPlayer, Vector3 force)
        {
            RpcHitPlayer(otherPlayer, force);
            server.HitPlayer(localPlayer, otherPlayer);
        }


        private void Start()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            PlayerBrain = GetComponent<PlayerBehaviour>();
            Ani = GetComponent<Animator>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            var contact_points = collision.contacts.ToList(); //GATHER ALL POINTS OF CONTACT FROM EVERY COLLIDER ATTACHED TO THIS GAMEOBJECT
            contact_points.ForEach(point =>
            {
                //ITERATE THROUGH THE LIST OF CONTACT POINTS, IF ONE IS THE 'RightHand' OR 'LeftHand'
                //CHECK FOR PLAYER COLLISION AND APPLY DAMAGE
                if (point.thisCollider == RightHand || point.thisCollider == LeftHand)
                {
                    var impact_velocity = collision.relativeVelocity;

                    if (point.otherCollider.CompareTag("Player"))
                    {
                        var other_player_behaviour = collision.gameObject.GetComponentInParent<PlayerBehaviour>();
                        var other_network_identity = collision.gameObject.GetComponentInParent<NetworkIdentity>();
                        var local_network_identity = GetComponent<NetworkIdentity>();

                        if (other_player_behaviour.isLocalPlayer)
                        {
                            return;
                        }
                        
                        if(Ani.GetBool("Fighting"))
                        {
                            Debug.Log(collision.gameObject.name + "@ " + impact_velocity.magnitude.ToString() + " Force");
                            CmdHitPlayer(local_network_identity, other_network_identity, impact_velocity * KnockBack);
                        }
                    }
                }
                
                //if(point.otherCollider.CompareTag("Hand"))
                //{
                //    var player_behaviour = point.otherCollider.GetComponent<PlayerBehaviour>();
                //    if(player_behaviour.isLocalPlayer)
                //    {
                //        return;
                //    }

                //    var force = collision.relativeVelocity;
                //    GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                //}

            });
        }
    }
}
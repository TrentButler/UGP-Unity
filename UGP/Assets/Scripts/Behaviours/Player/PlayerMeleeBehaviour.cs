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

        public void OnDamageChange(float damageChange)
        {
            Damage = Mathf.Clamp(damageChange, 0, 999999);
        }

        [ClientRpc] public void RpcHitPlayer(NetworkIdentity localPlayer, NetworkIdentity otherPlayer, Vector3 force)
        {
            if (otherPlayer.isLocalPlayer)
            {
                var player_behaviour = otherPlayer.GetComponent<PlayerBehaviour>();
                GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                player_behaviour.CmdTakeDamage(localPlayer, Damage);
            }
        }
        [Command] public void CmdHitPlayer(NetworkIdentity localPlayer, NetworkIdentity otherPlayer, Vector3 force)
        {
            //var server = FindObjectOfType<InGameNetworkBehaviour>();
            //server.HitPlayer(localPlayer, otherPlayer);

            RpcHitPlayer(localPlayer, otherPlayer, force);
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

                        if (Ani.GetBool("Fighting"))
                        {
                            Debug.Log(collision.gameObject.name + "@ " + impact_velocity.magnitude.ToString() + " Force");
                            CmdHitPlayer(local_network_identity, other_network_identity, impact_velocity * KnockBack);
                        }
                    }
                }
            });
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (!isLocalPlayer)
        //    {
        //        return;
        //    }

        //    Debug.Log(other.gameObject.name);

        //    //if(other.CompareTag("Player"))
        //    //{
        //    //    var netidentity = other.GetComponent<NetworkIdentity>();
        //    //    if(netidentity.isLocalPlayer)
        //    //    {
        //    //        return;
        //    //    }
        //    //}

        //    if (other.CompareTag("Hand"))
        //    {
        //        var attacker_player_behaviour = other.gameObject.GetComponentInParent<PlayerBehaviour>();
        //        var attacker_network_identity = attacker_player_behaviour.GetComponent<NetworkIdentity>();

        //        var localPlayer_network_identity = GetComponent<NetworkIdentity>();

        //        if (attacker_player_behaviour.isLocalPlayer)
        //        {
        //            return;
        //        }

        //        if (attacker_player_behaviour.ani.GetBool("Fighting"))
        //        {
        //            Debug.Log("GOT HIT BY PLAYER");
        //            CmdHitPlayer(attacker_network_identity, localPlayer_network_identity, Vector3.up * KnockBack);
        //        }
        //    }
        //}

        //private void OnControllerColliderHit(ControllerColliderHit hit)
        //{
        //    //if(!isLocalPlayer)
        //    //{
        //    //    return;
        //    //}

        //    //Debug.Log(hit.gameObject.name);

        //    //if(hit.collider.CompareTag("Player"))
        //    //{
        //    //    Debug.Log("COLLISION WITH PLAYER");
        //    //}

        //    //if (hit.collider.CompareTag("Hand"))
        //    //{
        //    //    var attacker_player_behaviour = hit.collider.gameObject.GetComponentInParent<PlayerBehaviour>();
        //    //    var attacker_network_identity = attacker_player_behaviour.GetComponent<NetworkIdentity>();

        //    //    var localPlayer_network_identity = GetComponent<NetworkIdentity>();

        //    //    if (attacker_player_behaviour.isLocalPlayer)
        //    //    {
        //    //        return;
        //    //    }

        //    //    if (attacker_player_behaviour.ani.GetBool("Fighting"))
        //    //    {
        //    //        Debug.Log("GOT HIT BY PLAYER");
        //    //        CmdHitPlayer(attacker_network_identity, localPlayer_network_identity, Vector3.up * KnockBack);
        //    //    }
        //    //}
        //}

    }
}
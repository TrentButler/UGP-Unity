using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class PlayerMeleeBehaviour : NetworkBehaviour
    {
        [Range(0, 999999)] public int Damage = 10;
        public Collider RightHand;
        public Collider LeftHand;

        private void OnCollisionEnter(Collision collision)
        {
            if(isServer)
            {
                return;
            }

            //var impact_velocity = collision.relativeVelocity.magnitude;
            //Debug.Log(collision.gameObject.name + "@ " + impact_velocity.ToString() + " Force");

            var contact_points = collision.contacts.ToList();
            contact_points.ForEach(point =>
            {
                if(point.thisCollider == RightHand || point.thisCollider == LeftHand)
                {
                    var impact_velocity = collision.relativeVelocity.magnitude;
                    Debug.Log(point.thisCollider.gameObject.name + " COLLIDE WITH " + point.otherCollider.gameObject.name + "@ " + impact_velocity.ToString() + " Force");

                    //var network_identity = GetComponent<NetworkIdentity>(); //CHECK IF THE PLAYER HAS AUTHORITY
                    //var hasAuthority = network_identity.hasAuthority;


                    if (point.otherCollider.CompareTag("Player"))
                    {
                        //ASSIGN AND REMOVE AUTHORITY TO THE 'CLIENT/OTHER' PLAYER OBJECT WHEN HIT
                        var other_player_behaviour = collision.gameObject.GetComponentInParent<PlayerBehaviour>();
                        var other_network_identity = other_player_behaviour.GetComponent<NetworkIdentity>(); 

                        if (other_player_behaviour.isLocalPlayer)
                        {
                            return;
                        }
                        else
                        {
                            //Debug.Log("Hit a Player");
                            Debug.Log(collision.gameObject.name + "@ " + impact_velocity.ToString() + " Force");
                            other_player_behaviour.CmdTakeDamage(Damage);
                        }
                    }
                }

            });

            //if (collision.collider.tag == "Player")
            //{
                
            //}
        }
    }

}
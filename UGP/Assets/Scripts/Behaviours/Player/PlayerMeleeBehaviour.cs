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

                    if (point.otherCollider.CompareTag("Player"))
                    {
                        var player_behaviour = collision.gameObject.GetComponentInParent<PlayerBehaviour>();

                        if (player_behaviour.isLocalPlayer)
                        {
                            return;
                        }
                        else
                        {
                            //Debug.Log("Hit a Player");
                            Debug.Log(collision.gameObject.name + "@ " + impact_velocity.ToString() + " Force");
                            player_behaviour.CmdTakeDamage(Damage);
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
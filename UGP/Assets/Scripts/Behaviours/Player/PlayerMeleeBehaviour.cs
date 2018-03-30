using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class PlayerMeleeBehaviour : MonoBehaviour
    {
        [Range(0, 999999)] public int Damage = 10;

        private void OnCollisionEnter(Collision collision)
        {
            var impact_velocity = collision.relativeVelocity.magnitude;
            Debug.Log(collision.gameObject.name + "@ " + impact_velocity.ToString() + " Force");

            if (collision.collider.tag == "Player")
            {
                var player_behaviour = collision.gameObject.GetComponent<PlayerBehaviour>();
                if(player_behaviour.isLocalPlayer)
                {
                    return;
                }
                else
                {
                    Debug.Log("Hit a Player");
                    player_behaviour.CmdTakeDamage(Damage);
                }
            }
        }
    }

}
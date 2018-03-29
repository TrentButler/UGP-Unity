using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class PlayerMeleeBehaviour : MonoBehaviour
    {
        public int Damage = 10;

        private void OnCollisionEnter(Collision collision)
        {
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
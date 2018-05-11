using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class StormBehaviour : NetworkBehaviour
    {
        [Range(0.0001f, 999999)] public float Damage;
        [Range(0.0001f, 60)] public float TimeTakenToDamage;
        private float damage_timer = 0.0f;

        private void OnTriggerStay(Collider other)
        {
            if(!isServer)
            {
                return;
            }

            var playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if(playerBehaviour != null)
            {
                if(!playerBehaviour.isDead)
                {
                    if(playerBehaviour.isDriving)
                    {
                        return;
                    }

                    var player_identity = playerBehaviour.GetComponent<NetworkIdentity>();
                    damage_timer += Time.deltaTime;
                    if (damage_timer > TimeTakenToDamage)
                    {
                        playerBehaviour.RpcTakeDamage_Other(player_identity, "STORM", Damage);
                        damage_timer = 0.0f;
                    }
                }
            }

            var vehicleBehaviour = other.GetComponentInParent<VehicleBehaviour>();
            if (vehicleBehaviour != null)
            {
                if(!vehicleBehaviour.isDestroyed)
                {
                    damage_timer += Time.deltaTime;
                    if (damage_timer > TimeTakenToDamage)
                    {
                        vehicleBehaviour.RpcTakeDamage(Damage);
                        damage_timer = 0.0f;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            damage_timer = 0.0f;
        }
    }
}
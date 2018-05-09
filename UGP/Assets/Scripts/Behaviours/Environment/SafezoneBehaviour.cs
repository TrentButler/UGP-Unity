using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class SafezoneBehaviour : MonoBehaviour
    {
        [Range(0.0001f, 999999)] public float HealthRegen;
        [Range(0.0001f, 60)] public float TimeTakenToRegen;
        private float regen_timer = 0.0f;

        private void OnTriggerStay(Collider other)
        {
            var playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                if(playerBehaviour.isServer)
                {
                    if (!playerBehaviour.isDead)
                    {
                        if (playerBehaviour.isDriving)
                        {
                            return;
                        }

                        var player_identity = playerBehaviour.GetComponent<NetworkIdentity>();
                        regen_timer += Time.deltaTime;
                        if (regen_timer > TimeTakenToRegen)
                        {
                            playerBehaviour.RpcTakeHealth(HealthRegen);
                            regen_timer = 0.0f;
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var playerBehaviour = other.GetComponent<PlayerBehaviour>();
            if(playerBehaviour != null)
            {
                if(playerBehaviour.isServer)
                {
                    regen_timer = 0.0f;
                }
            }
        }
    }
}
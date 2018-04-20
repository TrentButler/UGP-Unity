using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class RagdollBehaviour : MonoBehaviour
    {
        [Range(1, 999)] public float delete_timer = 10.0f;

        private void LateUpdate()
        {
            delete_timer -= Time.deltaTime;
            if (delete_timer <= 0.0f)
            {
                var server = FindObjectOfType<InGameNetworkBehaviour>();
                server.Server_Destroy(gameObject);
            }
        }
    }

}
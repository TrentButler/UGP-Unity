using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBulletHitObjectBehaviour : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ammo")
        {
            var mesh_renderer = GetComponent<MeshRenderer>();
            mesh_renderer.material.color = Random.ColorHSV();
            Debug.Log("CHANGING COLOR");
        }
    }
}

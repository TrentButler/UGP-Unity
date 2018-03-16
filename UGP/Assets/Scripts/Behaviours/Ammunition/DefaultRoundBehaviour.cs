using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class DefaultRoundBehaviour : MonoBehaviour
    {
        [Range(1, 999)] public float DamageDealt;

        void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Vehicle")
            {
                var vehicle_scriptable = collision.collider.GetComponentInParent<VehicleBehaviour>()._v;
                vehicle_scriptable.TakeDamage(DamageDealt);
                Destroy(gameObject);
            }
        }
    }
}
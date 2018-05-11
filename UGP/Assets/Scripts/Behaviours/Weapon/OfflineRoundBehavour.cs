using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineRoundBehavour : MonoBehaviour
    {
        [Range(1, 999)] public float delete_timer = 6.0f;
        public GameObject BulletHitParticle;

        void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        public void DestroyBullet()
        {
            Destroy(gameObject);
        }

        private void LateUpdate()
        {
            delete_timer -= Time.deltaTime;
            if (delete_timer <= 0.0f)
            {
                DestroyBullet();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Ammo" || collision.collider.name == "Sphere")
            {
                return;
            }
            else
            {
                var impact_point = collision.contacts[0];
                //var impact_point = collision.collider.ClosestPoint(transform.position);
                var impact_particle = Instantiate(BulletHitParticle);
                impact_particle.transform.position = impact_point.point;
                //impact_particle.transform.position = impact_point;

                var particles = impact_particle.GetComponents<ParticleSystem>().ToList();
                particles.ForEach(particle =>
                {
                    particle.Play();
                });

                Destroy(gameObject);
            }
        }
    } 
}
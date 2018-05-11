using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineVehicleParticleSystemBehaviour : MonoBehaviour
    {
        public List<ParticleSystem> Thrusters = new List<ParticleSystem>();
        public ParticleSystem DustTrail;
        private float originalThrusterStartSize;
        private float originalThrusterLifetime;
        private Color originalThrusterStartColor;
        private float originalDustStartSize;
        private float originalDustLifetime;
        private Color originalDustStartColor;

        public OfflineUserControl ic;
        void Start()
        {
            if (ic == null)
            {
                ic = GetComponent<OfflineUserControl>();
            }

            originalThrusterLifetime = Thrusters[0].startLifetime;
            originalThrusterStartSize = Thrusters[0].startSize;
            originalThrusterStartColor = Thrusters[0].startColor;

            originalDustLifetime = DustTrail.startLifetime;
            originalDustStartSize = DustTrail.startSize;
            originalDustStartColor = DustTrail.startColor;
        }
        
        void FixedUpdate()
        {
            Thrusters.ForEach(thruster =>
            {
                thruster.startLifetime = Mathf.Lerp(0.0f, originalThrusterLifetime, ic.GetCurrentForwardInput());
                thruster.startSize = Mathf.Lerp(originalThrusterStartSize * .3f, originalThrusterStartSize, ic.GetCurrentForwardInput());
                thruster.startColor = Color.Lerp(Vector4.zero, originalThrusterStartColor, ic.GetCurrentForwardInput());
            });

            DustTrail.startLifetime = Mathf.Lerp(0.0f, originalDustLifetime, ic.GetCurrentForwardInput());
            DustTrail.startSize = Mathf.Lerp(originalDustStartSize * .3f, originalDustStartSize, ic.GetCurrentForwardInput());
            DustTrail.startColor = Color.Lerp(Vector4.zero, originalDustStartColor, ic.GetCurrentForwardInput());
        }
    }

}
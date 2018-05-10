using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class VehicleParticleSystemBehaviour : NetworkBehaviour
    {
        public List<ParticleSystem> Thrusters = new List<ParticleSystem>();
        public ParticleSystem DustTrail;
        private float originalThrusterStartSize;
        private float originalThrusterLifetime;
        private Color originalThrusterStartColor;
        private float originalDustStartSize;
        private float originalDustLifetime;
        private Color originalDustStartColor;

        public NetworkUserControl ic;

        [SyncVar(hook = "OnCurrentVerticalThrottleChange")] public float current_vertical_throttle = 0.0f;
        [SyncVar(hook = "OnCurrentHorizontalThrottleChange")] public float current_horizontal_throttle = 0.0f;
        private void OnCurrentVerticalThrottleChange(float throttleChange)
        {
            current_vertical_throttle = throttleChange;
        }
        private void OnCurrentHorizontalThrottleChange(float throttleChange)
        {
            current_horizontal_throttle = throttleChange;
        }

        [Command] public void CmdCurrentVerticalThrottle(float currentThrottle)
        {
            current_vertical_throttle = currentThrottle;
        }
        [Command] public void CmdCurrentHorizontalThrottle(float currentThrottle)
        {
            current_horizontal_throttle = currentThrottle;
        }
        
        private void Start()
        {
            if(ic == null)
            {
                ic = GetComponent<NetworkUserControl>();
            }

            originalThrusterLifetime = Thrusters[0].startLifetime;
            originalThrusterStartSize = Thrusters[0].startSize;
            originalThrusterStartColor = Thrusters[0].startColor;

            originalDustLifetime = DustTrail.startLifetime;
            originalDustStartSize = DustTrail.startSize;
            originalDustStartColor = DustTrail.startColor;
        }
        
        private void FixedUpdate()
        {
            if (hasAuthority && !isServer)
            {
                current_vertical_throttle = ic.GetCurrentVerticalInput();
                current_horizontal_throttle = Mathf.Abs(ic.GetCurrentHorizontalInput());
                CmdCurrentVerticalThrottle(ic.GetCurrentVerticalInput());
                CmdCurrentHorizontalThrottle(Mathf.Abs(ic.GetCurrentHorizontalInput()));
            }
        }

        private void LateUpdate()
        {
            Thrusters.ForEach(thruster =>
            {
                thruster.startLifetime = Mathf.Lerp(0.0f, originalThrusterLifetime, current_vertical_throttle);
                thruster.startSize = Mathf.Lerp(originalThrusterStartSize * .3f, originalThrusterStartSize, current_vertical_throttle);
                thruster.startColor = Color.Lerp(Vector4.zero, originalThrusterStartColor, current_vertical_throttle);
            });

            DustTrail.startLifetime = Mathf.Lerp(0.0f, originalDustLifetime, current_vertical_throttle + current_horizontal_throttle);
            DustTrail.startSize = Mathf.Lerp(originalDustStartSize * .3f, originalDustStartSize, current_vertical_throttle + current_horizontal_throttle);
            DustTrail.startColor = Color.Lerp(Vector4.zero, originalDustStartColor, current_vertical_throttle + current_horizontal_throttle);
        }
    }

}
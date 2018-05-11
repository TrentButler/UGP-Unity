using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class RailGun : Weapon
    {
        [Range(0.0001f, 10.0f)] public float ChargeTime = 2.0f;
        [Range(0.0001f, 10.0f)] public float ChargeRate = 1.5f;
        [Range(0.0001f, 10.0f)] public float DischargeRate = 0.5f;
        [Range(0.0001f, 10.0f)] public float MaxRailRotateSpeed = 1.0f;
        [Range(0.0001f, 10.0f)] public float RailRotateAccelerationModifier = 1.0f;

        #region ParticleSystem
        public List<ParticleSystem> ChargeParticles = new List<ParticleSystem>();
        public ParticleSystem ChargeParticle;
        private float originalChargeParticleStartSize;
        private float originalChargeParticleLifetime;
        private Color originalChargeParticleStartColor;
        private float originalChargeParticlesStartSize;
        private float originalChargeParticlesLifetime;
        private Color originalChargeParticlesStartColor;
        #endregion

        public Transform RailTransform;
        OfflineWeaponBehaviour shootBehaviour;
        public NetworkUserControl ic;

        [SyncVar(hook = "OnShootTimerChange")] public float shot_timer = 0.0f;
        public bool can_shoot = true;
        public bool needs_recharge = false;

        private void OnShootTimerChange(float timerChange)
        {
            shot_timer = timerChange;
        }
        
        private void Discharge()
        {
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
                MuzzleFlash.Play();
            }

            CmdFireRound(GunBarrel.position, GunBarrel.rotation, ShotStrength);
        }

        private void Start()
        {
            originalChargeParticleLifetime = ChargeParticle.startLifetime;
            originalChargeParticleStartSize = ChargeParticle.startSize;
            originalChargeParticleStartColor = ChargeParticle.startColor;

            originalChargeParticlesLifetime = ChargeParticles[0].startLifetime;
            originalChargeParticlesStartSize = ChargeParticles[0].startSize;
            originalChargeParticlesStartColor = ChargeParticles[0].startColor;
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                if(!needs_recharge)
                {
                    shot_timer += (Time.deltaTime * ChargeRate);
                    if (shot_timer >= ChargeTime)
                    {
                        Discharge();
                        needs_recharge = true;
                    }
                }
                else
                {
                    shot_timer -= (Time.deltaTime * DischargeRate);
                    shot_timer = Mathf.Clamp(shot_timer, 0, ChargeTime);
                    if (shot_timer <= 0.0f)
                    {
                        shot_timer = 0.0f;
                        needs_recharge = false;
                    }
                }
            }
            else
            {
                shot_timer -= (Time.deltaTime * DischargeRate);
                shot_timer = Mathf.Clamp(shot_timer, 0, ChargeTime);
                if(needs_recharge)
                {
                    if(shot_timer <= 0.0f)
                    {
                        shot_timer = 0.0f;
                        needs_recharge = false;
                    }
                }
            }

            var rail_rotate_vector = Vector3.forward * ((shot_timer * MaxRailRotateSpeed) * RailRotateAccelerationModifier);
            RailTransform.Rotate(-rail_rotate_vector);

            ChargeParticles.ForEach(particle =>
            {
                particle.startLifetime = Mathf.Lerp(0.0f, originalChargeParticlesLifetime, shot_timer);
                particle.startSize = Mathf.Lerp(originalChargeParticlesStartSize * .3f, originalChargeParticleStartSize, shot_timer);
                particle.startColor = Color.Lerp(Vector4.zero, originalChargeParticlesStartColor, shot_timer);
            });

            ChargeParticle.startLifetime = Mathf.Lerp(0.0f, originalChargeParticleLifetime, shot_timer);
            ChargeParticle.startSize = Mathf.Lerp(originalChargeParticleStartSize * .3f, originalChargeParticleStartSize, shot_timer);
            ChargeParticle.startColor = Color.Lerp(Vector4.zero, originalChargeParticleStartColor, shot_timer);

        }

        public override void Shoot(VehicleShootBehaviour shootBehaviour)
        {
            return;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class RailGun : Weapon
    {
        //[Range(0.0001f, 10.0f)] public float ChargeTime = 2.0f;
        //[Range(0.0001f, 10.0f)] public float ChargeRate = 1.5f;
        //[Range(0.0001f, 10.0f)] public float DischargeRate = 0.5f;
        //[Range(0.0001f, 10.0f)] public float MaxRailRotateSpeed = 1.0f;
        //[Range(0.0001f, 10.0f)] public float RailRotateAccelerationModifier = 1.0f;

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
        public VehicleBehaviour vehicleBehaviour;
        public VehicleShootBehaviour vehicleShoot;
        
        public void Discharge()
        {
            var sniper = vehicleBehaviour.Sniper;
            if (sniper > 0)
            {
                //if (MuzzleFlash != null)
                //{
                //    MuzzleFlash.Stop();
                //    MuzzleFlash.Play();
                //}

                vehicleBehaviour.CmdUseAmmunition(0, 0, 1, 0);
                vehicleShoot.CmdFireRound(vehicleBehaviour.owner, GunBarrel.position, GunBarrel.rotation, ShotStrength);
            }
            else
            {
                vehicleShoot.needs_recharge = true;
                Debug.Log("OUT OF SNIPER ROUNDS");
            }
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

        private void LateUpdate()
        {
            if(!vehicleBehaviour.vehicleActive)
            {
                ChargeParticles.ForEach(particle =>
                {
                    if (particle.isPlaying)
                    {
                        particle.Stop();
                    }
                });

                if (ChargeParticle.isPlaying)
                {
                    ChargeParticle.Stop();
                }
            }
            else
            {
                var rail_rotate_vector = Vector3.forward * ((vehicleShoot.shot_timer * vehicleShoot.MaxRailRotateSpeed) * vehicleShoot.RailRotateAccelerationModifier);
                RailTransform.Rotate(-rail_rotate_vector);

                ChargeParticles.ForEach(particle =>
                {
                    if (particle.isStopped)
                    {
                        particle.Play();
                    }   

                    particle.startLifetime = Mathf.Lerp(0.0f, originalChargeParticlesLifetime, vehicleShoot.shot_timer);
                    particle.startSize = Mathf.Lerp(originalChargeParticlesStartSize * .3f, originalChargeParticleStartSize, vehicleShoot.shot_timer);
                    particle.startColor = Color.Lerp(Vector4.zero, originalChargeParticlesStartColor, vehicleShoot.shot_timer);
                });

                if(ChargeParticle.isStopped)
                {
                    ChargeParticle.Play();
                }
                ChargeParticle.startLifetime = Mathf.Lerp(0.0f, originalChargeParticleLifetime, vehicleShoot.shot_timer);
                ChargeParticle.startSize = Mathf.Lerp(originalChargeParticleStartSize * .3f, originalChargeParticleStartSize, vehicleShoot.shot_timer);
                ChargeParticle.startColor = Color.Lerp(Vector4.zero, originalChargeParticleStartColor, vehicleShoot.shot_timer);
            }
        }

        public override void Shoot(VehicleShootBehaviour shootBehaviour)
        {
            return;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class Shotgun : Weapon
    {
        public int PelletCount = 6;
        [Range(0.001f, 100.0f)] public float Spread = 10.0f;

        public override void Shoot(VehicleShootBehaviour shootBehaviour)
        {
            for (int i = 0; i < PelletCount; i++)
            {
                var vehicle_behaviour = shootBehaviour.GetComponent<VehicleBehaviour>();
                var vehicle_rb = vehicle_behaviour.GetComponent<Rigidbody>();
                var point = new Vector3(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread), 0);

                point += new Vector3(0, 0, Mathf.Abs(vehicle_rb.velocity.z));

                var transformed_point = GunBarrel.TransformPoint(point);

                shootBehaviour.CmdFireRound(vehicle_behaviour.owner, transformed_point, GunBarrel.rotation, ShotStrength);
                //weaponAnimator.SetTrigger("Fire");
            }
        }
    }
}
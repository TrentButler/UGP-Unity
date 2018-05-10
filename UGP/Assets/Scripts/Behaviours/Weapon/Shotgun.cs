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
                var point = new Vector2(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread));
                var transformed_point = GunBarrel.TransformPoint(point);

                shootBehaviour.CmdFireRound(transformed_point, GunBarrel.rotation, ShotStrength);
                //weaponAnimator.SetTrigger("Fire");
            }
        }
    }
}
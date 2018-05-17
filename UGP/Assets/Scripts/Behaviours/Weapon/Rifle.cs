﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class Rifle : Weapon
    {
        [Range(0.001f, 100.0f)] public float Spread = 0;

        public override void Shoot(VehicleShootBehaviour shootBehaviour)
        {
            var vehicle_behaviour = shootBehaviour.GetComponent<VehicleBehaviour>();
            var randomOffset = new Vector3(Random.Range(-Spread, Spread), Random.Range(0, Spread), 0);
            var point = GunBarrel.TransformPoint(randomOffset);
            shootBehaviour.CmdFireRound(vehicle_behaviour.owner, point, GunBarrel.rotation, ShotStrength);
        }
    }
}
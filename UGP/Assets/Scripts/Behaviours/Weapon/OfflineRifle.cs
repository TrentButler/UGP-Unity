using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineRifle : OfflineWeaponBehaviour
    {
        [Range(0.001f, 100.0f)] public float Spread = 0;

        public override void Shoot()
        {
            var randomOffset = new Vector3(Random.Range(-Spread, Spread), Random.Range(0, Spread), 0);
            var point = GunBarrel.position + randomOffset;
            CmdFireRound(point, GunBarrel.rotation, ShotStrength);
        }
    }
}
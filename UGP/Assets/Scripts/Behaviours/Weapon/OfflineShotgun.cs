using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineShotgun : OfflineWeaponBehaviour
    {
        public int PelletCount = 6;
        [Range(0.001f, 100.0f)] public float Spread = 10.0f;
        private Vector3 previousPos = Vector3.zero;

        public override void Shoot()
        {
            for (int i = 0; i < PelletCount; i++)
            {
                var point = new Vector3(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread), 0);
                var dist = Vector3.Distance(previousPos, point);                

                CmdFireRound(GunBarrel.TransformPoint(point), GunBarrel.rotation, ShotStrength);
            }
        }
    }
}
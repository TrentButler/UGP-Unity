using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineShotgun : OfflineWeaponBehaviour
    {
        public int PelletCount = 6;
        [Range(0.001f, 100.0f)] public float Spread = 10.0f;

        public override void Shoot()
        {
            for (int i = 0; i < PelletCount; i++)
            {
                var point = new Vector2(Random.Range(-Spread, Spread), Random.Range(-Spread, Spread));

                CmdFireRound(GunBarrel.TransformPoint(point), GunBarrel.rotation, ShotStrength);
                //weaponAnimator.SetTrigger("Fire");
            }
        }
    }
}
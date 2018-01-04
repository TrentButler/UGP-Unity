using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Trent
{
    [CreateAssetMenu(fileName = "AmmoBox", menuName = "AmmoBox", order = 3)]
    public class AmmoBox : Item, ICollectable
    {
        //HOLDS ALL AMMO TYPES
        //ASSAULT
        //SHOTGUN
        //SNIPER
        //ROCKET

        public AssaultRound assault;
        public ShotgunRound shotgun;
        public SniperRound sniper;
        public RocketRound rocket;

        private void OnEnable()
        {
            assault = CreateInstance<AssaultRound>() as AssaultRound;
            shotgun = CreateInstance<ShotgunRound>() as ShotgunRound;
            sniper = CreateInstance<SniperRound>() as SniperRound;
            rocket = CreateInstance<RocketRound>() as RocketRound;
        }

        public Item ItemGivenOnPickup(ICollector c)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return new List<Item>() { assault, shotgun, sniper, rocket };
        }
    }
}
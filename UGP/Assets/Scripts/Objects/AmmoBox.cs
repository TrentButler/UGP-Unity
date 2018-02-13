using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    //NEEDS WORK, 
    //FUNCTION(S) FOR HANDLING WHEN PLAYER SHOOTS A GIVEN AMMOTYPE
    //FUNCTION(S) FOR HANDLING WHEN PLAYER PICKSUP ANOTHER AMMOBOX
    //FUNCTION(S) FOR HANDLING WHEN PLAYER DROPS A GIVEN AMMOTYPE

    [CreateAssetMenu(fileName = "AmmoBox", menuName = "AmmoBox", order = 3)]
    public class AmmoBox : Item, ICollectable
    {
        //HOLDS ALL AMMO TYPES
        //ASSAULT
        //SHOTGUN
        //SNIPER
        //ROCKET

        [Range(0, 999)] public int Assault;
        [Range(0, 999)] public int Shotgun;
        [Range(0, 999)] public int Sniper;
        [Range(0, 999)] public int Rocket;

        public Item ItemGivenOnPickup(ICollector c)
        {
            return this as AmmoBox;
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return null;
        }
    }
}
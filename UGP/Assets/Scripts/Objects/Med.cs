using System.Collections.Generic;
using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "Med", menuName = "Med", order = 0)]
    public class Med : Item, IConsumable, ICollectable
    {
        public float HealFactor;
        public int ItemCount;

        public Item ItemGivenOnPickup(ICollector c)
        {
            return this; //RETURN THE MED OBJECT
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return null; //USE 'ItemGivenOnPickup'
        }

        public void OnUse(IHealable h)
        {
            h.TakeHealth(HealFactor);
        }
    }
}
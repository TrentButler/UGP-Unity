using System.Collections.Generic;
using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "Hammer", menuName = "Hammer", order = 1)]
    public class Hammer : Item, IDamager, IRepair, ICollectable
    {
        public float DamageFactor;
        public float RepairFactor;

        public void DealRepair(float repairDealt, IRepairable r)
        {
            r.TakeRepair(RepairFactor);
        }

        public void DealDamage(float damageDealt, IDamageable d)
        {
            d.TakeDamage(DamageFactor);
        }

        public Item ItemGivenOnPickup(ICollector c)
        {
            return this; //RETURN THE HAMMER OBJECT
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return null; //USE 'ItemGivenOnPickup'
        }
    }
}
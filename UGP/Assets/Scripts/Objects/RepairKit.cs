using System.Collections.Generic;
using UnityEngine;
namespace UGP
{
    [CreateAssetMenu(fileName = "RepairKit", menuName = "RepairKit", order = 2)]
    public class RepairKit : Item, IRepair, ICollectable
    {
        public float RepairFactor;
        public int ItemCount;

        public void DealRepair(float repairDealt, IRepairable r)
        {
            r.TakeRepair(RepairFactor);
        }

        public Item ItemGivenOnPickup(ICollector c)
        {
            return this; //RETURN THE REPAIR KIT OBJECT
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return null; //USE 'ItemGivenOnPickup'
        }
    }
}
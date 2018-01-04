using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "RepairKit", menuName = "RepairKit", order = 2)]
    public class RepairKit : ScriptableObject, IRepair
    {
        public float RepairFactor;
        public int StackAmmount;

        public void DealRepair(float repairDealt, IRepairable r)
        {
            r.TakeRepair(RepairFactor);
        }
    }
}
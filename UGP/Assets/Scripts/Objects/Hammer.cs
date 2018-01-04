using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "Hammer", menuName = "Hammer", order = 1)]
    public class Hammer : ScriptableObject, IDamager, IRepair
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
    }
}
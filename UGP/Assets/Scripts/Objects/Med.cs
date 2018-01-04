using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "Med", menuName = "Med", order = 0)]
    public class Med : ScriptableObject, IConsumable
    {
        public float HealFactor;
        public int StackAmmount;
        
        public void OnUse(IHealable h)
        {
            h.TakeHealth(HealFactor);
        }
    }
}
using UnityEngine;
namespace Trent
{
    [CreateAssetMenu(fileName = "SniperRound", menuName = "AmmoType/Sniper", order = 2)]
    public class SniperRound : Ammo, IShootable
    {
    }
}
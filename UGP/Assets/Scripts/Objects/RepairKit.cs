using System.Collections.Generic;
using UnityEngine;
namespace UGP
{
    [CreateAssetMenu(fileName = "RepairKit", menuName = "RepairKit", order = 2)]
    public class RepairKit : Item
    {
        [Range(0, 999999)] public float RepairFactor;
    }
}
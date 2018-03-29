using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    [CreateAssetMenu(fileName = "Fuel", menuName = "Fuel", order = 5)]
    public class Fuel : Item
    {
        [Range(0, 999999)] public float RefuelFactor;
    }
}
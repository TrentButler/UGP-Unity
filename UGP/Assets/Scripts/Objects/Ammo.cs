using UnityEngine;

namespace UGP
{
    public abstract class Ammo : Item
    {
        [Range(0, 999)] public int Count;
    }
}
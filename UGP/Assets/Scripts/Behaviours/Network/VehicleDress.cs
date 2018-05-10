using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{

    public class VehicleDress : MonoBehaviour
    {
        #region Colors
        public Color Part0Color;
        public Color Part1Color;
        public Color Part2Color;
        public Color Part3Color;
        public Color Part4Color;
        public Color Part5Color;
        public Color Part6Color;
        public Color Part7Color;
        #endregion
        public void OnUse()
        {
            Destroy(gameObject);
        }
    }
}
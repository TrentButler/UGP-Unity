using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{


    public class PlayerDress : MonoBehaviour
    {
        #region PlayerName
        public string PlayerName;

        public int PlayerIndex;
        #endregion
        #region Colors
        public Color ShirtColor;
        public Color PantsColor;
        public Color SkinColor;
        #endregion
        public void OnUse()
        {
            Destroy(gameObject);
        }
    }
}
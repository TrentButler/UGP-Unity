using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UGP
{
    //NEEDS WORK
    public class OfflinePlayerInteractionBehaviour : MonoBehaviour
    {
        public bool isHolding;

        public void SetHolding(bool holding)
        {
            isHolding = holding;
        }

        private void Start()
        {
            isHolding = false;
        }
    }
}
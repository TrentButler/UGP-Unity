using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class OfflineUserControl : MonoBehaviour
    {
        public string InputHorizontal = "Horizontal";
        public string InputVertical = "Vertical";

        public InputController ic;

        void FixedUpdate()
        {
            var h = Input.GetAxis(InputHorizontal);
            var v = Input.GetAxis(InputVertical);

            ic.Move(h, v);
            ic.Rotate(h, v, 0.0f);
        }
    }
}
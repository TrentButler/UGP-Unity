using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class NetworkUserControl : NetworkBehaviour
    {
        public string InputHorizontal = "Horizontal";
        public string InputVertical = "Vertical";

        public DefaultVehicleController ic;

        void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    var _h = Input.GetAxis(InputHorizontal);
                    var _v = Input.GetAxis(InputVertical);
                    ic.Move(_h, _v);
                    ic.Rotate(_h, _v, 0.0f);
                    return;
                }

                return;
            }

            var h = Input.GetAxis(InputHorizontal);
            var v = Input.GetAxis(InputVertical);
            ic.Move(h, v);
            ic.Rotate(h, v, 0.0f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class NetworkUserControl : NetworkBehaviour
    {
        //CONVERT THIS TO A SCRIPTABLE OBJECT
        //INPUT CONFIGURATION
        //LIST ALL THE BUTTON/AXIS NEEDED FOR THE GAME

        public string InputHorizontal = "Horizontal";
        public string InputVertical = "Vertical";
        public string AltInputHorizontal;
        public string AltInputVertical;
        public string CameraInputHorizontal = "Mouse X";
        public string CameraInputVertical = "Mouse Y";
        public bool InvertCameraHorizontal = false;
        public bool InvertCameraVertical = false;

        public InputController ic;

        public float GetCurrentVerticalInput()
        {
            return Input.GetAxis(InputVertical);
        }
        public float GetCurrentHorizontalInput()
        {
            return Input.GetAxis(InputHorizontal);
        }

        void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                if(hasAuthority && !isServer)
                {
                    var _h = Input.GetAxis(InputHorizontal);
                    var _v = Input.GetAxis(InputVertical);
                    var alt_h = Input.GetAxis(AltInputHorizontal);
                    ic.Move(_h, _v);
                    ic.Rotate(alt_h, _v, 0.0f);
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
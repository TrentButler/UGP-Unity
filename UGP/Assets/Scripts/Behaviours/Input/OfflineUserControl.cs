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
        public string AltInputHorizontal;
        public string AltInputVertical;
        public string CameraInputHorizontal = "Mouse X";
        public string CameraInputVertical = "Mouse Y";
        public bool InvertCameraHorizontal = false;
        public bool InvertCameraVertical = false;

        public InputController ic;

        public float GetCurrentForwardInput()
        {
            return Input.GetAxis(InputVertical);
        }
        public float GetCurrentHorizontalInput()
        {
            return Input.GetAxis(InputHorizontal);
        }

        void FixedUpdate()
        {
            var _h = Input.GetAxis(InputHorizontal);
            var _v = Input.GetAxis(InputVertical);
            var alt_h = Input.GetAxis(AltInputHorizontal);
            ic.Move(_h, _v);
            ic.Rotate(alt_h, _v, 0.0f);
        }
    }
}
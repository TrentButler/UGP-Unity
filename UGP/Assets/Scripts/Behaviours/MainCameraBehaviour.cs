using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Trent
{
    public class MainCameraBehaviour : MonoBehaviour
    {
        public Vector3 Offset;
        public float CameraRotation;
        public float cameraSpeed = 1.0f;
        public float cameraTranslationSpeed = 1.0f;

        public void ResetCamera()
        {
            transform.rotation = Quaternion.identity; //ZERO OUT THE ROTATION
        }

        private void FixedUpdate()
        {
            #region MouseLook
            //CALCULATE AN MOUSE DELTA
            //DERIVE AN DIRECTION
            //LERP BETWEEN THE CURRENT CAMERA ROTATION TO AN NEW CAMERA ROTATION
            if (Input.GetMouseButton(1))
            {
                var deltaX = Input.GetAxis("Mouse X"); //GET THE MOUSE DELTA X
                var deltaY = Input.GetAxis("Mouse Y"); //GET THE MOUSE DELTA Y

                Vector3 rotX = new Vector3(-deltaY, 0, 0);
                Vector3 rotY = new Vector3(0, deltaX, 0);

                Quaternion rot = Quaternion.Euler(rotX); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

                rot = Quaternion.Euler(rotY); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            }

            //TRANSLATE THE CAMERA BASED ON CLICKDRAG DELTA
            if (Input.GetMouseButton(2))
            {
                var dX = Input.GetAxis("Mouse X"); //GET THE DELTA MOUSE X
                var dY = Input.GetAxis("Mouse Y"); //GET THE DELTA MOUSE Y

                Vector3 trans = new Vector3(-(dX * cameraTranslationSpeed), -(dY * cameraTranslationSpeed), 0); //DERIVE A TRANSLATION VECTOR
                GetComponent<Transform>().Translate(trans); //APPLY THE TRANSLATION TO THE CAMERA'S TRANSFORM
            }

            //MOVE THE CAMERA EITHER FORWARD OR BACKWARD FROM MOUSE SCROLL
            var scrollDelta = Input.GetAxis("Mouse ScrollWheel"); //GET THE MOUSE SCROLL DELTA
            Vector3 translation = new Vector3(0, 0, scrollDelta * cameraSpeed); //DERIVE A TRANSLATION VECTOR
            GetComponent<Transform>().Translate(translation); //TRANSLATE THE CAMERA'S TRANSFORM

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //RESET THE CAMERA'S ROTATION TO ZERO
                //ResetCamera();
            }
            #endregion
        }
    }
}
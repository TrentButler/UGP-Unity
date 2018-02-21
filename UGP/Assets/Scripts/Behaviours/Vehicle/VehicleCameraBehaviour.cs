using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class VehicleCameraBehaviour : NetworkBehaviour
    {
        private VehicleMovementBehaviour behaviour;
        public GameObject aimCamera;
        public GameObject followCamera;
        public GameObject cam;

        private void Start()
        {
            if (!isLocalPlayer)
                Destroy(cam);
                Destroy(aimCamera);
                Destroy(followCamera);
                return;

            //cam = transform.Find("Camera");

            //behaviour = GetComponentInParent<VehicleMovementBehaviour>();
            //aimCamera = GameObject.Find("AimCamera");
            //followCamera = GameObject.Find("FollowCamera");
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                //cam.gameObject.SetActive(false);
                return;

            //behaviour = GetComponentInParent<VehicleMovementBehaviour>();
            //aimCamera = GameObject.Find("AimCamera");
            //followCamera = GameObject.Find("FollowCamera");

            //switch (behaviour.mode._mode)
            //{
            //    case 0:
            //        {
            //            //SWITCH TO FOLLOW CAMERA
            //            followCamera.SetActive(true);
            //            aimCamera.SetActive(false);
            //            break;
            //        }

            //    case 1:
            //        {
            //            //SWITCH TO AIM CAMERA
            //            aimCamera.SetActive(true);
            //            followCamera.SetActive(false);

            //            var rot = aimCamera.transform.rotation;
            //            rot[0] = 0.0f;
            //            rot[2] = 0.0f;

            //            behaviour.Aim(rot);
            //            break;
            //        }
            //}
        }
    }
}
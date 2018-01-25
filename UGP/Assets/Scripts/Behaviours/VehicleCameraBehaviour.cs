using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class VehicleCameraBehaviour : MonoBehaviour
    {
        private VehicleMovementBehaviour behaviour;
        private GameObject aimCamera;
        private GameObject followCamera;

        private void Start()
        {
            behaviour = GetComponentInParent<VehicleMovementBehaviour>();
            aimCamera = GameObject.Find("AimCamera");
            followCamera = GameObject.Find("FollowCamera");
        }

        private void LateUpdate()
        {
            switch(behaviour.mode)
            {
                case VehicleState.DRIVE:
                    {
                        //SWITCH TO FOLLOW CAMERA
                        followCamera.SetActive(true);
                        aimCamera.SetActive(false);
                        break;
                    }

                case VehicleState.COMBAT:
                    {
                        //SWITCH TO AIM CAMERA
                        aimCamera.SetActive(true);
                        followCamera.SetActive(false);

                        var rot = aimCamera.transform.rotation;
                        rot[0] = 0.0f;
                        rot[2] = 0.0f;

                        
                        //behaviour.Aim(rot);
                        break;
                    }
            }
        }
    }
}
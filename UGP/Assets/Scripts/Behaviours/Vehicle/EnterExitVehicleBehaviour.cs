using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public enum RotationAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public class EnterExitVehicleBehaviour : MonoBehaviour
    {
        public Transform Door;
        public float MaxDoorRotation;
        public float DoorRotationSpeed = 1.0f;

        private bool isOpen;
        private bool isClosed;

        public void OpenDoor(RotationAxis axis)
        {
            if(!isOpen)
            {
                switch (axis)
                {
                    case RotationAxis.X:
                        {
                            //LERP THE X-ROTATION OF 'Door' TO THE VALUE 'MaxDoorRotation'
                            var currentRot = Door.rotation;
                            var x_rot = currentRot[0];
                            x_rot = Mathf.LerpAngle(x_rot, MaxDoorRotation, Time.smoothDeltaTime * DoorRotationSpeed);

                            break;
                        }

                    case RotationAxis.Y:
                        {
                            break;
                        }

                    case RotationAxis.Z:
                        {
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }



        void Update()
        {

        }
    }
}
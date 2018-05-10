using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UGP
{

    public class GarageHoverBehaviour : MonoBehaviour
    {
        #region Sliders
        public Slider RedSlider;
        public Slider GreenSlider;
        public Slider BlueSlider;
        #endregion
        #region Colors
        public Color Part0Color;
        public Color Part1Color;
        public Color Part2Color;
        public Color Part3Color;
        public Color Part4Color;
        public Color Part5Color;
        public Color Part6Color;
        public Color Part7Color;

        public int VehicleIndex;

        public float Red = 255;
        public float Green = 128;
        public float Blue = 255;
        #endregion
        public GameObject HoverBike;
        public GameObject Truck;
        public float hoverStrength;
        public float TargetHeight;
        public float RotateSpeed;
        public MeshRenderer HoverBikePt0;
        public MeshRenderer HoverBikePt1;
        public MeshRenderer HoverBikePt2;
        public MeshRenderer HoverBikePt3;
        public MeshRenderer HoverBikePt4;
        public MeshRenderer HoverBikePt5;
        public MeshRenderer HoverBikePt6;
        public MeshRenderer HoverBikePt7;

        public MeshRenderer TruckPt0;
        public MeshRenderer TruckPt1;
        public MeshRenderer TruckPt2;
        public MeshRenderer TruckPt3;
        public MeshRenderer TruckPt4;
        public MeshRenderer TruckPt5;
        public MeshRenderer TruckPt6;
        public MeshRenderer TruckPt7;

        private enum MeshType
        {
            Part0 = 0,
            Part1 = 1,
            Part2 = 2,
            Part3 = 3,
            Part4 = 4,
            Part5 = 5,
            Part6 = 6,
            Part7 = 7,
        }
        private MeshType type;

        public GameObject currentVehicle;
        public MeshRenderer currentMeshRender;
        public Color currentColor;

        public void OnHoverBike()
        {
            currentVehicle = HoverBike;
            ToggleHoverBikePartChange00();
            VehicleIndex = 0;
            Truck.SetActive(false);
            HoverBike.SetActive(true);
        }
        public void OnTruck()
        {
            currentVehicle = Truck;
            ToggleTruckPartChange00();
            VehicleIndex = 1;
            Truck.SetActive(true);
            HoverBike.SetActive(false);
        }
        public void OnApply()
        {
            var vehicle_dress = FindObjectOfType<VehicleDress>();
            if(vehicle_dress != null)
            {
                //OVERWRITE THIS ONE
                vehicle_dress.Part0Color = Part0Color;
                vehicle_dress.Part1Color = Part1Color;
                vehicle_dress.Part2Color = Part2Color;
                vehicle_dress.Part3Color = Part3Color;
                vehicle_dress.Part4Color = Part4Color;
                vehicle_dress.Part5Color = Part5Color;
                vehicle_dress.Part6Color = Part6Color;
                vehicle_dress.Part7Color = Part7Color;
            }
            else
            {
                var vehicleDress = Instantiate(new GameObject());
                vehicleDress.name = "VEHICLEDRESS";
                var vehicleDressBehaviour = vehicleDress.AddComponent<VehicleDress>();

                vehicleDressBehaviour.Part0Color = Part0Color;
                vehicleDressBehaviour.Part1Color = Part1Color;
                vehicleDressBehaviour.Part2Color = Part2Color;
                vehicleDressBehaviour.Part3Color = Part3Color;
                vehicleDressBehaviour.Part4Color = Part4Color;
                vehicleDressBehaviour.Part5Color = Part5Color;
                vehicleDressBehaviour.Part6Color = Part6Color;
                vehicleDressBehaviour.Part7Color = Part7Color;

                DontDestroyOnLoad(vehicleDress);
            }
        }

        public void ToggleHoverBikePartChange00()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt0;
                currentColor = Part0Color;
                type = MeshType.Part0;
            }
            else
            {
                currentMeshRender = TruckPt0;
                currentColor = Part0Color;
                type = MeshType.Part0;
            }
        }
        public void ToggleHoverBikePartChange01()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt1;
                currentColor = Part1Color;
                type = MeshType.Part1;
            }
            else
            {
                currentMeshRender = TruckPt1;
                currentColor = Part1Color;
                type = MeshType.Part1;
            }
        }
        public void ToggleHoverBikePartChange02()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt2;
                currentColor = Part2Color;
                type = MeshType.Part2;
            }
            else
            {
                currentMeshRender = TruckPt2;
                currentColor = Part2Color;
                type = MeshType.Part2;
            }
        }
        public void ToggleHoverBikePartChange03()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt3;
                currentColor = Part3Color;
                type = MeshType.Part3;
            }
            else
            {
                currentMeshRender = TruckPt3;
                currentColor = Part3Color;
                type = MeshType.Part3;
            }
        }
        public void ToggleHoverBikePartChange04()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt4;
                currentColor = Part4Color;
                type = MeshType.Part4;
            }
            else
            {
                currentMeshRender = TruckPt4;
                currentColor = Part4Color;
                type = MeshType.Part4;
            }
        }
        public void ToggleHoverBikePartChange05()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt5;
                currentColor = Part5Color;
                type = MeshType.Part5;
            }
            else
            {
                currentMeshRender = TruckPt5;
                currentColor = Part5Color;
                type = MeshType.Part5;
            }
        }
        public void ToggleHoverBikePartChange06()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt6;
                currentColor = Part6Color;
                type = MeshType.Part6;
            }
            else
            {
                currentMeshRender = TruckPt6;
                currentColor = Part6Color;
                type = MeshType.Part6;
            }
        }
        public void ToggleHoverBikePartChange07()
        {
            if (currentVehicle == HoverBike)
            {
                currentMeshRender = HoverBikePt7;
                currentColor = Part7Color;
                type = MeshType.Part7;
            }
            else
            {
                currentMeshRender = TruckPt7;
                currentColor = Part7Color;
                type = MeshType.Part7;
            }
        }


        public void ToggleTruckPartChange00()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt0;
                currentColor = Part0Color;
                type = MeshType.Part0;
            }
            else
            {
                currentMeshRender = TruckPt0;
                currentColor = Part0Color;
                type = MeshType.Part0;
            }
        }
        public void ToggleTruckPartChange01()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt1;
                currentColor = Part1Color;
                type = MeshType.Part1;
            }
            else
            {
                currentMeshRender = TruckPt1;
                currentColor = Part1Color;
                type = MeshType.Part1;
            }
        }
        public void ToggleTruckPartChange02()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt2;
                currentColor = Part2Color;
                type = MeshType.Part2;
            }
            else
            {
                currentMeshRender = TruckPt2;
                currentColor = Part2Color;
                type = MeshType.Part2;
            }
        }
        public void ToggleTruckPartChange03()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt3;
                currentColor = Part3Color;
                type = MeshType.Part3;
            }
            else
            {
                currentMeshRender = TruckPt3;
                currentColor = Part3Color;
                type = MeshType.Part3;
            }
        }
        public void ToggleTruckPartChange04()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt4;
                currentColor = Part4Color;
                type = MeshType.Part4;
            }
            else
            {
                currentMeshRender = TruckPt4;
                currentColor = Part4Color;
                type = MeshType.Part4;
            }
        }
        public void ToggleTruckPartChange05()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt5;
                currentColor = Part5Color;
                type = MeshType.Part5;
            }
            else
            {
                currentMeshRender = TruckPt5;
                currentColor = Part5Color;
                type = MeshType.Part5;
            }
        }
        public void ToggleTruckPartChange06()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt6;
                currentColor = Part6Color;
                type = MeshType.Part6;
            }
            else
            {
                currentMeshRender = TruckPt6;
                currentColor = Part6Color;
                type = MeshType.Part6;
            }
        }
        public void ToggleTruckPartChange07()
        {
            if (currentVehicle == Truck)
            {
                currentMeshRender = TruckPt7;
                currentColor = Part7Color;
                type = MeshType.Part7;
            }
            else
            {
                currentMeshRender = TruckPt7;
                currentColor = Part7Color;
                type = MeshType.Part7;
            }
        }


        // Use this for initialization
        void Start()
        {
            OnHoverBike();
            ToggleHoverBikePartChange00();
            //PlayerCam.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {


            currentMeshRender.material.color = new Vector4(Red, Green, Blue, 1);
            switch (type)
            {
                case MeshType.Part0:
                    {
                        Part0Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part1:
                    {
                        Part1Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part2:
                    {
                        Part2Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part3:
                    {
                        Part3Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part4:
                    {
                        Part4Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part5:
                    {
                        Part5Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part6:
                    {
                        Part6Color = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Part7:
                    {
                        Part7Color = currentMeshRender.material.color;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            currentVehicle.transform.Rotate(Vector3.up * Time.smoothDeltaTime);
        }
        void FixedUpdate()
        {

        }
    }

}
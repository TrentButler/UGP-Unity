using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace UGP
{
    //NEEDS WORK
    public class VehicleBehaviour : NetworkBehaviour
    {
        public GameObject VirtualCamera;
        public Color vColor;
        public List<MeshRenderer> models;
        [Range(0.001f, 1.0f)] public float colorChangeSpeed;
        public Vehicle VehicleConfig;
        [HideInInspector] public Vehicle _v;
        public InGameVehicleMovementBehaviour vehicleMovement;
        public VehicleShootBehaviour shootBehaviour;
        public Canvas vehicleUI;
        public Vector3 vehicleCamOffset;

        [SyncVar] public bool vehicleActive;
        public Transform seat;

        //NEEDS WORK
        private void UpdateUI()
        {
            //HEALTH SLIDER
            //FUEL SLIDER

            //FORMAT AND POPULATE THE TEXT BOX WITH THE INFO FROM 'AMMOBOX'

            var fuel = _v.Fuel;
            var health = _v.Health;

            var sliders = vehicleUI.GetComponentsInChildren<Slider>().ToList();
            sliders.ForEach(s =>
            {
                if (s.name == "Health")
                {
                    s.value = health;
                    s.maxValue = _v.MaxHealth;
                }
                else
                {
                    s.value = fuel;
                    s.maxValue = _v.MaxFuel;
                }

            });

            var text = vehicleUI.GetComponentInChildren<Text>();

            var _assault = _v.ammunition.Assault;
            var _shotgun = _v.ammunition.Shotgun;
            var _sniper = _v.ammunition.Sniper;
            var _rocket = _v.ammunition.Rocket;

            string sAssault = "ASSAULT: " + _assault;
            string sShotgun = "SHOTGUN: " + _shotgun;
            string sSniper = "SNIPER: " + _sniper;
            string sRocket = "ROCKET: " + _rocket;

            text.text = sAssault + "\n" + sShotgun + "\n"
                + sSniper + "\n" + sRocket;
        }

        public void SetVehicleActive(bool active)
        {
            vehicleActive = active;
        }

        [Command] private void CmdUpdateVehicle()
        {
            var health = _v.Health;
            var fuel = _v.Fuel;

            if (health <= 0.0f)
            {
                _v.Destroyed = true;
            }

            if (fuel <= 0.0f)
            {
                _v.FuelDepeleted = true;
            }
        }

        [Command] public void CmdOnVehicleEnter()
        {
            //LERP THE CURRENT COLOR OF THE VEHICLE TO THE TARGET COLOR
            models.ForEach(m =>
            {
                var oldColor = m.material.color;
                var cVector = new Vector4(oldColor.r, oldColor.g, oldColor.b, oldColor.a);

                var targetColor = new Vector4(vColor.r, vColor.g, vColor.b, vColor.a);

                var lerpR = Mathf.Lerp(cVector.x, targetColor.x, Time.deltaTime * colorChangeSpeed);
                var lerpG = Mathf.Lerp(cVector.y, targetColor.y, Time.deltaTime * colorChangeSpeed);
                var lerpB = Mathf.Lerp(cVector.z, targetColor.z, Time.deltaTime * colorChangeSpeed);
                var lerpA = Mathf.Lerp(cVector.w, targetColor.w, Time.deltaTime * colorChangeSpeed);

                var lerpColor = new Vector4(lerpR, lerpG, lerpB, lerpA);

                m.material.color = lerpColor;
            });
        }
        [Command] public void CmdOnVehicleExit()
        {
            //LERP THE CURRENT COLOR OF THE VEHICLE TO THE TARGET COLOR
            models.ForEach(m =>
            {
                var oldColor = m.material.color;
                var cVector = new Vector4(oldColor.r, oldColor.g, oldColor.b, oldColor.a);

                var targetColor = Vector4.one;

                var lerpR = Mathf.Lerp(cVector.x, targetColor.x, Time.deltaTime * colorChangeSpeed);
                var lerpG = Mathf.Lerp(cVector.y, targetColor.y, Time.deltaTime * colorChangeSpeed);
                var lerpB = Mathf.Lerp(cVector.z, targetColor.z, Time.deltaTime * colorChangeSpeed);
                var lerpA = Mathf.Lerp(cVector.w, targetColor.w, Time.deltaTime * colorChangeSpeed);

                var lerpColor = new Vector4(lerpR, lerpG, lerpB, lerpA);

                m.material.color = lerpColor;
            });
        }


        public override void OnStartClient()
        {
            _v = Instantiate(VehicleConfig);
            _v.Health = _v.MaxHealth;
            _v.Fuel = _v.MaxFuel;
            _v.Destroyed = false;
            _v.FuelDepeleted = false;
            vehicleActive = false;
        }

        private void Awake()
        {
            if (!isLocalPlayer)
            {   
                if(hasAuthority || isServer)
                {
                    return;
                }
                VirtualCamera.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    vehicleActive = false;
                    _v = Instantiate(VehicleConfig);
                    _v.Health = _v.MaxHealth;
                    _v.Fuel = _v.MaxFuel;
                    _v.Destroyed = false;
                    _v.FuelDepeleted = false;

                    models.ForEach(m =>
                    {
                        m.material.color = vColor;
                    });
                    return;
                }

                VirtualCamera.SetActive(false);
                return;
            }

            vehicleActive = false;
            _v = Instantiate(VehicleConfig);
            _v.Health = _v.MaxHealth;
            _v.Fuel = _v.MaxFuel;
            _v.Destroyed = false;
            _v.FuelDepeleted = false;

            models.ForEach(m =>
            {
                m.material.color = vColor;
            });
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                if(hasAuthority)
                {
                    switch (vehicleActive)
                    {
                        case true:
                            {
                                VirtualCamera.SetActive(true);
                                CmdOnVehicleEnter();
                                Cursor.visible = false;
                                vehicleMovement.enabled = true;
                                //shootBehaviour.enabled = true;
                                vehicleUI.gameObject.SetActive(true);
                                vehicleUI.enabled = true;
                                CmdUpdateVehicle();
                                UpdateUI();
                                break;
                            }

                        case false:
                            {
                                VirtualCamera.SetActive(false);
                                CmdOnVehicleExit();
                                Cursor.visible = true;
                                vehicleMovement.enabled = false;
                                //shootBehaviour.enabled = false;
                                vehicleUI.gameObject.SetActive(false);
                                vehicleUI.enabled = false;
                                CmdUpdateVehicle();
                                break;
                            }
                    }
                    return;
                } 

                VirtualCamera.SetActive(false);
                return;
            }

            switch (vehicleActive)
            {
                case true:
                    {
                        VirtualCamera.SetActive(true);
                        CmdOnVehicleEnter();
                        Cursor.visible = false;
                        vehicleMovement.enabled = true;
                        //shootBehaviour.enabled = true;
                        vehicleUI.gameObject.SetActive(true);
                        vehicleUI.enabled = true;
                        CmdUpdateVehicle();
                        UpdateUI();
                        break;
                    }

                case false:
                    {
                        VirtualCamera.SetActive(false);
                        CmdOnVehicleExit();
                        Cursor.visible = true;
                        vehicleMovement.enabled = false;
                        //shootBehaviour.enabled = false;
                        vehicleUI.gameObject.SetActive(false);
                        vehicleUI.enabled = false;
                        CmdUpdateVehicle();
                        break;
                    }
            }
        }
    }

}
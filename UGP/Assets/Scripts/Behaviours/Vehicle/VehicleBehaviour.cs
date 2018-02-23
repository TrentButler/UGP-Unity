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

        private void UpdateVehicle()
        {
            var health = _v.Health;
            var fuel = _v.Fuel;

            if(health <= 0.0f)
            {
                _v.Destroyed = true;
            }

            if(fuel <= 0.0f)
            {
                _v.FuelDepeleted = true;
            }
        }

        public void SetVehicleActive(bool active)
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }
            vehicleActive = active;
        }

        public void OnVehicleEnter()
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
        public void OnVehicleExit()
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

        private void Awake()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            vehicleActive = false;
        }

        private void Start()
        {
            if (!localPlayerAuthority)
            {
                enabled = false;
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
            if (!localPlayerAuthority)
            {
                enabled = false;
                return;
            }

            switch (vehicleActive)
            {
                case true:
                    {
                        OnVehicleEnter();
                        Cursor.visible = false;
                        vehicleMovement.enabled = true;
                        shootBehaviour.enabled = true;
                        vehicleUI.gameObject.SetActive(true);
                        vehicleUI.enabled = true;
                        UpdateVehicle();
                        UpdateUI();
                        break;
                    }

                case false:
                    {
                        OnVehicleExit();
                        Cursor.visible = true;
                        vehicleMovement.enabled = false;
                        shootBehaviour.enabled = false;
                        vehicleUI.gameObject.SetActive(false);
                        vehicleUI.enabled = false;
                        UpdateVehicle();
                        break;
                    }
            }
        }
    }

}
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
        #region COLOR_CHANGE
        public Color vColor;
        public List<MeshRenderer> models;
        [Range(0.001f, 1.0f)] public float colorChangeSpeed;
        #endregion

        public GameObject VirtualCamera;
        public DefaultVehicleController ic;
        public VehicleShootBehaviour shootBehaviour;

        public Vehicle VehicleConfig;
        [HideInInspector] public Vehicle _v;

        public Canvas vehicleUI;

        public Transform seat;
        public Animator ani;

        #region SYNCED_VARIABLES
        [SyncVar] public bool vehicleActive; //ADD FUNCTION TO TRIGGER VEHICLE DESTROYED EVENT???? (hook = "OnVehicleDestroyed")
        [SyncVar(hook = "OnVehicleHealthChange")] public float vehicleHealth;
        //[SyncVar] public float vehicleArmor;
        [SyncVar(hook = "OnVehicleFuelChange")] public float vehicleFuel;
        #endregion

        [Command]
        public void CmdUseFuel(float fuelUsed)
        {
            vehicleFuel -= fuelUsed;

            if (vehicleFuel <= 0.0f)
            {
                vehicleActive = false;
            }
        }

        [Command]
        public void CmdTakeDamage(float healthTaken)
        {
            vehicleHealth -= healthTaken;

            if (vehicleHealth <= 0.0f)
            {
                vehicleActive = false;
            }
        }

        private void UpdateVehicle()
        {
            if (vehicleHealth <= 0.0f)
            {
                vehicleActive = false;
            }

            if (vehicleFuel <= 0.0f)
            {
                vehicleActive = false;
            }
        }


        public void OnVehicleHealthChange(float healthChange)
        {
            var sliders = vehicleUI.GetComponentsInChildren<Slider>().ToList();
            sliders.ForEach(s =>
            {
                if (s.name == "Health")
                {
                    s.value = healthChange;
                    s.maxValue = _v.MaxHealth;
                }
            });
        }
        public void OnVehicleFuelChange(float fuelChange)
        {
            var sliders = vehicleUI.GetComponentsInChildren<Slider>().ToList();
            sliders.ForEach(s =>
            {
                if (s.name == "Fuel")
                {
                    s.value = fuelChange;
                    s.maxValue = _v.MaxFuel;
                }
            });
        }

        //NEEDS WORK
        private void UpdateUI()
        {
            #region OLD
            //HEALTH SLIDER
            //FUEL SLIDER

            //FORMAT AND POPULATE THE TEXT BOX WITH THE INFO FROM 'AMMOBOX'

            //var fuel = _v.Fuel;
            //var health = _v.Health;

            //var sliders = vehicleUI.GetComponentsInChildren<Slider>().ToList();
            //sliders.ForEach(s =>
            //{
            //    if (s.name == "Health")
            //    {
            //        s.value = health;
            //        s.maxValue = _v.MaxHealth;
            //    }
            //    else
            //    {
            //        s.value = fuel;
            //        s.maxValue = _v.MaxFuel;
            //    }

            //});
            #endregion

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

        //NEEDS WORK
        //- UPDATE THE VEHICLE'S HEALTH AND FUEL
        [Command]
        private void CmdUpdateVehicle()
        {
            //var health = _v.Health;
            //var fuel = _v.Fuel;

            //if (health <= 0.0f)
            //{
            //    _v.Destroyed = true;
            //}

            //if (fuel <= 0.0f)
            //{
            //    _v.FuelDepeleted = true;
            //}
        }

        public void SetVehicleActive(bool active)
        {
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

        //public override void OnStartClient()
        //{
        //    if(_v == null)
        //    {
        //        _v = Instantiate(VehicleConfig);
        //    }
        //}

        private void Start()
        {
            if (!isLocalPlayer)
            {
                if (hasAuthority || isServer)
                {
                    if (_v == null)
                    {
                        if (VehicleConfig == null)
                        {
                            VehicleConfig = Resources.Load("Assets//Resources//ScriptableObjects//Vehicles//BasicVehicle") as Vehicle;
                        }
                        _v = Instantiate(VehicleConfig);
                        vehicleHealth = _v.MaxHealth;
                        vehicleFuel = _v.MaxFuel;
                        _v.Destroyed = false;
                        _v.FuelDepeleted = false;
                        //vehicleActive = false;
                    }

                    models.ForEach(m =>
                    {
                        m.material.color = vColor;
                    });
                    return;
                }

                VirtualCamera.SetActive(false);
                return;
            }

            if (_v == null)
            {
                if (VehicleConfig == null)
                {
                    VehicleConfig = Resources.Load("Assets//Resources//ScriptableObjects//Vehicles//BasicVehicle") as Vehicle;
                }
                _v = Instantiate(VehicleConfig);
                vehicleHealth = _v.MaxHealth;
                vehicleFuel = _v.MaxFuel;
                _v.Destroyed = false;
                _v.FuelDepeleted = false;
                //vehicleActive = false;
            }

            models.ForEach(m =>
            {
                m.material.color = vColor;
            });
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                UpdateVehicle();
                return;
            }

            if (!isLocalPlayer)
            {
                if (hasAuthority && !isServer)
                {
                    if (vehicleActive)
                    {
                        VirtualCamera.SetActive(true);
                        OnVehicleEnter();
                        Cursor.visible = false;
                        ic.enabled = true;
                        //shootBehaviour.enabled = true;
                        vehicleUI.gameObject.SetActive(true);
                        vehicleUI.enabled = true;
                        //CmdUpdateVehicle();
                        //UpdateUI();
                    }
                    else
                    {
                        VirtualCamera.SetActive(false);
                        OnVehicleExit();
                        Cursor.visible = true;
                        ic.enabled = false;
                        //shootBehaviour.enabled = false;
                        vehicleUI.gameObject.SetActive(false);
                        vehicleUI.enabled = false;
                        //CmdUpdateVehicle();
                    }
                    return;
                }

                VirtualCamera.SetActive(false);
                return;
            }

            if (vehicleActive)
            {
                VirtualCamera.SetActive(true);
                OnVehicleEnter();
                Cursor.visible = false;
                ic.enabled = true;
                //shootBehaviour.enabled = true;
                vehicleUI.gameObject.SetActive(true);
                vehicleUI.enabled = true;
                //CmdUpdateVehicle();
                //UpdateUI();
            }
            else
            {
                VirtualCamera.SetActive(false);
                OnVehicleExit();
                Cursor.visible = true;
                ic.enabled = false;
                //shootBehaviour.enabled = false;
                vehicleUI.gameObject.SetActive(false);
                vehicleUI.enabled = false;
                //CmdUpdateVehicle();
            }
        }
    }
}
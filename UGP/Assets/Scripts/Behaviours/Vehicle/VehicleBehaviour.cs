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
        public NetworkUserControl ic;
        public VehicleShootBehaviour shootBehaviour;

        public Vehicle VehicleConfig;
        [HideInInspector] public Vehicle _v;

        #region UI
        public Canvas vehicleUI;
        public Slider HealthSlider;
        public Slider FuelSlider;
        #endregion

        public Transform seat;
        public Animator ani;

        #region SYNCED_VARIABLES
        [SyncVar] public bool vehicleActive; //ADD FUNCTION TO TRIGGER VEHICLE DESTROYED EVENT???? (hook = "OnVehicleDestroyed")
        [SyncVar] public bool playerInSeat = false;
        [SyncVar(hook = "OnVehicleHealthChange")] public float vehicleHealth;
        private float max_health;
        [SyncVar(hook = "OnVehicleFuelChange")] public float vehicleFuel;
        private float max_fuel;
        #endregion

        //VEHICLE TAKE HEALTH
        //VEHICLE TAKE ARMOR

        //VEHICLE TAKE DAMAGE

        //VEHICLE TAKE FUEL
        //VEHICLE USE FUEL

        #region COMMAND_FUNCTIONS
        [Command] public void CmdTakeHealth(float healthTaken)
        {
            //INCREMENT VEHICLE HEALTH, CLAMP THE VALUE BETWEEN 0.0F AND THE 'MaxHealth'

            vehicleHealth += healthTaken;
            vehicleHealth = Mathf.Clamp(vehicleHealth, 0.0f, _v.MaxHealth);
            //Debug.Log("VEHICLE TAKE " + healthTaken.ToString() + " HEALTH");
        }

        [Command] public void CmdTakeDamage(float healthTaken)
        {
            //DEPLETE THE VEHICLE'S HEALTH
            vehicleHealth -= healthTaken;
            //Debug.Log("VEHICLE TAKE " + healthTaken.ToString() + " DAMAGE");

            if (vehicleHealth <= 0.0f)
            {
                vehicleActive = false;
            }
        }

        [Command] public void CmdRefuel(float refuel)
        {
            vehicleFuel += refuel;
            vehicleFuel = Mathf.Clamp(vehicleFuel, 0.0f, _v.MaxFuel);
        }

        [Command] public void CmdUseFuel(float fuelUsed)
        {
            vehicleFuel -= fuelUsed;

            if (vehicleFuel <= 0.0f)
            {
                vehicleActive = false;
            }
        }
        #endregion

        public void OnVehicleHealthChange(float healthChange)
        {
            HealthSlider.value = healthChange;
            HealthSlider.maxValue = max_health;
        }
        public void OnVehicleFuelChange(float fuelChange)
        {
            FuelSlider.value = fuelChange;
            FuelSlider.maxValue = max_fuel;
        }

        public void SetVehicleActive(string player_name, bool active)
        {
            Debug.Log(player_name + "SETACTIVE(" + active.ToString() + ") " + gameObject.name);
            vehicleActive = active;
        }

        [ClientRpc] public void RpcSetVehicleActive(bool active)
        {
            vehicleActive = active;
        }

        //NEEDS WORK
        private void UpdateUI()
        {
            var sliders = vehicleUI.GetComponentsInChildren<Slider>().ToList();
            sliders.ForEach(s =>
            {
                if (s.name == "Health")
                {
                    s.value = vehicleHealth;
                    s.maxValue = max_health;
                }
                if(s.name == "Fuel")
                {
                    s.value = vehicleFuel;
                    s.maxValue = max_fuel;
                }
            });

            //var text = vehicleUI.GetComponentInChildren<Text>();

            //var _assault = _v.ammunition.Assault;
            //var _shotgun = _v.ammunition.Shotgun;
            //var _sniper = _v.ammunition.Sniper;
            //var _rocket = _v.ammunition.Rocket;

            //string sAssault = "ASSAULT: " + _assault;
            //string sShotgun = "SHOTGUN: " + _shotgun;
            //string sSniper = "SNIPER: " + _sniper;
            //string sRocket = "ROCKET: " + _rocket;

            //text.text = sAssault + "\n" + sShotgun + "\n"
            //    + sSniper + "\n" + sRocket;
        }
        
        private void UpdateVehicle()
        {
            //- SERVER ONLY
            //- CHECK IF VEHICLE FUEL OR HEALTH IS BELOW ZERO(0)
            //- SET VEHICLE ACTIVE TO FALSE

            if (vehicleHealth <= 0.0f)
            {
                //vehicleActive = false;
                RpcSetVehicleActive(false);
                Debug.Log("VEHICLE OUT OF HEALTH");
            }

            if (vehicleFuel <= 0.0f)
            {
                //vehicleActive = false;
                RpcSetVehicleActive(false);
                Debug.Log("VEHICLE OUT OF FUEL");
            }
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
                max_health = _v.MaxHealth;

                vehicleFuel = _v.MaxFuel;
                max_fuel = _v.MaxFuel;
                
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
                        //UpdateVehicle();
                        UpdateUI();
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
                UpdateUI();
            }
            else
            {
                if(playerInSeat)
                {
                    VirtualCamera.SetActive(true);
                }
                else
                {
                    VirtualCamera.SetActive(false);
                }
                
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
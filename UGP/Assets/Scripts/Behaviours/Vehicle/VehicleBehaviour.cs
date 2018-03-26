﻿using System.Collections.Generic;
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
        public List<MeshRenderer> models;
        [Range(0.001f, 1.0f)] public float colorChangeSpeed;
        #endregion

        public GameObject VirtualCamera;
        public NetworkUserControl ic;
        public VehicleShootBehaviour shootBehaviour;

        public Vehicle VehicleConfig;
        [HideInInspector] public Vehicle _v;

        #region UI
        public GameObject vehicleUI;
        public Slider HealthSlider;
        public Slider FuelSlider;
        #endregion

        public Transform seat;
        public Animator ani;

        #region SYNCED_VARIABLES
        [SyncVar] public bool vehicleActive; //ADD FUNCTION TO TRIGGER VEHICLE DESTROYED EVENT???? (hook = "OnVehicleDestroyed")
        [SyncVar] public Color vColor;
        [SyncVar] public bool playerInSeat = false;
        [SyncVar(hook = "OnVehicleHealthChange")] public float vehicleHealth;
        [SyncVar] private float max_health;
        [SyncVar(hook = "OnVehicleFuelChange")] public float vehicleFuel;
        [SyncVar] private float max_fuel;
        #endregion

        #region COMMAND_FUNCTIONS
        [Command]
        public void CmdTakeHealth(float healthTaken)
        {
            //INCREMENT VEHICLE HEALTH, CLAMP THE VALUE BETWEEN 0.0F AND THE 'MaxHealth'

            vehicleHealth += healthTaken;
            vehicleHealth = Mathf.Clamp(vehicleHealth, 0.0f, _v.MaxHealth);
            //Debug.Log("VEHICLE TAKE " + healthTaken.ToString() + " HEALTH");
        }
        [Command]
        public void CmdTakeDamage(float healthTaken)
        {
            //DEPLETE THE VEHICLE'S HEALTH
            vehicleHealth -= healthTaken;
            //Debug.Log("VEHICLE TAKE " + healthTaken.ToString() + " DAMAGE");

            if (vehicleHealth <= 0.0f)
            {
                vehicleActive = false;
            }
        }
        [Command]
        public void CmdRefuel(float refuel)
        {
            vehicleFuel += refuel;
            vehicleFuel = Mathf.Clamp(vehicleFuel, 0.0f, _v.MaxFuel);
        }
        [Command]
        public void CmdUseFuel(float fuelUsed)
        {
            vehicleFuel -= fuelUsed;

            if (vehicleFuel <= 0.0f)
            {
                vehicleActive = false;
            }
        }
        #endregion

        #region CLIENTRPC_FUNCTIONS
        [ClientRpc] public void RpcSetVehicleActive(bool active)
        {
            vehicleActive = active;
        }
        #endregion

        public void OnVehicleHealthChange(float healthChange)
        {
            //RpcUpdateVehicleHealth(healthChange);

            vehicleHealth = Mathf.Clamp(healthChange, 0.0f, max_health);
            if (vehicleHealth <= 0.0f)
            {
                vehicleActive = false;
            }

            //HealthSlider.value = vehicleHealth;
            //HealthSlider.maxValue = max_health;
        }
        public void OnVehicleFuelChange(float fuelChange)
        {
            //RpcUpdateVehicleFuel(fuelChange);

            vehicleFuel = Mathf.Clamp(fuelChange, 0.0f, max_fuel);
            if (vehicleFuel <= 0.0f)
            {
                vehicleActive = false;
            }

            //FuelSlider.value = vehicleFuel;
            //FuelSlider.maxValue = max_fuel;
        }

        public void UpdateVehicleUI()
        {
            HealthSlider.value = Mathf.Clamp(vehicleHealth, 0.0f, max_health);

            //Debug.Log("VEHICLE HEALTH: " + vehicleHealth.ToString());
            //Debug.Log("HEALTH SLIDER VALUE: " + HealthSlider.value.ToString());
            //Debug.Log("HEALTH SLIDER MAX VALUE: " + HealthSlider.maxValue.ToString());


            FuelSlider.value = Mathf.Clamp(vehicleFuel, 0.0f, max_fuel);
            //Debug.Log("VEHICLE FUEL: " + vehicleFuel.ToString());
            //Debug.Log("FUEL SLIDER VALUE: " + FuelSlider.value.ToString());
            //Debug.Log("FUEL SLIDER MAX VALUE: " + FuelSlider.maxValue.ToString());
        }

        private void UpdateVehicle()
        {
            //- SERVER ONLY
            //- CHECK IF VEHICLE FUEL OR HEALTH IS BELOW ZERO(0)
            //- SET VEHICLE ACTIVE TO FALSE

            if (vehicleHealth <= 0.0f)
            {
                RpcSetVehicleActive(false);
                Debug.Log("VEHICLE OUT OF HEALTH");
            }

            if (vehicleFuel <= 0.0f)
            {
                RpcSetVehicleActive(false);
                Debug.Log("VEHICLE OUT OF FUEL");
            }
            else
            {
                if (vehicleHealth > 0.0f && playerInSeat)
                {
                    RpcSetVehicleActive(true);
                }
            }
        }

        public void ColorChangeOn()
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
        public void ColorChangeOff()
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
            HealthSlider.maxValue = max_health;
            FuelSlider.maxValue = max_fuel;
        }

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

                        max_fuel = _v.MaxFuel;
                        max_health = _v.MaxHealth;

                        _v.Destroyed = false;
                        _v.FuelDepeleted = false;
                    }

                    HealthSlider.maxValue = max_health;
                    FuelSlider.maxValue = max_fuel;

                    models.ForEach(m =>
                    {
                        m.material.color = Vector4.one;
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

                max_fuel = _v.MaxFuel;
                max_health = _v.MaxHealth;

                _v.Destroyed = false;
                _v.FuelDepeleted = false;
            }

            HealthSlider.maxValue = max_health;
            FuelSlider.maxValue = max_fuel;

            models.ForEach(m =>
            {
                m.material.color = Vector4.one;
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
                        Cursor.visible = false;
                        ic.enabled = true;
                        //shootBehaviour.enabled = true;
                        UpdateVehicleUI();
                        vehicleUI.SetActive(true);
                    }
                    else
                    {
                        if (playerInSeat)
                        {
                            VirtualCamera.SetActive(true);
                        }
                        else
                        {
                            VirtualCamera.SetActive(false);
                        }

                        Cursor.visible = true;
                        ic.enabled = false;
                        shootBehaviour.enabled = false;
                    }
                    return;
                }

                if(!playerInSeat)
                {
                    vehicleUI.SetActive(false);
                }

                VirtualCamera.SetActive(false);
                return;
            }

            if (vehicleActive)
            {
                VirtualCamera.SetActive(true);
                Cursor.visible = false;
                ic.enabled = true;
                //shootBehaviour.enabled = true;
                vehicleUI.SetActive(true);
                UpdateVehicleUI();
            }
            else
            {
                if (playerInSeat)
                {
                    VirtualCamera.SetActive(true);
                }
                else
                {
                    VirtualCamera.SetActive(false);
                }

                Cursor.visible = true;
                ic.enabled = false;
                shootBehaviour.enabled = false;
                vehicleUI.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (vehicleActive)
            {
                ColorChangeOn();
            }
            else
            {
                ColorChangeOff();
            }
        }
    }
}
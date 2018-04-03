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
        [SyncVar] private float max_health, max_fuel;
        [SyncVar(hook = "OnVehicleHealthChange")] public float vehicleHealth;
        [SyncVar(hook = "OnVehicleFuelChange")] public float vehicleFuel;
        [SyncVar] public int Assault, Shotgun, Sniper, Rocket;
        #endregion

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
        [Command] public void CmdTakeAmmunition(int assault, int shotgun, int sniper, int rocket)
        {
            Assault += assault;
            Assault = Mathf.Clamp(Assault, 0, 999);

            Shotgun += shotgun;
            Shotgun = Mathf.Clamp(Shotgun, 0, 999);

            Sniper += sniper;
            Sniper = Mathf.Clamp(Sniper, 0, 999);

            Rocket += rocket;
            Rocket = Mathf.Clamp(Rocket, 0, 999);
        }
        [Command] public void CmdUseAmmunition(int assault, int shotgun, int sniper, int rocket)
        {
            Assault -= assault;
            Assault = Mathf.Clamp(Assault, 0, 999);

            Shotgun -= shotgun;
            Shotgun = Mathf.Clamp(Shotgun, 0, 999);

            Sniper -= sniper;
            Sniper = Mathf.Clamp(Sniper, 0, 999);

            Rocket -= rocket;
            Rocket = Mathf.Clamp(Rocket, 0, 999);
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
            FuelSlider.value = Mathf.Clamp(vehicleFuel, 0.0f, max_fuel);

            var text = vehicleUI.GetComponentInChildren<Text>();

            string sAssault = "ASSAULT: " + Assault;
            string sShotgun = "SHOTGUN: " + Shotgun;
            string sSniper = "SNIPER: " + Sniper;
            string sRocket = "ROCKET: " + Rocket;

            text.text = sAssault + "\n" + sShotgun + "\n"
                + sSniper + "\n" + sRocket;
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
                        
                        //vehicleHealth = _v.MaxHealth; //INITALIZE THE VEHICLE WITH FULL HEALTH
                        //vehicleFuel = _v.MaxFuel; //INITALIZE THE VEHICLE WITH FULL FUEL
                        vehicleHealth = _v.MaxHealth / 2; //INITALIZE THE VEHICLE WITH HALF HEALTH
                        vehicleFuel = 0.0f; //INITALIZE THE VEHICLE WITH NO FUEL

                        max_fuel = _v.MaxFuel;
                        max_health = _v.MaxHealth;

                        _v.Destroyed = false;
                        _v.FuelDepeleted = false;
                    }

                    var freeLook = VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>();
                    var virtualCamera = VirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

                    if (freeLook != null)
                    {
                        //ASSIGN THE CAMERA THE INPUT AXIS FROM THE INPUT CONTROLLER
                        VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = ic.CameraInputHorizontal;
                        VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = ic.CameraInputVertical;
                    }
                    if (virtualCamera != null)
                    {
                        var pov_camera = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachinePOV>();

                        pov_camera.m_HorizontalAxis.m_InputAxisName = ic.CameraInputHorizontal;
                        pov_camera.m_VerticalAxis.m_InputAxisName = ic.CameraInputVertical;
                        pov_camera.m_HorizontalAxis.m_InvertAxis = ic.InvertCameraHorizontal;
                        pov_camera.m_VerticalAxis.m_InvertAxis = ic.InvertCameraVertical;
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

                //vehicleHealth = _v.MaxHealth; //INITALIZE THE VEHICLE WITH FULL HEALTH
                //vehicleFuel = _v.MaxFuel; //INITALIZE THE VEHICLE WITH FULL FUEL
                vehicleHealth = _v.MaxHealth / 2; //INITALIZE THE VEHICLE WITH HALF HEALTH
                vehicleFuel = 0.0f; //INITALIZE THE VEHICLE WITH NO FUEL

                max_fuel = _v.MaxFuel;
                max_health = _v.MaxHealth;

                Assault = _v.ammunition.Assault;
                Shotgun = _v.ammunition.Shotgun;
                Sniper = _v.ammunition.Sniper;
                Rocket = _v.ammunition.Rocket;

                _v.Destroyed = false;
                _v.FuelDepeleted = false;
            }

            var free_look = VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>();
            var virtual_camera = VirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

            if (free_look != null)
            {
                //ASSIGN THE CAMERA THE INPUT AXIS FROM THE INPUT CONTROLLER
                VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = ic.CameraInputHorizontal;
                VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = ic.CameraInputVertical;
            }
            if (virtual_camera != null)
            {
                var pov_camera = virtual_camera.GetCinemachineComponent<Cinemachine.CinemachinePOV>();

                pov_camera.m_HorizontalAxis.m_InputAxisName = ic.CameraInputHorizontal;
                pov_camera.m_VerticalAxis.m_InputAxisName = ic.CameraInputVertical;
                pov_camera.m_HorizontalAxis.m_InvertAxis = ic.InvertCameraHorizontal;
                pov_camera.m_VerticalAxis.m_InvertAxis = ic.InvertCameraVertical;
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
                        shootBehaviour.enabled = true;
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
                shootBehaviour.enabled = true;
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
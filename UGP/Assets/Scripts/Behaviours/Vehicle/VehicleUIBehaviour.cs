using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class VehicleUIBehaviour : NetworkBehaviour
    {
        public VehicleBehaviour VehicleBrain;

        public GameObject vehicleUI;
        public GameObject enterVehicleUI;
        public Image HealthSlider;
        public Image FuelSlider;

        public float GetVehicleHealth()
        {
            //var health_displacement = VehicleBrain.vehicleHealth / VehicleBrain.max_health;
            //var calc = new Vector3(health_displacement, 0, 0);
            //return calc.normalized.x;

            return (1 / VehicleBrain.max_health) * VehicleBrain.vehicleHealth;
        }
        public float GetVehicleFuel()
        {
            //var fuel_displacement = VehicleBrain.vehicleFuel / VehicleBrain.max_fuel;
            //var calc = new Vector3(fuel_displacement, 0, 0);
            //return calc.normalized.x;

            return (1 / VehicleBrain.max_fuel) * VehicleBrain.vehicleFuel;
        }

        public void UpdateVehicleUI()
        {
            HealthSlider.fillAmount = GetVehicleHealth();
            FuelSlider.fillAmount = GetVehicleFuel();

            var text = vehicleUI.GetComponentInChildren<Text>();
            var shoot_behaviour = VehicleBrain.shootBehaviour;

            switch(shoot_behaviour.weapon.type)
            {
                case WeaponType.ASSAULT:
                    {
                        text.text = "";
                        text.text = VehicleBrain.Assault.ToString();
                        break;
                    }

                case WeaponType.SHOTGUN:
                    {
                        text.text = "";
                        text.text = VehicleBrain.Shotgun.ToString();
                        break;
                    }

                case WeaponType.SNIPER:
                    {
                        text.text = "";
                        text.text = VehicleBrain.Sniper.ToString();
                        break;
                    }

                case WeaponType.ROCKET:
                    {
                        text.text = "";
                        text.text = VehicleBrain.Rocket.ToString();
                        break;
                    }

                default:
                    {
                        text.text = "";
                        text.text = "0";
                        return;
                    }
            }
        }

        public override void OnStartClient()
        {
            HealthSlider.fillAmount = GetVehicleHealth();
            FuelSlider.fillAmount = GetVehicleFuel();
        }
        
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                if (hasAuthority && !isServer)
                {
                    if (VehicleBrain.vehicleActive)
                    {
                        Cursor.visible = false;
                        vehicleUI.SetActive(true);
                        UpdateVehicleUI();
                    }
                    else
                    {
                        Cursor.visible = true;
                    }
                    return;
                }

                if (!VehicleBrain.playerInSeat)
                {
                    vehicleUI.SetActive(false);
                }
                return;
            }

            if (VehicleBrain.vehicleActive)
            {
                Cursor.visible = false;
                vehicleUI.SetActive(true);
                UpdateVehicleUI();
            }
            else
            {
                Cursor.visible = true;
                vehicleUI.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (VehicleBrain.vehicleActive)
            {
                enterVehicleUI.SetActive(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //ENABLE THE ENTERVEHICLE UI
                var player_network_identity = other.GetComponentInParent<NetworkIdentity>();
                if (player_network_identity.isLocalPlayer && !VehicleBrain.vehicleActive && !VehicleBrain.playerInSeat)
                {
                    enterVehicleUI.SetActive(true);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            //DISABLE THE ENTERVEHICLE UI
            enterVehicleUI.SetActive(false);
        }
    }
}
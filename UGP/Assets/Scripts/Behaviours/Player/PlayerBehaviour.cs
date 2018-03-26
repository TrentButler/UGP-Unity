using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        public GameObject VirtualCamera;
        public GameObject model;
        public Player player;
        [HideInInspector] public Player p;
        [SyncVar] public Color vehicleColor;

        [SyncVar] public bool isDriving;
        public VehicleBehaviour vehicle;
        public float TimeToExitVehicle;
        private float exitTimer = 0.0f;
        public InputController ic;
        public PlayerInteractionBehaviour interaction;

        public Animator ani;

        #region COMMAND_FUNCTIONS
        [Command] public void CmdSetDriving(bool driving)
        {
            isDriving = driving;
        }
        [Command] private void CmdExitVehicle(NetworkIdentity identity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = identity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        #endregion

        public void SetVehicle(VehicleBehaviour vehicleBehaviour)
        {
            vehicle = vehicleBehaviour;
        }

        private void ExitVehicleWithTimer()
        {
            if (Input.GetKey(KeyCode.F))
            {
                exitTimer += Time.fixedDeltaTime; //INCREMENT TIME WHILE THE 'F' KEY IS HELD
            }
            else
            {
                exitTimer = 0; //RESET TIMER
            }

            if (exitTimer >= TimeToExitVehicle)
            {
                CmdSetDriving(false);
                var vehicleIdentity = vehicle.GetComponent<NetworkIdentity>();
                interaction.CmdSetVehicleActive(false, vehicleIdentity);
                interaction.CmdSetPlayerInSeat(false, vehicleIdentity);
                CmdExitVehicle(vehicleIdentity);
                exitTimer = 0.0f; //RESET THE TIMER

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                vehicle = null;
            }
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            p = Instantiate(player);
            p.Alive = true;
            p.Health = p.MaxHealth;

            if (ani == null)
            {
                ani = GetComponent<Animator>();
            }
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            if (isDriving)
            {
                VirtualCamera.SetActive(false);
                ic.enabled = false;
                interaction.enabled = false;
                
                ani.SetFloat("Walk", 0.0f);

                ExitVehicleWithTimer(); //EXIT THE VEHICLE
            }
            else
            {
                VirtualCamera.SetActive(true);
                ic.enabled = true;
                interaction.enabled = true;
            }
        }

        private void LateUpdate()
        {
            if (isDriving)
            {
                model.SetActive(false);
            }
            else
            {
                model.SetActive(true);
            }
        }
    }
}
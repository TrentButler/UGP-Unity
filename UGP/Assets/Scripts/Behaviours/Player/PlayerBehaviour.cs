using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        public GameObject VirtualCamera;
        public GameObject model;
        public GameObject RagDoll;
        public Player PlayerConfig;
        [HideInInspector] public Player _p;

        public VehicleBehaviour vehicle;
        public float TimeToExitVehicle;
        public float TimeToDestroyRagdoll = 10.0f;
        private float exitTimer = 0.0f;
        public NetworkUserControl ic;
        public PlayerInteractionBehaviour interaction;
        public PlayerUIBehaviour ui;

        public Animator ani;

        #region SYNCED_VARIABLES
        [SyncVar(hook = "OnPlayerHealthChange")] public float playerHealth;
        [SyncVar(hook = "OnisDeadChange")] public bool isDead;
        [SyncVar] public bool isDriving;
        [SyncVar] public Color vehicleColor;
        [SyncVar(hook = "OnMaxHealthAssign")] public float MaxHealth;
        #endregion

        #region SYNCVAR_HOOK_FUNCTIONS
        private void OnPlayerHealthChange(float healthChange)
        {
            playerHealth = Mathf.Clamp(healthChange, 0.0f, 99999);

            if (playerHealth <= 0.0f)
            {
                isDead = true;
                ic.enabled = false;
                interaction.enabled = false;
            }
        }
        private void OnMaxHealthAssign(float assignMaxHealth)
        {
            MaxHealth = assignMaxHealth;
        }
        private void OnisDeadChange(bool Dead)
        {
            isDead = Dead;
            if(isDead)
            {
                var holding_item = interaction.isHolding;
                if(holding_item)
                {
                    interaction.DropItem();
                    interaction.item.Drop();
                }
                
                CmdSpawnRagdoll();
            }
        }
        #endregion

        #region COMMAND_FUNCTIONS
        [Command] public void CmdTakeHealth(float healthTaken)
        {
            playerHealth += healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 99999);
        }
        [Command] public void CmdTakeDamage(float healthTaken)
        {
            Debug.Log("Player Take Damage");
            playerHealth -= healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 99999);
            if (playerHealth <= 0.0f)
            {
                isDead = true;
                //isDriving = false;
                ic.enabled = false;
                interaction.enabled = false;
            }
        }
        [Command] private void CmdSetHealth(float health)
        {
            playerHealth = health;
        }
        [Command] private void CmdSetMaxHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
        }
        [Command] private void CmdSetisDead(bool dead)
        {
            isDead = dead;
        }
        [Command] public void CmdSetDriving(bool driving)
        {
            isDriving = driving;
        }
        [Command] public void CmdRemoveVehicleAuthority(NetworkIdentity vehicleIdentity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var vehicleNetworkIdentity = vehicleIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            vehicleNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        [Command] private void CmdSpawnRagdoll()
        {
            var ragdoll = Instantiate(RagDoll, transform.position, transform.rotation);
            NetworkServer.Spawn(ragdoll);
            RpcSetRagdollPos(ragdoll);
        }
        [Command] public void CmdRespawn(Vector3 position, Quaternion rotation)
        {
            RpcRespawn(position, rotation);
        }
        [Command] public void CmdAssignPlayerAuthority(NetworkIdentity playerIdentity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var playerNetworkIdentity = playerIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            playerNetworkIdentity.AssignClientAuthority(localPlayerConn);
        }
        [Command] public void CmdRemovePlayerAuthority(NetworkIdentity playerIdentity)
        {
            var localPlayerNetworkIdentity = GetComponent<NetworkIdentity>();
            var localPlayerConn = localPlayerNetworkIdentity.connectionToClient;

            var playerNetworkIdentity = playerIdentity;

            //INVOKE THESE FUNCTIONS ON THE SERVER
            playerNetworkIdentity.RemoveClientAuthority(localPlayerConn);
        }
        #endregion

        #region CLIENTRPC_FUNCTIONS
        //NEEDS WORK
        [ClientRpc] private void RpcKillPlayer()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            isDead = true;
            //isDriving = false;
            ic.enabled = false;
            interaction.enabled = false;
        }
        [ClientRpc] private void RpcSetRagdollPos(GameObject ragdoll)
        {
            if (!isLocalPlayer)
                return;
            LookAt(ragdoll.transform);
        }
        [ClientRpc] private void RpcRespawn(Vector3 pos, Quaternion rot)
        {
            if(isLocalPlayer)
            {
                playerHealth = _p.MaxHealth;
                isDead = false;
                isDriving = false;

                CmdSetHealth(_p.MaxHealth);
                CmdSetMaxHealth(_p.MaxHealth);
                CmdSetisDead(false);
                CmdSetDriving(false);
                transform.position = pos;
                transform.rotation = rot;
                LookAt(transform);
            }
        }
        #endregion

        #region LOCAL_FUNCTIONS
        public void OnRespawn(Transform spawnPoint)
        {
            if (PlayerConfig == null)
            {
                PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
            }

            _p = Instantiate(PlayerConfig);

            //playerHealth = _p.MaxHealth;
            isDead = false;
            isDriving = false;
            
            CmdRespawn(spawnPoint.position, spawnPoint.rotation);
        }

        private void PlayerHealthRegenrate()
        {
            if (!isDead)
            {
                //IF THE PLAYER HASNT BEEN DAMAGED FOR A SPECIFIC AMOUT OF TIME, REGENRATE THE HEALTH
            }
        }

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
                CmdRemoveVehicleAuthority(vehicleIdentity);
                exitTimer = 0.0f; //RESET THE TIMER

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                vehicle = null;
            }
        }

        private void LookAt(Transform t)
        {
            VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = t;
            VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = t;
        }
        #endregion

        private void Start()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            var free_look = VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>();
            var virtual_camera = VirtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();

            if(free_look != null)
            {
                //ASSIGN THE CAMERA THE INPUT AXIS FROM THE INPUT CONTROLLER
                VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = ic.CameraInputHorizontal;
                VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = ic.CameraInputVertical;
            }
            if(virtual_camera != null)
            {
                var pov_camera = virtual_camera.GetCinemachineComponent<Cinemachine.CinemachinePOV>();

                pov_camera.m_HorizontalAxis.m_InputAxisName = ic.CameraInputHorizontal;
                pov_camera.m_VerticalAxis.m_InputAxisName = ic.CameraInputVertical;
                pov_camera.m_HorizontalAxis.m_InvertAxis = ic.InvertCameraHorizontal;
                pov_camera.m_VerticalAxis.m_InvertAxis = ic.InvertCameraVertical;
            }

            if (PlayerConfig == null)
            {
                PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
            }

            _p = Instantiate(PlayerConfig);
            
            CmdSetHealth(_p.MaxHealth);
            CmdSetMaxHealth(_p.MaxHealth);
            CmdSetisDead(false);
            CmdSetDriving(false);

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

            if(isDead)
            {
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
                //UpdatePlayerUI();
            }
        }

        private void LateUpdate()
        {
            if (isDead)
            {
                model.SetActive(false);
            }
            else
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
}
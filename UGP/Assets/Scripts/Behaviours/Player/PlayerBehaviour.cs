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
        private Transform lookat_override;
        public Player PlayerConfig;
        [HideInInspector] public Player _p;

        public VehicleBehaviour vehicle;
        public float TimeToExitVehicle;
        public float TimeToDestroyRagdoll = 10.0f;
        private float exitTimer = 0.0f;
        public NetworkUserControl ic;
        public PlayerInteractionBehaviour interaction;

        public Animator ani;

        #region UI
        public GameObject playerUI;
        public Slider HealthSlider;
        public Text PlayerNameText;
        #endregion

        #region SYNCED_VARIABLES
        [SyncVar(hook = "OnPlayerHealthChange")] public float playerHealth;
        [SyncVar(hook = "OnisDeadChange")] public bool isDead;
        [SyncVar] private bool hasInstantiatedRagdoll;
        [SyncVar] public bool isDriving;
        [SyncVar] public Color vehicleColor;
        [SyncVar] private float max_health;
        #endregion

        #region SYNCVAR_HOOK_FUNCTIONS
        private void OnPlayerHealthChange(float healthChange)
        {
            playerHealth = Mathf.Clamp(healthChange, 0.0f, max_health);

            if (playerHealth <= 0.0f)
            {
                isDead = true;
            }
        }
        private void OnisDeadChange(bool isDead)
        {
            if(isDead)
            {
                CmdSpawnRagdoll();
            }
        }
        #endregion

        #region COMMAND_FUNCTIONS
        [Command] public void CmdTakeHealth(float healthTaken)
        {
            playerHealth += healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, _p.MaxHealth);
        }
        [Command] public void CmdTakeDamage(float healthTaken)
        {
            playerHealth -= healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, _p.MaxHealth);

            if (playerHealth <= 0.0f)
            {
                isDead = true;
            }
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
            lookat_override = ragdoll.transform;
            NetworkServer.Spawn(ragdoll);
            Destroy(ragdoll, TimeToDestroyRagdoll);
        }
        #endregion

        #region CLIENTRPC_FUNCTIONS
        [ClientRpc] private void RpcPlayerDeath()
        {
            isDead = true;
            isDriving = false;
            model.SetActive(false);

            //if (!hasInstantiatedRagdoll)
            //{
            //    var ragdoll = Instantiate(RagDoll, transform.position, transform.rotation);
            //    NetworkServer.Spawn(ragdoll);
            //    hasInstantiatedRagdoll = true;
            //}
        }
        #endregion

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

        public void UpdatePlayerUI()
        {
            HealthSlider.value = Mathf.Clamp(playerHealth, 0.0f, max_health);
        }

        private void Server_UpdatePlayer()
        {
            if (playerHealth <= 0.0f)
            {
                RpcPlayerDeath();
                Debug.Log("Player DEAD");
            }
        }

        //public override void OnStartLocalPlayer()
        //{
        //    if (PlayerConfig == null)
        //    {
        //        PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
        //    }

        //    _p = Instantiate(PlayerConfig);

        //    playerHealth = _p.MaxHealth;
        //    max_health = _p.MaxHealth;
        //    HealthSlider.maxValue = max_health;
        //    PlayerNameText.text = gameObject.name;
        //    isDead = false;
        //    hasInstantiatedRagdoll = false;
        //    isDriving = false;
        //}

        private void Start()
        {
            if (!isLocalPlayer)
            {
                //if (isServer)
                //{
                //    if (PlayerConfig == null)
                //    {
                //        PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
                //    }

                //    _p = Instantiate(PlayerConfig);

                //    playerHealth = _p.MaxHealth;
                //    max_health = _p.MaxHealth;
                //    HealthSlider.maxValue = max_health;
                //    PlayerNameText.text = gameObject.name;
                //    isDead = false;
                //    hasInstantiatedRagdoll = false;
                //    isDriving = false;
                //}

                VirtualCamera.SetActive(false);
                return;
            }

            if (PlayerConfig == null)
            {
                PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
            }

            _p = Instantiate(PlayerConfig);

            playerHealth = _p.MaxHealth;
            max_health = _p.MaxHealth;
            HealthSlider.maxValue = max_health;
            PlayerNameText.text = gameObject.name;
            isDead = false;
            hasInstantiatedRagdoll = false;
            isDriving = false;

            if (ani == null)
            {
                ani = GetComponent<Animator>();
            }
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                //Server_UpdatePlayer();
            }

            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                CmdTakeDamage(10);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                CmdTakeHealth(10);
            }

            if (isDead)
            {
                VirtualCamera.SetActive(true);
                //VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().Follow = lookat_override;
                VirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = lookat_override;
                ic.enabled = false;

                interaction.enabled = false;
                playerUI.SetActive(false);
                //PlayerDeadCanvas.SetActive(true);
                //Cursor.enabled = true;
                return;
            }
            else
            {
                if (isDriving)
                {
                    VirtualCamera.SetActive(false);
                    ic.enabled = false;
                    interaction.enabled = false;
                    playerUI.SetActive(false);

                    ani.SetFloat("Walk", 0.0f);

                    ExitVehicleWithTimer(); //EXIT THE VEHICLE
                }
                else
                {
                    VirtualCamera.SetActive(true);
                    ic.enabled = true;
                    interaction.enabled = true;
                    UpdatePlayerUI();
                    playerUI.SetActive(true);
                }
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
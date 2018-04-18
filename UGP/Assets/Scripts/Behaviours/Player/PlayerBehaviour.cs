using System.Linq;
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

        public Transform Center;
        public VehicleBehaviour vehicle;
        public float TimeToExitVehicle;
        public float TimeToDestroyRagdoll = 10.0f;
        private float exitTimer = 0.0f;
        public NetworkUserControl ic;
        public PlayerInteractionBehaviour interaction;
        public PlayerUIBehaviour ui;
        public Vector3 Gravity = new Vector3(0, -4.0f, 0);

        public Animator ani;
        public List<Collider> colliders;

        public Rigidbody rb;

        #region SYNCED_VARIABLES
        [SyncVar(hook = "OnPlayerHealthChange")] public float playerHealth;
        [SyncVar(hook = "OnisActiveChange")] public bool isActive;
        [SyncVar(hook = "OnisDeadChange")] public bool isDead;
        [SyncVar(hook = "OnisDrivingChange")] public bool isDriving;
        [SyncVar(hook = "OnVehicleColorChange")] public Color vehicleColor;
        [SyncVar(hook = "OnMaxHealthAssign")] public float MaxHealth;
        [SyncVar(hook = "OnPlayerNameChange")] public string playerName;
        #endregion

        #region SYNCVAR_HOOK_FUNCTIONS
        private void OnPlayerHealthChange(float healthChange)
        {
            playerHealth = Mathf.Clamp(healthChange, 0.0f, 99999);

            if (playerHealth <= 0.0f)
            {
                isDead = true;
            }
        }
        private void OnMaxHealthAssign(float assignMaxHealth)
        {
            MaxHealth = assignMaxHealth;
        }
        private void OnisActiveChange(bool Active)
        {
            isActive = Active;
        }
        private void OnisDeadChange(bool Dead)
        {
            isDead = Dead;
            if(isDead)
            {
                var controller = GetComponent<CharacterController>();
                controller.detectCollisions = false;

                colliders.ForEach(collider =>
                {
                    collider.enabled = false;
                });

                var holding_item = interaction.isHolding;
                if(holding_item)
                {
                    interaction.DropItem();
                    interaction.item.isBeingHeld = false;
                    interaction.item.CmdSetHolding(false);
                    interaction.item.Drop();
                    interaction.CmdSetHolding(false, "");
                    ic.enabled = false;
                    interaction.enabled = false;
                }

                CmdSpawnRagdoll();
            }
        }
        private void OnisDrivingChange(bool Driving)
        {
            isDriving = Driving;
        }
        private void OnVehicleColorChange(Color color)
        {
            vehicleColor = color;
        }
        private void OnPlayerNameChange(string nameChange)
        {
            playerName = nameChange;
        }
        #endregion

        #region COMMAND_FUNCTIONS
        [Command] public void CmdTakeHealth(float healthTaken)
        {
            playerHealth += healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 99999);
        }
        [Command] public void CmdTakeDamage(NetworkIdentity attacker, float healthTaken)
        {
            playerHealth -= healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 99999);

            //Debug.Log("Player Take Damage");
            if (playerHealth <= 0.0f)
            {
                var server = FindObjectOfType<InGameNetworkBehaviour>();
                var localPlayer = GetComponent<NetworkIdentity>();
                server.PlayerKilledByPlayer(attacker, localPlayer);

                isDead = true;
                isDriving = false;
                ic.enabled = false;
                interaction.isHolding = false;
                interaction.enabled = false;
            }
        }
        [Command] public void CmdTakeDamage_Other(string attacker, float healthTaken)
        {
            playerHealth -= healthTaken;
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 99999);

            //Debug.Log("Player Take Damage");
            if (playerHealth <= 0.0f)
            {
                var server = FindObjectOfType<InGameNetworkBehaviour>();
                var localPlayer = GetComponent<NetworkIdentity>();
                server.PlayerKilledByOther(attacker, localPlayer);

                isDead = true;
                isDriving = false;
                ic.enabled = false;
                interaction.isHolding = false;
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
        [Command] public void CmdSetActive(bool active)
        {
            isActive = active;
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
            interaction.enabled = true;

            if(isLocalPlayer)
            {
                var holding_item = interaction.isHolding;
                if (holding_item)
                {
                    interaction.DropItem();
                    interaction.item.isBeingHeld = false;
                    interaction.item.Drop();
                    interaction.CmdSetHolding(false, "");
                    ic.enabled = false;
                    interaction.enabled = false;
                }

                playerHealth = _p.MaxHealth;
                isActive = true;
                isDead = false;
                isDriving = false;

                colliders.ForEach(collider =>
                {
                    collider.enabled = true;
                });

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                var controller = GetComponent<CharacterController>();
                controller.detectCollisions = true;

                CmdSetHealth(_p.MaxHealth);
                CmdSetMaxHealth(_p.MaxHealth);
                CmdSetActive(true);
                CmdSetisDead(false);
                CmdSetDriving(false);
                transform.position = pos;
                transform.rotation = rot;
                LookAt(transform);
            }
        }
        [ClientRpc] public void RpcSetActive(bool active)
        {
            if(isLocalPlayer)
            {
                CmdSetActive(active);
                isActive = active;
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

        public void ServerRespawn(Transform spawnPoint)
        {
            if (PlayerConfig == null)
            {
                PlayerConfig = Resources.Load("Assets//Resources//ScriptableObjects//Players//BasicPlayer") as Player;
            }

            _p = Instantiate(PlayerConfig);

            //playerHealth = _p.MaxHealth;
            isDead = false;
            isDriving = false;

            RpcRespawn(spawnPoint.position, spawnPoint.rotation);
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

        public void RemovePlayerFromVehicle()
        {
            CmdSetDriving(false);
            isDriving = false;
            var vehicleIdentity = vehicle.GetComponent<NetworkIdentity>();
            interaction.CmdSetVehicleActive(false, vehicleIdentity);
            interaction.CmdSetPlayerInSeat(false, vehicleIdentity);

            //vehicle.CmdRemovePlayer();
            vehicle.seatedPlayer = null;
            CmdRemoveVehicleAuthority(vehicleIdentity);

            exitTimer = 0.0f; //RESET THE TIMER

            //var rb = GetComponent<Rigidbody>();
            //rb.velocity = Vector3.zero;
            //rb.MovePosition(vehicle.seat.position);
            //rb.MoveRotation(vehicle.seat.rotation);

            transform.position = vehicle.seat.position;
            transform.rotation = vehicle.seat.rotation;

            vehicle = null;
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
                isDriving = false;
                CmdSetDriving(false);
                var vehicleIdentity = vehicle.GetComponent<NetworkIdentity>();
                interaction.CmdSetVehicleActive(false, vehicleIdentity);
                interaction.CmdSetPlayerInSeat(false, vehicleIdentity);

                //vehicle.CmdRemovePlayer();
                vehicle.seatedPlayer = null;
                CmdRemoveVehicleAuthority(vehicleIdentity);

                exitTimer = 0.0f; //RESET THE TIMER

                //var rb = GetComponent<Rigidbody>();
                //rb.velocity = Vector3.zero;
                //rb.MovePosition(vehicle.seat.position);
                //rb.MoveRotation(vehicle.seat.rotation);

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

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {   
                VirtualCamera.SetActive(false);
                //CHARACTERCONTROLLER COLLISION CALLBACKS
                var rb = gameObject.AddComponent<Rigidbody>();

                if(isClient)
                {
                    var playercontroller = GetComponent<ShannonSharpePlayerController>();
                    //Destroy(playercontroller);
                    //playercontroller.controller = null;
                    //playercontroller.enabled = false;
                    //var character_controller = GetComponent<CharacterController>();
                    //Destroy(character_controller);
                }

                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.useGravity = false;
                //rb.isKinematic = true;
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
            CmdSetActive(true);
            CmdSetisDead(false);
            CmdSetDriving(false);

            if (ani == null)
            {
                ani = GetComponent<Animator>();
            }

            colliders = GetComponents<Collider>().ToList();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                VirtualCamera.SetActive(false);
                return;
            }

            if (isDead)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            if (isDriving)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                VirtualCamera.SetActive(false);
                ic.enabled = false;
                interaction.enabled = false;

                ani.SetFloat("Walk", 0.0f);

                transform.position = vehicle.seat.position;
                transform.rotation = vehicle.seat.rotation;

                ExitVehicleWithTimer(); //EXIT THE VEHICLE
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                VirtualCamera.SetActive(true);
                ic.enabled = true;
                interaction.enabled = true;
                
                //PICKUP ITEM
                //CHANGE THIS TO THE INPUTCONTROLLER.BUTTONINPUTYOUNEED
                if(Input.GetKeyDown(KeyCode.F))
                {
                    colliders.ForEach(collider =>
                    {
                        if (collider.CompareTag("Hand"))
                        {
                            collider.enabled = true;
                        }
                    });

                    if (!interaction.isHolding)
                    {
                        interaction.PickUpItem();
                    }
                }

                //DROP ITEM
                //CHANGE THIS TO THE INPUTCONTROLLER.BUTTONINPUTYOUNEED
                if (Input.GetKeyDown(KeyCode.RightAlt))
                {
                    if(interaction.isHolding)
                    {
                        colliders.ForEach(collider =>
                        {
                            if (collider.CompareTag("Hand"))
                            {
                                collider.enabled = false;
                            }
                        });

                        interaction.DropItem();
                        interaction.item.Drop();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if(isActive)
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
                        var controller = GetComponent<CharacterController>();
                        controller.Move(Gravity);

                        model.SetActive(true);
                    }
                }
            }
            else
            {
                model.SetActive(false);
            }
        }
    }
}
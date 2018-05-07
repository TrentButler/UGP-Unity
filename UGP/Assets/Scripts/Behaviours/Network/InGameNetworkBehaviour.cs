using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGP
{
    //NEEDS WORK
    public class InGameNetworkBehaviour : NetworkBehaviour
    {
        public List<GameObject> VehiclePrefabs;
        public List<GameObject> ItemPrefabs;
        public List<NetworkStartPosition> PlayerStartPositions = new List<NetworkStartPosition>();

        public Transform OriginVehicleSpawn;

        public float vehiclePositionOffset;
        public float ItemPositionOffset = 5.0f;
        private bool spawnOnPlayerCount;
        [Range(1, 500)] public int TextCountLimit = 200;

        public NetworkUIBehaviour networkUI;
        public LANDirectConnect lanDirectConnect;

        #region ServerCamera
        private GameObject server_camera;
        public float cameraSpeed = 2.5f;
        public float cameraTranslationSpeed = 10.0f;

        private void FreeLookCamera()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            var move_vector = new Vector3(h, 0, v);
            server_camera.transform.Translate(move_vector);

            #region MouseLook
            //CALCULATE AN MOUSE DELTA
            //DERIVE AN DIRECTION
            //LERP BETWEEN THE CURRENT CAMERA ROTATION TO AN NEW CAMERA ROTATION
            if (Input.GetMouseButton(1))
            {
                var deltaX = Input.GetAxis("Mouse X"); //GET THE MOUSE DELTA X
                var deltaY = Input.GetAxis("Mouse Y"); //GET THE MOUSE DELTA Y

                Vector3 rotX = new Vector3(-deltaY, 0, 0);
                Vector3 rotY = new Vector3(0, deltaX, 0);

                Quaternion rot = Quaternion.Euler(rotX); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                server_camera.transform.rotation = Quaternion.Slerp(server_camera.transform.rotation, server_camera.transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

                rot = Quaternion.Euler(rotY); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                server_camera.transform.rotation = Quaternion.Slerp(server_camera.transform.rotation, server_camera.transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            }

            //TRANSLATE THE CAMERA BASED ON CLICKDRAG DELTA
            if (Input.GetMouseButton(2))
            {
                var dX = Input.GetAxis("Mouse X"); //GET THE DELTA MOUSE X
                var dY = Input.GetAxis("Mouse Y"); //GET THE DELTA MOUSE Y

                Vector3 trans = new Vector3(-(dX * cameraTranslationSpeed), -(dY * cameraTranslationSpeed), 0); //DERIVE A TRANSLATION VECTOR
                server_camera.transform.Translate(trans); //APPLY THE TRANSLATION TO THE CAMERA'S TRANSFORM
            }

            //RESET THE CAMERA'S ROTATION
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var currentRot = server_camera.transform.rotation;
                currentRot[0] = 0.0f;
                currentRot[2] = 0.0f;

                server_camera.transform.rotation = currentRot;
            }

            //MOVE THE CAMERA EITHER FORWARD OR BACKWARD FROM MOUSE SCROLL
            var scrollDelta = Input.GetAxis("Mouse ScrollWheel"); //GET THE MOUSE SCROLL DELTA
            Vector3 translation = new Vector3(0, 0, scrollDelta * cameraSpeed); //DERIVE A TRANSLATION VECTOR
            server_camera.transform.Translate(translation); //TRANSLATE THE CAMERA'S TRANSFORM
            #endregion
        }
        #endregion

        [SyncVar(hook = "OnPreRoundTimerChange")] [Range(1.0f, 999999.0f)] public float PreMatchTimer;
        private float original_prematchtimer;

        [SyncVar(hook = "OnStartPositionIndexChange")] public int StartPositionIndex = 0;

        [SyncVar(hook = "OnScoreboardTextChange")] public string scoreboardText;
        public Text scoreboard;
        public Text preroundtimer;

        private void OnPreRoundTimerChange(float timerChange)
        {
            PreMatchTimer = timerChange;
        }
        private void OnScoreboardTextChange(string textChange)
        {
            if (scoreboardText.Length > TextCountLimit)
            {
                scoreboardText = "";
            }
            else
            {
                scoreboardText += textChange;
            }

            //CmdScoreboardTextChange(scoreboardText);
        }
        private void OnStartPositionIndexChange(int indexChange)
        {
            StartPositionIndex = indexChange;
        }

        public void PlayerHitPlayer(NetworkIdentity localPlayer, NetworkIdentity otherPlayer)
        {
            if (!isServer)
            {
                return;
            }

            var localPlayerName = localPlayer.GetComponent<PlayerBehaviour>().playerName;
            var otherPlayerName = otherPlayer.GetComponent<PlayerBehaviour>().playerName;

            var localPlayerHealth = localPlayer.GetComponent<PlayerBehaviour>().playerHealth;
            var otherPlayerHealth = otherPlayer.GetComponent<PlayerBehaviour>().playerHealth;

            string localPlayerInfo = localPlayerName + " HP: " + localPlayerHealth;
            string otherPlayerInfo = localPlayerName + " HP: " + otherPlayerHealth;

            scoreboardText += localPlayerInfo + " HIT " + otherPlayerInfo + "\n";
        }
        public void VehicleHitPlayer(NetworkIdentity vehicle, NetworkIdentity player)
        {
            if (!isServer)
            {
                return;
            }

            var vehicleName = vehicle.gameObject.name;
            var playerName = player.GetComponent<PlayerBehaviour>().playerName;

            scoreboardText += playerName + " HIT BY A " + vehicleName + "\n";
        }

        public void PlayerShotByPlayer(NetworkIdentity attacker, NetworkIdentity player, string weapon)
        {
            if (!isServer)
            {
                return;
            }

            var attackerName = attacker.GetComponent<PlayerBehaviour>().playerName;
            var playerName = player.GetComponent<PlayerBehaviour>().playerName;

            scoreboardText += attackerName + " SHOT " + playerName + "\n";
        }
        public void PlayerShotByOther(string attacker, NetworkIdentity player, string weapon)
        {
            if (!isServer)
            {
                return;
            }

            var playerName = player.GetComponent<PlayerBehaviour>().playerName;
            scoreboardText += attacker + " SHOT " + playerName + "\n";
        }

        public void PlayerKilledByPlayer(NetworkIdentity attacker, NetworkIdentity player)
        {
            if (!isServer)
            {
                return;
            }

            var attackerName = attacker.GetComponent<PlayerBehaviour>().playerName;
            var playerName = player.GetComponent<PlayerBehaviour>().playerName;

            scoreboardText += attackerName + " KILLED " + playerName + "\n";
        }
        public void PlayerKilledByOther(string attacker, NetworkIdentity player)
        {
            if (!isServer)
            {
                return;
            }

            var playerName = player.GetComponent<PlayerBehaviour>().playerName;
            scoreboardText += attacker + " KILLED " + playerName + "\n";
        }

        public void PlayerFinishRace(NetworkIdentity player, string Results)
        {
            if (!isServer)
            {
                return;
            }

            var playerName = player.GetComponent<PlayerBehaviour>().playerName;
            scoreboardText += playerName + Results;
        }

        [ClientRpc]
        public void RpcAssignObjectAuthority(NetworkIdentity objectIdentity)
        {
            var server_network_identity = GetComponent<NetworkIdentity>();
            var server_connection = server_network_identity.connectionToClient;

            var objectNetworkIdentity = objectIdentity;

            objectNetworkIdentity.AssignClientAuthority(server_connection);
        }
        [ClientRpc]
        public void RpcRemoveObjectAuthority(NetworkIdentity objectIdentity)
        {
            var server_network_identity = GetComponent<NetworkIdentity>();
            var server_connection = server_network_identity.connectionToClient;

            var objectNetworkIdentity = objectIdentity;

            objectNetworkIdentity.RemoveClientAuthority(server_connection);
        }

        public void RespawnPlayer(NetworkIdentity playerIdentity)
        {
            //RESPAWN THE PLAYER AT ONE OF THE NETWORKSTARTPOSITIONS
            //MAYBE TRY TO INVOKE THE PLAYERBEHAVIOUR'S START FUNCTION
            var spawnPoints = FindObjectsOfType<NetworkStartPosition>().ToList();
            var randomSpawn = Random.Range(0, spawnPoints.Count);
            var spawn = spawnPoints[randomSpawn];

            var player_behaviour = playerIdentity.GetComponent<PlayerBehaviour>();
            player_behaviour.OnRespawn(spawn.transform);
        }
        public void ServerRespawnPlayer(NetworkIdentity playerIdentity)
        {
            //RESPAWN THE PLAYER AT ONE OF THE NETWORKSTARTPOSITIONS
            //MAYBE TRY TO INVOKE THE PLAYERBEHAVIOUR'S START FUNCTION
            var spawnPoints = FindObjectsOfType<NetworkStartPosition>().ToList();
            var randomSpawn = Random.Range(0, spawnPoints.Count);
            var spawn = spawnPoints[randomSpawn];

            var player_behaviour = playerIdentity.GetComponent<PlayerBehaviour>();
            player_behaviour.ServerRespawn(spawn.transform);
        }
        public Transform GetPlayerSpawn()
        {
            if (StartPositionIndex > PlayerStartPositions.Count - 1)
            {
                StartPositionIndex = 0;
            }

            var spawn = PlayerStartPositions[StartPositionIndex].transform;
            StartPositionIndex += 1;
            //CmdStartPositionIndexChange(StartPositionIndex)
            return spawn;
        }

        [Command]
        public void CmdRestartPreMatchTimer()
        {
            Debug.Log("RESTART PRE-MATCH TIMER");
            PreMatchTimer = original_prematchtimer;
            //ADD A RPC CALL TO MAKE SURE THIS IS HAPPENING
        }
        public void RestartPreMatchTimer()
        {
            PreMatchTimer = original_prematchtimer;
        }

        private void SpawnVehiclesOnPlayerCount()
        {
            //SPAWN A VEHICLE FOR EACH PLAYER THAT IS CONNECTED

            var allPlayers = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();

            //var allPlayers = NetworkManager.singleton.numPlayers;
            var allVehicles = GameObject.FindObjectsOfType<VehicleBehaviour>().ToList();

            int playerCount = 0;
            int vehicleCount = 0;

            if (allPlayers != null)
                playerCount = allPlayers.Count;

            if (allVehicles != null)
                vehicleCount = allVehicles.Count;

            int vehiclesNeeded = Mathf.Abs(playerCount - vehicleCount);

            for (int i = 0; i < vehiclesNeeded; i++)
            {
                var vehicle = VehiclePrefabs[0];
                var pos = NetworkManager.singleton.GetStartPosition();
                var v = Instantiate(vehicle, pos.position, pos.rotation);
                NetworkServer.Spawn(v);
            }
        }

        public void SpawnAllVehicles()
        {
            for (int i = 0; i < VehiclePrefabs.Count; i++)
            {
                var spawn_position = OriginVehicleSpawn.position;
                spawn_position.z = spawn_position.z + (i * vehiclePositionOffset);

                var v = Instantiate(VehiclePrefabs[i], spawn_position, OriginVehicleSpawn.rotation);
                NetworkServer.Spawn(v);
            }
        }

        public void SpawnAllVehiclesWithAllItems()
        {
            for (int i = 0; i < VehiclePrefabs.Count; i++)
            {
                var spawn_position = OriginVehicleSpawn.position;
                spawn_position.z = spawn_position.z + (i * vehiclePositionOffset);

                var v = Instantiate(VehiclePrefabs[i], spawn_position, OriginVehicleSpawn.rotation);
                NetworkServer.Spawn(v);

                for (int j = 0; j < ItemPrefabs.Count; j++)
                {
                    var item_spawn = spawn_position;
                    item_spawn.x = item_spawn.x - ((j + 1) * ItemPositionOffset);

                    var item = Instantiate(ItemPrefabs[j], item_spawn, OriginVehicleSpawn.rotation);
                    NetworkServer.Spawn(item);
                }
            }
        }

        public void ToggleVehicleSpawning()
        {
            if (spawnOnPlayerCount == true)
            {
                spawnOnPlayerCount = false;
            }
            else
            {
                spawnOnPlayerCount = true;
            }
        }

        public void ClearRagdolls()
        {
            var allragdoll = GameObject.FindGameObjectsWithTag("Ragdoll").ToList();
            for (int i = 0; i < allragdoll.Count; i++)
            {
                NetworkServer.Destroy(allragdoll[i]);
            }
        }

        public void ClearScoreboard()
        {
            //scoreboardText = "";
            //RpcScoreboardTextChange(scoreboardText);
        }

        public void ResetServer()
        {
            NetworkServer.ClearLocalObjects();
            NetworkServer.Reset();
            NetworkServer.Shutdown();
        }

        public void SpawnBuildings()
        {
            var allBuildings = FindObjectsOfType<BuildingBehaviour>().ToList();
            allBuildings.ForEach(building =>
            {
                building.ServerSpawn(this);
            });
        }

        public void Spawn(GameObject go, Vector3 pos, Quaternion rot)
        {
            var _go = Instantiate(go, pos, rot);
            NetworkServer.Spawn(_go);
        }
        public void Spawn(GameObject go)
        {
            NetworkServer.Spawn(go);
        }

        public void Server_Destroy(GameObject go)
        {
            NetworkServer.Destroy(go);
        }

        public void Server_ChangeScene(string scene)
        {
            NetworkManager.singleton.ServerChangeScene(scene);
        }
        public void Server_ChangeSceneLocal(string scene)
        {
            SceneManager.LoadScene(scene);
            //NetworkManager.singleton.ServerChangeScene(scene);
        }

        public void Server_LANDisconnectAll()
        {
            var lanManager = FindObjectOfType<LANNetworkManager>();
            lanManager.DisconnectAll();
        }

        [ClientRpc] public void RpcServer_Disconnect(NetworkIdentity player, string scene)
        {
            if (player.isLocalPlayer)
            {
                //NetworkManager.singleton.StopClient();
                var playerUIBehaviour = player.GetComponent<PlayerUIBehaviour>();
                playerUIBehaviour.GotoScene(scene);
            }
        }

        public void ClearAllItems()
        {
            var allItems = FindObjectsOfType<ItemBehaviour>().ToList();
            allItems.ForEach(item => { Server_Destroy(item.gameObject); });
        }
        public void ClearAllVehicles()
        {
            var allVehicles = FindObjectsOfType<VehicleBehaviour>().ToList();
            allVehicles.ForEach(vehicle => { Server_Destroy(vehicle.gameObject); });
        }

        private void Start()
        {
            if (!isServer)
            {
                return;
            }

            original_prematchtimer = PreMatchTimer;
            spawnOnPlayerCount = false;

            server_camera = Camera.main.gameObject;
            SpawnBuildings();

            PlayerStartPositions = FindObjectsOfType<NetworkStartPosition>().ToList();
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                //PreMatchTimer -= Time.deltaTime;

                //var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
                //allPlayers.ForEach(player =>
                //{
                //    if (PreMatchTimer > 0.0f)
                //    {
                //        player.RpcSetUserControl(false);
                //    }
                //    else
                //    {
                //        player.RpcSetUserControl(true);
                //    }
                //});
            }

            //preroundtimer.text = "";
            //preroundtimer.text = "BEGIN IN: " + PreMatchTimer.ToString();

            #region OLD
            //FreeLookCamera();
            //if (spawnOnPlayerCount)
            //{
            //    SpawnVehiclesOnPlayerCount();
            //} 
            #endregion
        }

        private void LateUpdate()
        {
            if (networkUI == null)
            {
                networkUI = FindObjectOfType<NetworkUIBehaviour>();
            }

            if (!isServer)
            {
                networkUI.serverUIActive = false;
                networkUI.clientUIActive = false;
                networkUI.ipUIActive = false;
            }

            if (isServer)
            {
                networkUI.clientUIActive = false;
            }

            scoreboard.text = scoreboardText;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InGameNetworkBehaviour))]
    public class InspectorInGameNetworkBehaviour : Editor
    {
        GUIStyle header = new GUIStyle();

        public override void OnInspectorGUI()
        {
            var mytarget = target as InGameNetworkBehaviour;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("SPAWN ALL VEHICLES"))
            {
                mytarget.SpawnAllVehicles();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("TOGGLE SPAWN VEHICLE PER PLAYER"))
            {
                mytarget.ToggleVehicleSpawning();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("SPAWN BUILDINGS"))
            {
                mytarget.SpawnBuildings();
            }
        }
    }
#endif
}
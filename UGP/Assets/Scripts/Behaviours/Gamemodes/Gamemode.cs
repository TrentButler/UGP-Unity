using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UGP
{
    public abstract class Gamemode : NetworkBehaviour
    {
        //WHAT EVERY GAMEMODE WILL CONSIST OF
        //PRE-MATCH TIMER
        //MATCH TIMER
        //COLLECTION OF ALL CONNECTED PLAYERS

        [Range(0, 999999)] public float PreMatchTimer;
        [Range(0, 999999)] public float PostMatchTimer;
        [Range(0, 999999)] public float MatchTimer;
        [SyncVar(hook = "OnWinConditionChange")] public bool WinCondition;
        [SyncVar(hook = "OnLoseConditionChange")] public bool LoseCondition;
        [SyncVar(hook = "OnMatchBegunChange")] public bool MatchBegun;
        public InGameNetworkBehaviour netCompanion;        
        public bool destroyUI, stopServer, restartServer, respawnAll, destroyNetManager, localSceneSwitch;
        public List<PlayerBehaviour> Players = new List<PlayerBehaviour>();

        private void OnWinConditionChange(bool winChange)
        {
            WinCondition = winChange;
        }
        private void OnLoseConditionChange(bool loseChange)
        {
            LoseCondition = loseChange;
        }
        private void OnMatchBegunChange(bool begunChange)
        {
            MatchBegun = begunChange;

            if(MatchBegun)
            {
                TogglePlayerControl(true);
            }
            else
            {
                TogglePlayerControl(false);
            }
        }

        public abstract bool CheckWinCondition();
        public abstract bool CheckLoseCondition();
        public abstract void OnWinCondition();
        public abstract void OnLoseCondition();
        public abstract void GameLoop();
        public abstract void Initialize();
        public abstract void RestartPreMatchTimer();

        protected void SetMatchBegun(bool begun)
        {
            MatchBegun = begun;
        }
        protected void TogglePlayerControl(bool control)
        {
            Players.ForEach(player =>
            {
                player.RpcSetUserControl(control);
            });
        }
        protected void EndMatchLAN(string scene)
        {
            var netManager = GameObject.FindGameObjectWithTag("NetworkManager");
            var network_manager = netManager.GetComponent<NetworkManager>();
            var directConnect = netManager.GetComponent<LANDirectConnect>();
            
            if (destroyUI)
            {
                //DESTROY UI
                var networkUI = network_manager.GetComponent<NetworkUIBehaviour>();
                networkUI.DestroyUI();
            }

            if (stopServer)
            {
                //STOP THE SERVER
                directConnect.StopServer();
            }

            if (restartServer)
            {
                //RESTART THE SERVER
                directConnect.RestartServer();
                directConnect.StopServer();
            }

            if (respawnAll)
            {
                directConnect.RespawnAllPlayers();
            }
            
            if (destroyNetManager)
            {
                Destroy(netManager);
            }

            if (localSceneSwitch)
            {
                netCompanion.Server_ChangeSceneLocal(scene);
            }
            else
            {
                netCompanion.Server_ChangeScene(scene);
            }
        }
        protected void EndMatchINT(string scene)
        {
            var netManager = GameObject.FindGameObjectWithTag("NetworkManager");
            var network_manager = netManager.GetComponent<NetworkManager>();
            var directConnect = netManager.GetComponent<DirectConnect>();

            if (destroyUI)
            {
                //DESTROY UI
                var networkUI = network_manager.GetComponent<NetworkUIBehaviour>();
                networkUI.DestroyUI();
            }

            if (stopServer)
            {
                //STOP THE SERVER
                directConnect.StopServer();
            }

            if (restartServer)
            {
                //RESTART THE SERVER
                directConnect.RestartServer();
                directConnect.StopServer();
            }

            if (respawnAll)
            {
                //directConnect.RespawnAllPlayers();
            }

            if (destroyNetManager)
            {
                Destroy(netManager);
            }

            if (localSceneSwitch)
            {
                netCompanion.Server_ChangeSceneLocal(scene);
            }
            else
            {
                netCompanion.Server_ChangeScene(scene);
            }
        }
    }
}
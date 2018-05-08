using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace UGP
{
    public class LANNetworkManager : NetworkManager
    {
        //REFRENCE TO THE PREMATCH TIMER
        public InGameNetworkBehaviour net_companion;
        public GamemodeManager gamemode_manager;
        public List<GameObject> PlayerPrefabs = new List<GameObject>();
        public List<NetworkConnection> allConnections = new List<NetworkConnection>();
        public int playerIndex = 0;

        public override void OnClientConnect(NetworkConnection conn)
        {
            short controllerID = 0;

            //SEND ALL RELEVANT INFOMATION ABOUT THE PLAYER FOR SPAWNING HERE

            IntegerMessage prefabIndexMessage = new IntegerMessage(playerIndex); 

            ClientScene.AddPlayer(conn, controllerID, prefabIndexMessage);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader reader)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            //GET MESSAGE FROM CLIENT FOR WHICH PLAYER PREFAB TO SPAWN
            if (reader != null)
            {
                var message_stream = reader.ReadMessage<IntegerMessage>();

                playerIndex = message_stream.value;
            }

            short controller_id = 0;

            var spawn = GetStartPosition();

            var spawn_player = Instantiate(PlayerPrefabs[playerIndex], spawn.position, spawn.rotation);

            var playerBehaviour = spawn_player.GetComponent<PlayerBehaviour>();
            playerBehaviour.playerName = RandomUserNames.GetUsername();
            playerBehaviour.vehicleColor = RandomUserNames.GetColor();

            NetworkServer.AddPlayerForConnection(conn, spawn_player, controller_id);
            gamemode_manager.gamemode.Players.Add(playerBehaviour);
            gamemode_manager.gamemode.RestartPreMatchTimer();

            #region OLD
            //allConnections.Add(conn);
            ////ADD THIS PLAYER TO THE GAMEMODE'S PLAYER LIST
            //var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            //allPlayers.ForEach(player =>
            //{
            //    var playerNetIdentity = player.GetComponent<NetworkIdentity>();
            //    var playerConnection = playerNetIdentity.connectionToClient.connectionId;
            //    if(playerConnection == conn.connectionId)
            //    {
            //        player.playerName = RandomUserNames.GetUsername();
            //        player.vehicleColor = RandomUserNames.GetColor();
            //        //ADD THIS PLAYER TO THE GAMEMODES LIST OF PLAYERS
            //        gamemode_manager.gamemode.Players.Add(player);
            //    }

            //}); 
            #endregion
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            base.OnServerRemovePlayer(conn, player);

            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(p =>
            {
                var playerNetIdentity = p.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if (playerConnection == conn.connectionId)
                {
                    //ADD THIS PLAYER TO THE GAMEMODES LIST OF PLAYERS
                    gamemode_manager.gamemode.Players.Remove(p);
                }

            });
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(p =>
            {
                var playerNetIdentity = p.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if (playerConnection == conn.connectionId)
                {
                    //ADD THIS PLAYER TO THE GAMEMODES LIST OF PLAYERS
                    gamemode_manager.gamemode.Players.Remove(p);
                }
            });
        }

        public void DisconnectAll()
        {
            for (int i = 0; i < allConnections.Count; i++)
            {
                allConnections[i].Dispose();
                allConnections[i].Disconnect();
            }
        }
    }
}
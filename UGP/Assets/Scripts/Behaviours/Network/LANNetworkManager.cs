using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class LANNetworkManager : NetworkManager
    {
        //REFRENCE TO THE PREMATCH TIMER
        public InGameNetworkBehaviour net_companion;
        public GamemodeManager gamemode_manager;
        public List<NetworkConnection> allConnections = new List<NetworkConnection>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            allConnections.Add(conn);

            //ADD THIS PLAYER TO THE GAMEMODE'S PLAYER LIST
            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(player =>
            {
                var playerNetIdentity = player.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if(playerConnection == conn.connectionId)
                {
                    //ADD THIS PLAYER TO THE GAMEMODES LIST OF PLAYERS
                    gamemode_manager.gamemode.Players.Add(player);
                }
                
            });

            gamemode_manager.gamemode.RestartPreMatchTimer();
            //net_companion.RestartPreMatchTimer();
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
            for(int i = 0; i < allConnections.Count; i++)
            {
                allConnections[i].Dispose();
                allConnections[i].Disconnect();
            }
        }
    }
}
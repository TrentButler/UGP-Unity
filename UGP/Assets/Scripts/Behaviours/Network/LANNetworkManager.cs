using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace UGP
{
    public struct Player_Dress
    {
        public int prefab_index;
        public string player_name;
        public Color skin_color;
        public Color shirt_color;
        public Color pants_color;
    };
    public struct Vehicle_Dress
    {
        public Color Part0Color;
        public Color Part1Color;
        public Color Part2Color;
        public Color Part3Color;
        public Color Part4Color;
        public Color Part5Color;
        public Color Part6Color;
        public Color Part7Color;
    }

    public class LANNetworkManager : NetworkManager
    {
        //REFRENCE TO THE PREMATCH TIMER
        public InGameNetworkBehaviour net_companion;
        public GamemodeManager gamemode_manager;
        public List<GameObject> PlayerPrefabs = new List<GameObject>();
        public List<NetworkConnection> allConnections = new List<NetworkConnection>();
        public int player_Index = 0;
        public Player_Dress player_Dress;
        public Vehicle_Dress vehicle_Dress;
        
        public class NetworkMessage : MessageBase
        {
            public Player_Dress player_dress;
            public Vehicle_Dress vehicle_dress;
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            short controllerID = 0;

            //SEND ALL RELEVANT INFOMATION ABOUT THE PLAYER FOR SPAWNING HERE

            NetworkMessage playerInfo = new NetworkMessage();
            var playerDress = FindObjectOfType<PlayerDress>();
            var vehicleDress = FindObjectOfType<VehicleDress>();
            if(playerDress != null)
            {
                playerInfo.player_dress.prefab_index = playerDress.PlayerIndex;
                playerInfo.player_dress.player_name = playerDress.PlayerName;
                playerInfo.player_dress.shirt_color = playerDress.ShirtColor;
                playerInfo.player_dress.skin_color = playerDress.SkinColor;
                playerInfo.player_dress.pants_color = playerDress.PantsColor;
                //playerDress.OnUse();
            }
            else
            {
                playerInfo.player_dress.prefab_index = Random.Range(0, PlayerPrefabs.Count);
                playerInfo.player_dress.player_name = RandomUserNames.GetUsername();
                playerInfo.player_dress.shirt_color = RandomUserNames.GetColor();
                playerInfo.player_dress.skin_color = RandomUserNames.GetColor();
                playerInfo.player_dress.pants_color = RandomUserNames.GetColor();
            }

            if (vehicleDress != null)
            {
                playerInfo.vehicle_dress.Part0Color = vehicleDress.Part0Color;
                playerInfo.vehicle_dress.Part1Color = vehicleDress.Part1Color;
                playerInfo.vehicle_dress.Part2Color = vehicleDress.Part2Color;
                playerInfo.vehicle_dress.Part3Color = vehicleDress.Part3Color;
                playerInfo.vehicle_dress.Part4Color = vehicleDress.Part4Color;
                playerInfo.vehicle_dress.Part5Color = vehicleDress.Part5Color;
                playerInfo.vehicle_dress.Part6Color = vehicleDress.Part6Color;
                playerInfo.vehicle_dress.Part7Color = vehicleDress.Part7Color;
            }
            else
            {
                playerInfo.vehicle_dress.Part0Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part1Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part2Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part3Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part4Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part5Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part6Color = RandomUserNames.GetColor();
                playerInfo.vehicle_dress.Part7Color = RandomUserNames.GetColor();
            }

            ClientScene.AddPlayer(conn, controllerID, playerInfo);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader reader)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            //GET MESSAGE FROM CLIENT FOR WHICH PLAYER PREFAB TO SPAWN
            if (reader != null)
            {
                var message_stream = reader.ReadMessage<NetworkMessage>();

                player_Dress = message_stream.player_dress;
                vehicle_Dress = message_stream.vehicle_dress;
                player_Index = player_Dress.prefab_index;
                //player_Index = message_stream.player_index;
            }

            //playerIndex = Random.Range(0, PlayerPrefabs.Count);

            short controller_id = 0;

            var spawn = GetStartPosition();

            var spawn_player = Instantiate(PlayerPrefabs[player_Index], spawn.position, spawn.rotation);

            var playerBehaviour = spawn_player.GetComponent<PlayerBehaviour>();
            var playerDressBehaviour = spawn_player.GetComponent<PlayerDressBehaviour>();
            playerDressBehaviour.Load(player_Dress, vehicle_Dress);

            //playerDress.SkinColor = RandomUserNames.GetColor();
            //playerDress.ShirtColor = RandomUserNames.GetColor();
            //playerDress.PantsColor = RandomUserNames.GetColor();

            playerBehaviour.playerName = player_Dress.player_name;

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
            for(int i = 0; i < allPlayers.Count; i++)
            {
                var p = allPlayers[i];
                var playerNetIdentity = p.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if (playerConnection == conn.connectionId)
                {
                    //REMOVE THIS PLAYER
                    gamemode_manager.gamemode.Players.Remove(p);
                    net_companion.Server_Destroy(p.gameObject);
                }
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                var p = allPlayers[i];
                var playerNetIdentity = p.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if (playerConnection == conn.connectionId)
                {
                    //REMOVE THIS PLAYER
                    gamemode_manager.gamemode.Players.Remove(p);
                    net_companion.Server_Destroy(p.gameObject);
                }
            }
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
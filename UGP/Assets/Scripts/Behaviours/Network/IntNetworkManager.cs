using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace UGP
{
    public class IntNetworkManager : NetworkManager
    {
        public string playerName = "";
        public Text PLAYERNAME;
        public Color playerColor = Color.white;

        //REFRENCE TO THE PREMATCH TIMER
        public InGameNetworkBehaviour net_companion;
        public GamemodeManager gamemode_manager;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);

            //ADD THIS PLAYER TO THE GAMEMODE'S PLAYER LIST
            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(player =>
            {
                var playerNetIdentity = player.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                if (playerConnection == conn.connectionId)
                {
                    player.playerName = playerName;
                    player.vehicleColor = playerColor;
                    //ADD THIS PLAYER TO THE GAMEMODES LIST OF PLAYERS
                    gamemode_manager.gamemode.Players.Add(player);
                }

            });

            gamemode_manager.gamemode.RestartPreMatchTimer();
            //net_companion.RestartPreMatchTimer();
        }



        public void LateUpdate()
        {
            playerName = PLAYERNAME.text;
        }
    }
}
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

        public Text PLAYERNAME;
        public PlayerInfo Info;
       
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
                    player.playerName = Info.PlayerName;
                    player.vehicleColor = Info.PlayerColor;
                }

            });

        }

        public override void OnStartClient(NetworkClient client)
        {
            //ADD THIS PLAYER TO THE GAMEMODE'S PLAYER LIST
            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(player =>
            {
                var playerNetIdentity = player.GetComponent<NetworkIdentity>();
                var playerConnection = playerNetIdentity.connectionToClient.connectionId;
                var conn = client.connection;
                if (playerConnection == conn.connectionId)
                {
                    player.playerName = Info.PlayerName;
                    player.vehicleColor = Info.PlayerColor;
                }
            });
        }

        public void LateUpdate()
        {
        
        }
    }

    public class PlayerNameManager : NetworkLobbyPlayer
    {


        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        public InputField nameInput;

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }
        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }



    }



}
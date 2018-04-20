using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
namespace UGP
{
    public class LANDirectConnect : NetworkManager
    {   
        public List<PlayerBehaviour> ListOfPlayers = new List<PlayerBehaviour>();
        
        //public void StartServer()
        //{
        //    Server.matchMaker.CreateMatch("Core", Server.matchSize, true, "", "", "", 0, 0, Server.OnMatchCreate);
        //}

       


        public void KillAllPlayers()
        {
            foreach (var players in ListOfPlayers)
            {
                players.CmdTakeDamage_Other("Admin", 9999999);
            }
        }

        public void StartClient()
        {
            if (AllMatches.Count <= 0)
                return;

            var matchPassword = "";
            var publicClientAddress = "";
            var privateClientAddress = "";
            var eloScoreForClient = 0;
            var requestDomain = 0;

            var match = AllMatches[0];
            Server.matchName = match.name;
            Server.matchMaker.JoinMatch(match.networkId,
                matchPassword,
                publicClientAddress,
                privateClientAddress,
                eloScoreForClient,
                requestDomain,
                Server.OnMatchJoined);

            var allClients = NetworkClient.allClients;
            ClientScene.AddPlayer(allClients[0].connection, 0);
        }

        public void RestartServer()
        {
            var currentScene = SceneManager.GetActiveScene();
            var scene_string = currentScene.name;

            NetworkManager.singleton.ServerChangeScene(scene_string);
        }
        
        void LateUpdate()
        {
            ListOfPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
        }
    }
}
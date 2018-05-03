using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
namespace UGP
{
    public class DirectConnect : MonoBehaviour
    {
        public GameObject PlayButton;
        public NetworkManager Server;
        public List<UnityEngine.Networking.Match.MatchInfoSnapshot> AllMatches = new List<UnityEngine.Networking.Match.MatchInfoSnapshot>();
        public int MatchCount;
        public List<PlayerBehaviour> ListOfPlayers = new List<PlayerBehaviour>();



        void Start()
        {

            if (Server == null)
                Server = FindObjectOfType<NetworkManager>();
            DisableServerPlayer();

        }
        public void StopServer()
        {
            Server.StopMatchMaker();
            Server.StopHost();
        }
        public void StartServer()
        {
            Server.matchMaker.CreateMatch("Core", Server.matchSize, true, "", "", "", 0, 0, Server.OnMatchCreate);
        }
        public void MasterServer()
        {
            Server.matchMaker.CreateMatch("Core", Server.matchSize, true, "", "", "", 0, 0, Server.OnMatchCreate);

        }
        public void KillAllPlayers()
        {
            foreach (var players in ListOfPlayers)
            {
                players.CmdTakeDamage_Other("Admin", 9999999);
            }
        }
        public struct MatchInfo
        {
            string matchPassword;
            string publicClientAddress;
            string privateClientAddress;
            int eloScoreForClient;
            int requestDomain;
        };

        public void DisableServerPlayer()
        {
            foreach (var players in ListOfPlayers)
            {
                if (players.isServer)
                {
                    //players.gameObject.SetActive(false);
                }
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
        public void ExitApplication()
        {
            Application.Quit();
        }
        public void RestartServer()
        {
            var currentScene = SceneManager.GetActiveScene();
            var scene_string = currentScene.name;
          
            NetworkManager.singleton.ServerChangeScene(scene_string);
        }

        public void OnMatchList(bool success, string extendedInfo, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
        {
            AllMatches = matches;
        }


        void Awake()
        {
            Server = GetComponent<NetworkManager>();
            Server.StartMatchMaker(); //enable the match maker
        }

        void Update()
        {
            Server.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);

            if (AllMatches.Count > 0)
            {
                if (PlayButton != null)
                {
                    PlayButton.SetActive(true);
                }
            }
            if (AllMatches.Count == 0)
            {
                if (PlayButton != null)
                {
                    PlayButton.SetActive(false);
                }
            }

            MatchCount = AllMatches.Count;
        }
        void LateUpdate()
        {
            ListOfPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
        }
    }
}
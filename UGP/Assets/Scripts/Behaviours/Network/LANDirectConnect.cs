using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UGP
{
    public class LANDirectConnect : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public LANNetworkManager Server;
        public List<PlayerBehaviour> ListOfPlayers = new List<PlayerBehaviour>();

        public InputField IPTEXT;
        public string server_ip;

        public void ExitApplication()
        {
            Application.Quit();
        }
        public void StartServer()
        {
            Server.StartServer();
        }
        public void StartClient()
        {
            Server.StartClient();
        }

        public void KillAllPlayers()
        {
            foreach (var players in ListOfPlayers)
            {
                players.CmdTakeDamage_Other("Admin", 9999999);
            }
        }

        public void RestartServer()
        {
            var currentScene = SceneManager.GetActiveScene();
            var scene_string = currentScene.name;

            NetworkManager.singleton.ServerChangeScene(scene_string);
        }

        void Awake()
        {
            Server = GetComponent<LANNetworkManager>();
        }

        void Start()
        {
            if (Server == null)
                Server = FindObjectOfType<LANNetworkManager>();

            Server.playerPrefab = PlayerPrefab;
        }

        void LateUpdate()
        {
            Server.networkAddress = IPTEXT.text;
            server_ip = Server.networkAddress;

            //if(Input.GetKeyDown(KeyCode.Keypad0))
            //{
            //    StartClient();
            //}
        }
    }
}
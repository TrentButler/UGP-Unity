﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class LANDirectConnect : MonoBehaviour
    {
        private List<PlayerBehaviour> ListOfPlayers = new List<PlayerBehaviour>();
        public GameObject PlayerPrefab;
        public LANNetworkManager Server;

        public void ExitApplication()
        {
            Application.Quit();
        }
        public void StartServer()
        {
            Server.StartServer();
        }
        public void StopServer()
        {
            //Server.StopHost();
            Server.StopServer();
        }
        public void RestartServer()
        {
            //FUNCTION TO RESTART THE SERVER
            //CREATE A EMPTY GAMEOBJECT WITH THE SCRIPT 'SERVERRESTART'
            //USE THE METHOD 'DontDestroyOnLoad' ON THE CREATED GAMEOBJECT
            var serverRestart = Instantiate(new GameObject());
            serverRestart.name = "ServerRestart";
            serverRestart.AddComponent<ServerRestart>();
            DontDestroyOnLoad(serverRestart);
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
            ListOfPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            //CHECK IF THE SERVER NEEDS TO BE STARTED
            //IF THE 'server_restart' OBJECT ISNT NULL, 
            //START THE SERVER
            var server_restart = FindObjectOfType<ServerRestart>();
            if(server_restart != null)
            {
                server_restart.Restart(this);
            }
        }
    }
}
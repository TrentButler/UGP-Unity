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
        private List<PlayerBehaviour> ListOfPlayers = new List<PlayerBehaviour>();
        public LANNetworkManager Server;

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

        public void MaxVehicleResources()
        {
            var allVehicles = FindObjectsOfType<VehicleBehaviour>().ToList();
            allVehicles.ForEach(vehicle =>
            {
                vehicle.CmdTakeHealth(99999);
                vehicle.CmdTakeAmmunition(999, 999, 999, 999);
            });
        }

        public void RespawnAllPlayers()
        {
            var allPlayers = FindObjectsOfType<PlayerBehaviour>().ToList();
            allPlayers.ForEach(player =>
            {
                var spawn = Server.GetStartPosition();
                player.RpcRespawn(spawn.position, spawn.rotation);
            });
        }

        void Awake()
        {
            Server = GetComponent<LANNetworkManager>();
        }

        void Start()
        {
            if (Server == null)
                Server = FindObjectOfType<LANNetworkManager>();

            //Server.playerPrefab = PlayerPrefab;
        }

        void LateUpdate()
        {
            Server.networkAddress = IPTEXT.text;
            server_ip = Server.networkAddress;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class PlayerSpawner : MonoBehaviour
    {
        public GameObject PlayerPrefab;

        // Use this for initialization
        void Start()
        {
            NetworkServer.RegisterHandler(MsgType.AddPlayer, OnPlayerConnect);
        }

        public void OnPlayerConnect(NetworkMessage networkMessage)
        {
            var net_companion = FindObjectOfType<InGameNetworkBehaviour>();
            var spawn = net_companion.GetPlayerSpawn();
            var player = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);

            var conn = networkMessage.conn;
            //var playerControlID = 0;

            NetworkServer.AddPlayerForConnection(conn, player, 0);
        }
    } 
}

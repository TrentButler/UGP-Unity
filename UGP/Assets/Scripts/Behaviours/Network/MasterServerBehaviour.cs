using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{

    public class MasterServerBehaviour : MonoBehaviour
    {
        public DirectConnect Server;

        public void StartServer()
        {
            Server.StartServer();
        }

        public void RestartServer()
        {
            Server.RestartServer();
        }
        // Use this for initialization
        void Start()
        {
            StartServer();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            //var server_restart = FindObjectOfType<ServerRestart>();
            //if(server_restart != null)
            //{
            //    server_restart.Restart(this);
            //}
        }
    }

}
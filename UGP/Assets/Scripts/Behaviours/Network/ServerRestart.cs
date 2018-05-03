using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class ServerRestart : MonoBehaviour
    {
        public void Restart(LANDirectConnect directConnect)
        {
            directConnect.StartServer();
            Destroy(gameObject);
        }
        public void Restart(DirectConnect directConnect)
        {
            directConnect.StartServer();
            Destroy(gameObject);
        }
    }
}
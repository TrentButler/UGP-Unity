using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class LANNetworkManager : NetworkManager
    {
        //REFRENCE TO THE PREMATCH TIMER
        public InGameNetworkBehaviour net_companion;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            net_companion.RestartPreMatchTimer();
        }
    }
}
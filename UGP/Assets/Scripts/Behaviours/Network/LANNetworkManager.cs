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

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            //INVOKE A METHOD THAT RESTARTS THE PREMATCH TIMER
            if(net_companion.enabled == false)
            {
                net_companion.enabled = true;
                net_companion.CmdRestartPreMatchTimer();
            }
            else
            {
                net_companion.CmdRestartPreMatchTimer();
            }
        }
    }
}
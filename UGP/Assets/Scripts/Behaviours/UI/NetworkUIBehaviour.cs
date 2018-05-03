using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class NetworkUIBehaviour : NetworkBehaviour
    {
        public GameObject canvas;
        public Text t;
        
        void Update()
        {
            if(isServer)
            {
                canvas.SetActive(true);
            }
            else
            {
                canvas.SetActive(false);
            }

            //var connections = NetworkServer.connections.ToList();
            //connections.ForEach(connection =>
            //{
            //    string dump = "";
            //    var ip = connection.address.ToString();
            //    dump += ip;
            //    dump += "\n";
            //    t.text = dump;
            //});
        }
    }
}
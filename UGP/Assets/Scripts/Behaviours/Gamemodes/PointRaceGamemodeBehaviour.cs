using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class PointRaceGamemodeBehaviour : NetworkBehaviour
    {
        public List<PlayerBehaviour> players;
        public Transform Finish;

        public InGameNetworkBehaviour net;
        public Text t;

        private void Start()
        {
            if(isServer)
            {
                players = FindObjectsOfType<PlayerBehaviour>().ToList();
                net.SpawnAllVehicles();
            }
        }

        private void FixedUpdate()
        {
            if(isServer)
            {
                //SORT THE LIST OF PLAYERS BY THEIR DISTANCE TO THE 'FINISH'
                var sorted_players = players.OrderBy(x => Vector3.Distance(x.transform.position, Finish.transform.position)).ToList();
                players = sorted_players;

                var first = "FIRST PLACE: " + players[0].name + "\n";
                var second = "SECOND PLACE: " + players[1].name + "\n";
                var third = "THRID PLACE: " + players[2].name + "\n";

                t.text = first + second + third;
            }
        }
    }
}
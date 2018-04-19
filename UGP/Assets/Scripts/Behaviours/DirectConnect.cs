using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
namespace UGP {
    public class DirectConnect : MonoBehaviour {

        public NetworkManager Server;
        public List<UnityEngine.Networking.Match.MatchInfoSnapshot> AllMatches = new List<UnityEngine.Networking.Match.MatchInfoSnapshot>();
        public int MatchCount;


        public void StartServer()
        {
            Server.matchMaker.CreateMatch("Core", Server.matchSize, true, "", "", "", 0, 0, Server.OnMatchCreate);
        }
        public void StartClient()
        {
            if(AllMatches.Count > 0)
            {
                var match = AllMatches[0];
                Server.matchName = match.name;
                Server.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, Server.OnMatchJoined);
            }
        }

        public void OnMatchList(bool success, string extendedInfo, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matches)
        {
            AllMatches = matches;
        }


        void Awake()
        {
            Server = GetComponent<NetworkManager>();
            Server.StartMatchMaker(); //enable the match maker
        }

        void Update()
        {
            if(AllMatches.Count <= 0)
            {
                Server.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
            }

            MatchCount = AllMatches.Count;
        }
    }
}
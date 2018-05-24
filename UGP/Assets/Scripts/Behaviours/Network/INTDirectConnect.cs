using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine;
namespace UGP
{
    public class INTDirectConnect : MonoBehaviour
    {
        public IntNetworkManager Server;
        public List<UnityEngine.Networking.Match.MatchInfoSnapshot> AllMatches = new List<UnityEngine.Networking.Match.MatchInfoSnapshot>();

        private void Awake()
        {
            NetworkManager.singleton.StartMatchMaker();
        }
        private void ListMatches()
        {
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, Populate);
        }
        private void Populate(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            //CREATE A BUTTON FOR EACH MATCH
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace UGP
{
    public class NetworkOnButtonClick : MonoBehaviour, IPointerClickHandler
    {
        public MatchInfoSnapshot info;
        
        public void MatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            NetworkManager.singleton.OnMatchJoined(success, extendedInfo, matchInfo);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(info != null)
            {
                NetworkManager.singleton.matchMaker.JoinMatch(info.networkId, "", "", "", 0, 0, MatchJoined);
            }
        }
    }
}

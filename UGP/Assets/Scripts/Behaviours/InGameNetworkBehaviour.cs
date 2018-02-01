using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    //NEEDS WORK
    public class InGameNetworkBehaviour : MonoBehaviour
    {
        private void FixedUpdate()
        {
            var allPlayers = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();
            var allVehicles = GameObject.FindObjectsOfType<VehicleBehaviour>().ToList();

            if(allVehicles == null || allPlayers.Count < allVehicles.Count)
            {
                //SPAWN A VEHICLE FOR EACH PLAYER THAT IS CONNECTED
            }
        }
    }

}

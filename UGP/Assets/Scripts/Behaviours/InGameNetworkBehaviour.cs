using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace UGP
{
    //NEEDS WORK
    public class InGameNetworkBehaviour : NetworkBehaviour
    {
        public GameObject Vehicle;





        private void FixedUpdate()
        {
            //SPAWN A VEHICLE FOR EACH PLAYER THAT IS CONNECTED

            //var allPlayers = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();

            var allPlayers = NetworkManager.singleton.numPlayers;
            var allVehicles = GameObject.FindObjectsOfType<VehicleBehaviour>().ToList();

            int playerCount = allPlayers;
            int vehicleCount = 0;

            //if (allPlayers != null)
            //    playerCount = allPlayers.Count;

            if (allVehicles != null)
                vehicleCount = allVehicles.Count;

            int vehiclesNeeded = Mathf.Abs(playerCount - vehicleCount);

            for(int i = 0; i < vehiclesNeeded; i++)
            {
                var pos = NetworkManager.singleton.GetStartPosition();
                var v = Instantiate(Vehicle, pos.position, pos.rotation);
                NetworkServer.Spawn(v);
            }
        }
    }

}

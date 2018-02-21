using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UGP
{
    //NEEDS WORK
    public class InGameNetworkBehaviour : NetworkBehaviour
    {
        public List<GameObject> VehiclePrefabs;
        public Transform OriginVehicleSpawn;

        [HideInInspector] public float vehiclePositionOffset; 
        private bool spawnOnPlayerCount;

        private void SpawnVehiclesOnPlayerCount()
        {
            //SPAWN A VEHICLE FOR EACH PLAYER THAT IS CONNECTED

            var allPlayers = GameObject.FindObjectsOfType<PlayerBehaviour>().ToList();

            //var allPlayers = NetworkManager.singleton.numPlayers;
            var allVehicles = GameObject.FindObjectsOfType<VehicleBehaviour>().ToList();

            int playerCount = 0;
            int vehicleCount = 0;

            if (allPlayers != null)
                playerCount = allPlayers.Count;

            if (allVehicles != null)
                vehicleCount = allVehicles.Count;

            int vehiclesNeeded = Mathf.Abs(playerCount - vehicleCount);

            for (int i = 0; i < vehiclesNeeded; i++)
            {
                var vehicle = VehiclePrefabs[0];
                var pos = NetworkManager.singleton.GetStartPosition();
                var v = Instantiate(vehicle, pos.position, pos.rotation);
                NetworkServer.Spawn(v);
            }
        }

        public void SpawnAllVehicles()
        {
            for(int i = 0; i < VehiclePrefabs.Count; i++)
            {
                var spawn = OriginVehicleSpawn.position;

                var spawnPos = i * vehiclePositionOffset;

                spawn.x += spawnPos;

                var v = Instantiate(VehiclePrefabs[i], spawn, OriginVehicleSpawn.rotation);
                NetworkServer.Spawn(v);
            }
        }

        public void ToggleVehicleSpawning()
        {
            if(spawnOnPlayerCount == true)
            {
                spawnOnPlayerCount = false;
            }
            else
            {
                spawnOnPlayerCount = true;
            }
        }

        private void Start()
        {
            spawnOnPlayerCount = false;
            vehiclePositionOffset = 20;
        }

        private void FixedUpdate()
        {
            if(spawnOnPlayerCount)
            {
                SpawnVehiclesOnPlayerCount();
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(InGameNetworkBehaviour))]
    public class InspectorInGameNetworkBehaviour : Editor
    {
        GUIStyle header = new GUIStyle();

        public override void OnInspectorGUI()
        {
            var mytarget = target as InGameNetworkBehaviour;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("SPAWN ALL VEHICLES"))
            {
                mytarget.SpawnAllVehicles();
            }
            GUILayout.Space(10);
            if(GUILayout.Button("TOGGLE SPAWN VEHICLE PER PLAYER"))
            {
                mytarget.ToggleVehicleSpawning();
            }
        }
    }
#endif
}

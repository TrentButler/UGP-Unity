using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGP
{
    public class BuildingBehaviour : MonoBehaviour
    {
        public List<GameObject> VehiclePrefabs;
        public Transform VehicleSpawn;

        public List<GameObject> ItemPrefabs;
        public List<Transform> ItemSpawns;

        private InGameNetworkBehaviour server;

        public void Spawn()
        {
            if(server != null)
            {
                if(server.isServer)
                {
                    //SPAWN THE VEHICLE 
                    //var vehicle_index = Random.Range(0, VehilcePrefabs.Count);
                    var vehicle_index = 0;
                    //var vehicle = Instantiate(VehiclePrefabs[vehicle_index], VehicleSpawn.position, VehicleSpawn.rotation);
                    //NetworkServer.Spawn(vehicle);

                    server.Spawn(VehiclePrefabs[vehicle_index], VehicleSpawn.position, VehicleSpawn.rotation); //SPAWN THE VEHICLE

                    //SPAWN THE ITEMS
                    ItemPrefabs.ForEach(item =>
                    {
                        var spawnPoint = Random.Range(0, ItemSpawns.Count);
                        var spawn = ItemSpawns[spawnPoint];
                        server.Spawn(item, spawn.position, spawn.rotation);

                        //var i = Instantiate(item, spawn.position, spawn.rotation);
                        //NetworkServer.Spawn(i);
                    });
                }
            }
        }

        private void CheckisServer()
        {
            if (server == null)
            {
                server = FindObjectOfType<InGameNetworkBehaviour>();
                if (server == null)
                {
                    return;
                }
            }
        }

        private void Start()
        {
            server = FindObjectOfType<InGameNetworkBehaviour>();
        }

        private void LateUpdate()
        {
            CheckisServer();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildingBehaviour))]
    public class InspectorBuildingBehaviour : Editor
    {
        GUIStyle header = new GUIStyle();

        public override void OnInspectorGUI()
        {
            var mytarget = target as BuildingBehaviour;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("SPAWN"))
            {
                mytarget.Spawn();
            }
        }
    }
#endif 
}
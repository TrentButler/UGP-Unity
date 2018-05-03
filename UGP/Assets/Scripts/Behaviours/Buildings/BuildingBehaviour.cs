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
    public class BuildingBehaviour : MonoBehaviour
    {
        public List<GameObject> VehiclePrefabs;
        public List<GameObject> ItemPrefabs;

        public List<GameObject> Doors;

        private InGameNetworkBehaviour server;

        public void Spawn()
        {
            if(server != null)
            {
                if(server.isServer)
                {
                    if (VehiclePrefabs.Count > 0)
                    {
                        var vehicleSpawns = FindObjectsOfType<VehicleSpawn>().ToList();
                        vehicleSpawns.ForEach(spawn => 
                        {
                            var vehicle_index = Random.Range(0, VehiclePrefabs.Count);
                            server.Spawn(VehiclePrefabs[vehicle_index], spawn.transform.position, spawn.transform.rotation); //SPAWN THE VEHICLE
                        });
                    }

                    if (ItemPrefabs.Count > 0)
                    {
                        var itemSpawns = FindObjectsOfType<ItemSpawn>().ToList();
                        itemSpawns.ForEach(spawn =>
                        {
                            var item_index = Random.Range(0, ItemPrefabs.Count);
                            server.Spawn(ItemPrefabs[item_index], spawn.transform.position, spawn.transform.rotation);

                        });
                    }

                    Doors.ForEach(door =>
                    {
                        var door_behaviour = door.GetComponent<DoorBehaviour>();
                        door_behaviour.SpawnButtons();
                    });
                }
            }
        }

        public void ServerSpawn(InGameNetworkBehaviour s)
        {
            server = s;

            if (VehiclePrefabs.Count > 0)
            {
                var vehicleSpawns = FindObjectsOfType<VehicleSpawn>().ToList();
                vehicleSpawns.ForEach(spawn =>
                {
                    var vehicle_index = Random.Range(0, VehiclePrefabs.Count);
                    var v = VehiclePrefabs[vehicle_index];
                    var vBehaviour = v.GetComponent<VehicleShootBehaviour>();
                    vBehaviour.OnSpawn(server);
                    server.Spawn(v, spawn.transform.position, spawn.transform.rotation); //SPAWN THE VEHICLE
                });
            }

            if (ItemPrefabs.Count > 0)
            {
                var itemSpawns = FindObjectsOfType<ItemSpawn>().ToList();
                itemSpawns.ForEach(spawn =>
                {
                    var item_index = Random.Range(0, ItemPrefabs.Count);
                    server.Spawn(ItemPrefabs[item_index], spawn.transform.position, spawn.transform.rotation);

                });
            }

            Doors.ForEach(door =>
            {
                var door_behaviour = door.GetComponent<DoorBehaviour>();
                door_behaviour.SpawnButtons();
            });
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
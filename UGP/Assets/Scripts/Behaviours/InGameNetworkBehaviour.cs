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

        #region ServerCamera
        private GameObject server_camera;
        public float cameraSpeed = 2.5f;
        public float cameraTranslationSpeed = 10.0f;

        private void FreeLookCamera()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            var move_vector = new Vector3(h, 0, v);
            server_camera.transform.Translate(move_vector);

            #region MouseLook
            //CALCULATE AN MOUSE DELTA
            //DERIVE AN DIRECTION
            //LERP BETWEEN THE CURRENT CAMERA ROTATION TO AN NEW CAMERA ROTATION
            if (Input.GetMouseButton(1))
            {
                var deltaX = Input.GetAxis("Mouse X"); //GET THE MOUSE DELTA X
                var deltaY = Input.GetAxis("Mouse Y"); //GET THE MOUSE DELTA Y

                Vector3 rotX = new Vector3(-deltaY, 0, 0);
                Vector3 rotY = new Vector3(0, deltaX, 0);

                Quaternion rot = Quaternion.Euler(rotX); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                server_camera.transform.rotation = Quaternion.Slerp(server_camera.transform.rotation, server_camera.transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION

                rot = Quaternion.Euler(rotY); //CREATE A QUATERNION ROTATION FROM A EULER ANGLE ROTATION
                server_camera.transform.rotation = Quaternion.Slerp(server_camera.transform.rotation, server_camera.transform.rotation * rot, 1.0f); //INTERPOLATE BETWEEN THE CURRENT ROTATION AND THE NEW ROTATION
            }

            //TRANSLATE THE CAMERA BASED ON CLICKDRAG DELTA
            if (Input.GetMouseButton(2))
            {
                var dX = Input.GetAxis("Mouse X"); //GET THE DELTA MOUSE X
                var dY = Input.GetAxis("Mouse Y"); //GET THE DELTA MOUSE Y

                Vector3 trans = new Vector3(-(dX * cameraTranslationSpeed), -(dY * cameraTranslationSpeed), 0); //DERIVE A TRANSLATION VECTOR
                server_camera.transform.Translate(trans); //APPLY THE TRANSLATION TO THE CAMERA'S TRANSFORM
            }

            //RESET THE CAMERA'S ROTATION
            if(Input.GetKeyDown(KeyCode.Space))
            {
                var currentRot = server_camera.transform.rotation;
                currentRot[0] = 0.0f;
                currentRot[2] = 0.0f;

                server_camera.transform.rotation = currentRot;
            }

            //MOVE THE CAMERA EITHER FORWARD OR BACKWARD FROM MOUSE SCROLL
            var scrollDelta = Input.GetAxis("Mouse ScrollWheel"); //GET THE MOUSE SCROLL DELTA
            Vector3 translation = new Vector3(0, 0, scrollDelta * cameraSpeed); //DERIVE A TRANSLATION VECTOR
            server_camera.transform.Translate(translation); //TRANSLATE THE CAMERA'S TRANSFORM
            #endregion
        }
        #endregion
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

        public void ResetServer()
        {
            NetworkServer.Reset();
        }

        private void Start()
        {
            spawnOnPlayerCount = false;
            vehiclePositionOffset = 20;
            if(isServer)
            {
                server_camera = Camera.main.gameObject;
            }
        }

        private void FixedUpdate()
        {
            if(spawnOnPlayerCount)
            {
                SpawnVehiclesOnPlayerCount();
            }

            if(isServer)
            {
                FreeLookCamera();
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

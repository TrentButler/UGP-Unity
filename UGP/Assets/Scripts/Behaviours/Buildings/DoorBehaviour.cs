using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGP
{
    public class DoorBehaviour : MonoBehaviour
    {
        public GameObject ParentButtonModel;
        public GameObject ButtonModel;
        public List<Transform> ButtonSpawns;

        public bool isOpen = false;
        [Range(1.0f, 999.0f)] public float DoorOpenSpeed;
        public Vector3 DoorOpen;
        public Vector3 DoorClosed;
        [Range(1.0f, 999.0f)] public float ColliderScale = 1.5f;

        private void Start()
        {
            gameObject.name = "DOOR @ :" + transform.position.ToString();
        }
        
        public void SpawnButtons()
        {
            //SPAWN ALL OF THE BUTTONS FOR THIS DOOR ON THE SERVER
            //ADD THE 'GARAGEDOORBEHAVIOUR' TO THE FIRST BUTTON
            var server = FindObjectOfType<InGameNetworkBehaviour>();
            GameObject parentButton = null;

            var door_id = "DOOR @ :" + transform.position.ToString();
            
            for(int i = 0; i < ButtonSpawns.Count; i++)
            {
                if(i == 0)
                {
                    var _button = Instantiate(ParentButtonModel, ButtonSpawns[i].position, ButtonSpawns[i].rotation);
                    parentButton = _button;

                    var _col = _button.AddComponent<BoxCollider>();
                    //_col.center = _button.transform.
                    _col.size = _col.size * ColliderScale;
                    _col.isTrigger = true;

                    var _behaviour = _button.GetComponent<GarageDoorBehaviour>();
                    _behaviour.DoorName = door_id.ToString();
                    _behaviour.OpenSpeed = DoorOpenSpeed;
                    _behaviour.GarageDoor = transform;
                    _behaviour.MaxDoorBounds = DoorOpen;
                    _behaviour.MinDoorBounds = DoorClosed;
                    _behaviour.opengaragedoor = isOpen;

                    server.Spawn(_button);
                    continue;
                }

                var button = Instantiate(ButtonModel, ButtonSpawns[i].position, ButtonSpawns[i].rotation);
                button.transform.SetParent(parentButton.transform);

                var col = parentButton.AddComponent<BoxCollider>();
                col.center = button.transform.localPosition;
                col.size = col.size * ColliderScale;
                col.isTrigger = true;

                server.Spawn(button);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DoorBehaviour))]
    public class InspectorDoorBehaviour : Editor
    {
        GUIStyle header = new GUIStyle();

        public override void OnInspectorGUI()
        {
            var mytarget = target as DoorBehaviour;
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("SPAWN"))
            {
                mytarget.SpawnButtons();
            }
        }
    }
#endif 
}
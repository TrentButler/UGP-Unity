using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class OfflineGarageSceneBehaviour : MonoBehaviour
    {
        public Transform HoverCraft;
        public Collider DockEntrance;
        void OnTriggerEnter(Collider vehicle)
        {
            var player = FindObjectsOfType<VehicleMovementBehaviour>();

            if (vehicle.tag == "Vehicle")
            {

                Debug.Log("hit");
                SceneManager.LoadScene(0);
            }
        }
        public void ChangeScene()
        {
            if (HoverCraft.transform.position == DockEntrance.transform.position)
            {
                SceneManager.LoadScene(0);
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ChangeScene();
        }
    }
}

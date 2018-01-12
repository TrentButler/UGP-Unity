using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Trent
{


    public class ChangeColorBehaviour : MonoBehaviour {

        public Transform HoverCraft;
        public Collider DockEntrance;
        public bool Clicked;
        public PlayerState state;
        // Use this for initialization
        void Start() {
            Clicked = false;
            //var carcolor = GetComponent<MeshRenderer>().material.color = Color.green;
            //HoverCraft.GetComponent<MeshRenderer>().material.color = carcolor;

        }
        void OnTriggerEnter(Collider vehicle)
        {
            var player = FindObjectsOfType<VehicleMovementBehaviour>();

            if (vehicle.tag == "Vehicle")
            {
                Debug.Log("hit");
                SceneManager.LoadScene(0);
            }
        }
    public void OnclickChangeColor()
        {
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = Color.yellow;
            HoverCraft.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        public void ChangeScene()
        {

            SceneManager.LoadScene(1);
            if (HoverCraft.transform.position == DockEntrance.transform.position)
            {
                Debug.Log("hit");
                SceneManager.LoadScene(0);
            }
        }
        public void ReturnChangeScene()
        {
            SceneManager.LoadScene(0);
        }
        // Update is called once per frame
        void Update() {
            if (Clicked == true)
            {
                GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }

        }
    }
}
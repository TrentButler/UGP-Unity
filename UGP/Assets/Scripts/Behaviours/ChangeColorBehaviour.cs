using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace UGP
{
    public class ChangeColorBehaviour : MonoBehaviour {


        

        public Transform HoverCraft;
        public Collider DockEntrance;
        public NetworkPlayer Player;
        public bool Clicked;
        public PlayerState state;
        public GameObject Panel;
        public Transform PanelTest;
        #region Colors
            public float Red = 255;
            public float Green = 128;
            public float Blue = 255;
        #endregion
        #region Sliders
        public Slider GlossySliderValue;
        public Slider MetallicSliderValue;
        public Slider Smoothness;
        public Slider RedSlider;
        public Slider GreenSlider;
        public Slider BlueSlider;
        #endregion
        // Use this for initialization
        void Start() {
   
            Clicked = false;
        

        }
        void OnTriggerEnter(Collider vehicle)
        {
            var player = FindObjectsOfType<VehicleMovementBehaviour>();
            
            if (vehicle.tag == "Vehicle")
            {
                NetworkManager.singleton.StopClient();
                NetworkManager.singleton.StopHost();
             
               
                Debug.Log("hit");
                //SceneManager.LoadScene(0);
            }
        }
        public void MetallicSlider()
        {
            var Metallic = MetallicSliderValue.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", Metallic);
        }
        public void GlossySlider()
        {
            var Gloss = GlossySliderValue.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", Gloss);
        }
        public void RedValueSlider()
        {

            Red = RedSlider.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Red, Green, Blue);

        }
        public void GreenValueSlider()
        {
           

            Green = GreenSlider.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Red, Green, Blue);

        }
        public void BlueValueSlider()
        {
            

            Blue = BlueSlider.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Red, Green, Blue);


        }
        public void OnclickChangeColor()
        {
            Panel.SetActive(true);
            
            //GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Red, Green, Blue);
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
            RedValueSlider();
            GreenValueSlider();
            BlueValueSlider();
            OnclickChangeColor();
            MetallicSlider();
            GlossySlider();
            if (Clicked == true)
            {
                GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Blue, Green, Red) ;
            }

        }
    }
}
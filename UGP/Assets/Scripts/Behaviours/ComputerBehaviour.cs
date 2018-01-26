using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


namespace UGP
{
    public class ComputerBehaviour : MonoBehaviour
    {
       
        public GameObject NetworkHUD;
        public PlayerMovementBehaviour b;
        public NetworkManager manager;

        public void NetworkHUDOn()
        {
            NetworkHUD.SetActive(true);
        }
        public void NetworkHUDOFF()
        {
            NetworkHUD.SetActive(false); 
        }
        public void TagCheck()
        {
            
            

        }
        // Use this for initialization
        void Start()
        {
            NetworkHUD = GameObject.FindGameObjectWithTag("NetworkManager");
            NetworkHUD.SetActive(false);
            Cursor.visible = true;
        }


        // Update is called once per frame
        void Update()
        {



            

            var s = b.state;

            if (s != PlayerState.sitting)
            {
                NetworkHUDOFF();
            }

          
        }
    }
}
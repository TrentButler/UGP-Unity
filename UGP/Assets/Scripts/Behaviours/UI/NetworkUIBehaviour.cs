using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class NetworkUIBehaviour : MonoBehaviour
    {
        public GameObject clientUI;
        public GameObject serverUI;
        
        public bool clientUIActive;
        public bool serverUIActive;

        public void DestroyUI()
        {
            Destroy(clientUI);
            Destroy(serverUI);
        }

        public void ToggleServerUI()
        {
            if (serverUIActive == true)
            {
                serverUIActive = false;
            }

            else
            {
                serverUIActive = true;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(clientUI);
            DontDestroyOnLoad(serverUI);
        }

        private void Start()
        {
            serverUI.SetActive(false);
        }

        private void Update()
        {   
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                ToggleServerUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            clientUI.SetActive(clientUIActive);
            serverUI.SetActive(serverUIActive);
        }
    }
}
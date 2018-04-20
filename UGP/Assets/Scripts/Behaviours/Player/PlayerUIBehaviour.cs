using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class PlayerUIBehaviour : NetworkBehaviour
    {
        public PlayerBehaviour PlayerBrain;
        
        public GameObject PlayerUI;
        public GameObject PlayerDeadCanvas;
        public GameObject SettingsCanvas;
        public Slider HealthSlider;
        public GameObject PlayerNameTag;
        public Text PlayerNameText;
        public string GotoSceneString;
        public Button MainMenuButton;
        public Button RespawnButton;

        public void ExitApplication()
        {
            Application.Quit();
        }

        public void GotoScene()
        {
            if(GotoSceneString == "")
            {
                GotoSceneString = "00.PickAScene";
            }
            
            var netManager = GameObject.FindGameObjectWithTag("NetworkManager");
            var network_manager = netManager.GetComponent<NetworkManager>();
            network_manager.StopClient();
            Destroy(netManager);

            SceneManager.LoadScene(GotoSceneString);
        }

        public void RespawnPlayer()
        {
            if(!isLocalPlayer)
            {
                return;
            }

            var net_manager = FindObjectOfType<InGameNetworkBehaviour>();
            var player_identity = GetComponent<NetworkIdentity>();

            net_manager.RespawnPlayer(player_identity);
        }
        
        private void Start()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            HealthSlider.maxValue = PlayerBrain.MaxHealth;
            PlayerNameText.text = PlayerBrain.playerName;
        }

        private void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                return;
            }

            //CHANGE THIS TO THE INPUTCONTROLLER.BUTTONINPUTYOUNEED
            if (Input.GetKey(KeyCode.Escape))
            {
                SettingsCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                SettingsCanvas.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            PlayerNameText.text = PlayerBrain.playerName;

            if (isServer)
            {
                PlayerNameTag.SetActive(true);
                PlayerUI.SetActive(false);
                PlayerDeadCanvas.SetActive(false);
            }

            if (!isLocalPlayer)
            {
                PlayerNameTag.SetActive(true);
                return;
            }

            PlayerNameTag.SetActive(false);

            if (HealthSlider.maxValue <= 0.0f)
            {
                HealthSlider.maxValue = PlayerBrain.MaxHealth;
            }

            HealthSlider.value = PlayerBrain.playerHealth;

            if(PlayerBrain.isDead)
            {
                PlayerUI.SetActive(false);
                PlayerDeadCanvas.SetActive(true);
                PlayerNameTag.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                PlayerDeadCanvas.SetActive(false);

                if(PlayerBrain.isDriving)
                {
                    PlayerUI.SetActive(false);
                }
                else
                {
                    PlayerUI.SetActive(true);
                }
            }
        }
    }
}
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
        public Slider HealthSlider;
        public Text PlayerNameText;
        public string GotoSceneString;
        public Button MainMenuButton;
        public Button RespawnButton;

        public void GotoScene()
        {
            if(GotoSceneString == "")
            {
                GotoSceneString = "00.PickAScene";
            }

            var netManager = GameObject.FindGameObjectWithTag("NetworkManager");
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
            PlayerNameText.text = PlayerBrain.gameObject.name;
        }

        private void LateUpdate()
        {
            if (isServer)
            {
                PlayerUI.SetActive(false);
                PlayerDeadCanvas.SetActive(false);
            }

            if (!isLocalPlayer)
            {
                return;
            }

            if(HealthSlider.maxValue <= 0.0f)
            {
                HealthSlider.maxValue = PlayerBrain.MaxHealth;
            }

            HealthSlider.value = PlayerBrain.playerHealth;

            if(PlayerBrain.isDead)
            {
                PlayerUI.SetActive(false);
                PlayerDeadCanvas.SetActive(true);
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
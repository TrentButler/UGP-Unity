﻿using System.Collections;
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
        public Image HealthSlider;
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
            //DESTROY THE NETWORK MANAGER, AND ITS UI
            var network_manager = netManager.GetComponent<NetworkManager>();
            var networkUI = network_manager.GetComponent<NetworkUIBehaviour>();
            networkUI.DestroyUI();

            //network_manager.StopClient();
            Destroy(netManager);

            SceneManager.LoadScene(GotoSceneString);
        }
        public void GotoScene(string scene)
        {
            if (scene == "")
            {
                scene = "00.PickAScene";
            }

            var netManager = GameObject.FindGameObjectWithTag("NetworkManager");
            //DESTROY THE NETWORK MANAGER, AND ITS UI
            var network_manager = netManager.GetComponent<NetworkManager>();
            var networkUI = network_manager.GetComponent<NetworkUIBehaviour>();
            networkUI.DestroyUI();

            network_manager.StopClient();
            Destroy(netManager);

            SceneManager.LoadScene(scene);
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

        public float GetPlayerHealth()
        {
            //var health_displacement = PlayerBrain.playerHealth / PlayerBrain.MaxHealth;
            //var calc = new Vector3(health_displacement, 0, 0);
            //return calc.normalized.x;

            return (1 / PlayerBrain.MaxHealth) * PlayerBrain.playerHealth;
        }
        
        private void Start()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            //HealthSlider.maxValue = PlayerBrain.MaxHealth;
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

            //if (HealthSlider.maxValue <= 0.0f)
            //{
            //    HealthSlider.maxValue = PlayerBrain.MaxHealth;
            //}

            //HealthSlider.value = PlayerBrain.playerHealth;
            HealthSlider.fillAmount = GetPlayerHealth();

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

    public GameObject NetworkHud;

    public Button OnlineMode;

    public NetworkMangerHUD gudhud;

    public GameObject MainMenuHUD;

    public GameObject MultiplayerHUD;
    public void NetOn()
    {
        NetworkHud.SetActive(true);
        MainMenuHUD.SetActive(false);
    }
    public void NetworkHUDOn()
    {
        MultiplayerHUD.SetActive(true);

    }
    public void NetworkHUDOFF()
    {
        MultiplayerHUD.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        //NetworkHud = GameObject.FindGameObjectWithTag("NetworkManager");
        //NetworkHud.SetActive(false);
        //MultiplayerHUD.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

    public GameObject NetworkHud;

    public Button OnlineMode;

    public NetworkMangerHUD gudhud;

    public void NetOn()
    {
        NetworkHud.SetActive(true);
    }

	// Use this for initialization
	void Start ()
    {
        NetworkHud = GameObject.FindGameObjectWithTag("NetworkManager");
        NetworkHud.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        	
	}
}

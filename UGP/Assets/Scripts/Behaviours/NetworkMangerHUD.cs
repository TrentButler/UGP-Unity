using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMangerHUD : NetworkBehaviour
{
    public Network Net;
    public NetworkPlayer NetPlayer;

    private string IpValue;
    private string PortValue;
    public string UIIP;
    public string UIPORTVALUE;
    public Text UITextIP;
    public Text UITextPort;
    public void CheckPlayerIP()
    {

        IpValue.Insert(0, NetPlayer.ipAddress);
        

        IpValue.Equals(NetPlayer.ipAddress);
        PortValue.Equals(NetPlayer.port);
     
    }
    public void ConnectToServer()
    {
        Network.Connect(IpValue, PortValue);
        NetworkClient.GetTotalConnectionStats();   
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        IpValue = UITextIP.text;
        PortValue = UITextPort.text;
        CheckPlayerIP();


	}
}

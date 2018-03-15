using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMangerHUD : MonoBehaviour
{

    #region NetworkStuff
    public NetworkManager Serverinfo;
    private NetworkServer Server;
    private NetworkServerSimple ServerSimp;
    private string con;
    
    #endregion

    #region Canvas
    public GameObject ToggleNetworkCanvas;
    public Button ToggleOn;
    public Text IP_DISPLAY;
    public Text PlayID;

    #endregion

    #region Strings
    private string IpValue;


    #endregion

    #region Text

    public InputField IPInputField;
    public InputField PortInputField;
    #endregion



    public void StartServer()
    {
        Serverinfo.StartHost();
        
    }
    public void ServerInfo()
    {

       var id = ServerSimp.serverHostId.ToString();
        id = PlayID.text;
        //var IP = Serverinfo.networkAddress.ToString();
        
        var IP = ServerSimp.connections.ToString();
        IpValue = IP;
        IpValue = IP_DISPLAY.text;
        Debug.Log(IpValue);
        Debug.Log(id);
    }
    public void UpdateServer()
    {
        ServerSimp.Update();
        ServerSimp.UpdateConnections();
        
    }
    public void CloseServer()
    {
        Serverinfo.StopServer();
    }

    public void NetworkHUDOn()
    {
        ToggleNetworkCanvas.SetActive(true);
    }
    public void NetworkHUDOFF()
    {
        ToggleNetworkCanvas.SetActive(false);
    }
    public void ButtonAssign()
    {
        Button btn = ToggleOn.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        ToggleOn.enabled = true;
    }

    void TaskOnClick()
    {
        if (ToggleOn == true)
        {
            ToggleNetworkCanvas.SetActive(true);
        }
        if (ToggleOn == false)
        {
            ToggleNetworkCanvas.SetActive(false);
        }
        Debug.Log("You have opened the Online Menu!");

    }

    //public void CheckPlayerIP()
    //{
    //    var test = UnityEngine.Networking.NetworkManager.singleton.networkAddress.ToString();

    //    test = IpValue;
    //    Text text = IP_DISPLAY.GetComponent<Text>();
    //    IpValue = text.text;
    //    IP_DISPLAY.text = IPInputField.text;
    //    PORT_DISPLAY.text = PortInputField.text;
    //    IpValue = IPInputField.text;
    //    Debug.Log(test);

    //}

    //public void ConnectToServer()
    //{
    //    Network.Connect(IpValue, PortValue);
    //    NetworkClient.GetTotalConnectionStats();
    //}

    // Use this for initialization
    void Start()
{
    NetworkHUDOFF();
}

// Update is called once per frame
void Update()
{
        var test = UnityEngine.Networking.NetworkManager.singleton.networkAddress.ToString();
        //CheckPlayerIP();

        Debug.Log(test);

        ButtonAssign();




}
}

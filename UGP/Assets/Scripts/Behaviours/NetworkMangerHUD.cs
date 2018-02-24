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
    public Network Net;
    public NetworkPlayer NetPlayer;


    #endregion

    #region Canvas
    public GameObject ToggleNetworkCanvas;
    public Button ToggleOn;
    public Text IP_DISPLAY;
    public Text PORT_DISPLAY;

    #endregion

    #region Strings
    private string IpValue;
    private string PortValue;


    #endregion

    #region Text

    public InputField IPInputField;
    public InputField PortInputField;
    #endregion





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

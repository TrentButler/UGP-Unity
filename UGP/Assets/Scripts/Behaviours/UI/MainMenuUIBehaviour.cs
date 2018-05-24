using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIBehaviour : MonoBehaviour
{
    public AudioSource OnButtonPress;
    public string CustomizeScene;
    public string OnlineScene;
    public string OfflineScene;
    
    public bool NetworkUIActive;
    
    public GameObject NetworkUI;
    
    public void GotoOnlineScene()
    {
        OnButtonPress.Stop();
        OnButtonPress.Play();
        SceneManager.LoadScene(OnlineScene);
    }
    public void GotoCustomizeScene()
    {
        OnButtonPress.Stop();
        OnButtonPress.Play();
        SceneManager.LoadScene(CustomizeScene);
    }
    public void GotoOfflineScene()
    {
        OnButtonPress.Stop();
        OnButtonPress.Play();
        SceneManager.LoadScene(OfflineScene);
    }
    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ToggleNetworkUI()
    {
        if (NetworkUIActive)
        {
            NetworkUIActive = false;
        }
        else
        {
            NetworkUIActive = true;
        }
    }
    public void ToggleNetworkUI(bool active)
    {
        if (active)
        {
            NetworkUIActive = true;
        }
        else
        {
            NetworkUIActive = false;
        }
    }

    private void Start()
    {
        ToggleNetworkUI(false);
    }

    private void LateUpdate()
    {
        NetworkUI.SetActive(NetworkUIActive);
    }
}

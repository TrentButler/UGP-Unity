using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishButtonClickBehaviour : MonoBehaviour
{
    public UGPEVENTS.GameEventArgs ButtonClicked;
    
    public void RaiseButtonEvent()
    {
        ButtonClicked.Raise(gameObject);
    }
}

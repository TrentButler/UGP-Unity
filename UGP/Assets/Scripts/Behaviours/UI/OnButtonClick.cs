using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace UGP
{
    public class OnButtonClick : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ButtonClick(string s);
        public ButtonClick onClick;
        public string data;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke(data);
        }
    }
}
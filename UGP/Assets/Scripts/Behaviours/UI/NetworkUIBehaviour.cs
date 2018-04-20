using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UGP
{
    public class NetworkUIBehaviour : MonoBehaviour
    {
        public GameObject canvas;
        public bool active = false;

        public void ToggleUI()
        {

            if (active == true)
            {
                active = false;
            }

            else
            {
                active = true;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(canvas);
        }

        private void Start()
        {
            canvas.SetActive(false);
        }

        private void Update()
        {   
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                ToggleUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            canvas.SetActive(active);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{

    public class PlayerClosetBehaviour : MonoBehaviour
    {

        public GameObject Shannon;

        public GameObject Sandra;



      public void OnSandra()
        {
            Shannon.SetActive(false);
            Sandra.SetActive(true);
        }

        public void OnShannon()
        {
            Shannon.SetActive(true);
            Sandra.SetActive(false);
        }


        // Use this for initialization
        void Start()
        {
            Sandra.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            

        }
    }
}
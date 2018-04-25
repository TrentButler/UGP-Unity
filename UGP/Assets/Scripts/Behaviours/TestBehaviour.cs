using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGPEVENTS
{

    public class TestBehaviour : MonoBehaviour
    {

        public int highscore = 100;
        public GameEventArgs GameStarted;

        // Use this for initialization
        void Start()
        {
            GameStarted.Raise(this);            
        }



        public void OnButtonClicked(UnityEngine.Object[] args)
        {
            var sender = args[0] as GameObject;
            if (sender == null)
                return;
            if (sender.name.Contains("Blue"))
                GetComponent<Renderer>().material.color = Color.blue;
            else
                GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
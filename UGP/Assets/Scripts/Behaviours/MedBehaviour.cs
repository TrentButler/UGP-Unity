using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class MedBehaviour : MonoBehaviour
    {
        //WHEN PLAYER CAN PICKUP THE ITEM, GENERATE A LIGHT ABOVE THE OBJECT
        
        public Med med;
        [HideInInspector]
        public Med _med;

        private GameObject lightGO;
        private bool alreadyON = false;

        public void LightOn()
        {
            if(!alreadyON)
            {
                alreadyON = true;
                lightGO = new GameObject(med.name + " Light");
                var light = lightGO.AddComponent<Light>();
                light.color = Color.white;
                light.type = LightType.Point;

                Vector3 lightPos = transform.position + new Vector3(0, 2.0f, 0);
                lightGO.transform.position = lightPos;
                Destroy(lightGO, 6.0f);
            }   
        }

        //NEEDS WORK
        //TYPE MISMATCH ON PICKUP
        public void PickUp(Player p)
        {
            if(p.toolBelt.AddItem(_med))
                Destroy(gameObject);
                Destroy(lightGO);
        }

        void Start()
        {
            _med = med;

            //lightGO = new GameObject(med.name + " Light");
            //var light = lightGO.AddComponent<Light>();
            //light.color = Color.white;
            //light.type = LightType.Point;

            //Vector3 lightPos = transform.position + new Vector3(0, 2.0f, 0);
            //lightGO.transform.position = lightPos;
        }

        void LateUpdate()
        {
            if (!lightGO)
            {
                alreadyON = false;
            }
        }
    }
}
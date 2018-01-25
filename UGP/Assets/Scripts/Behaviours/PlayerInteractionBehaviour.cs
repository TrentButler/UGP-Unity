using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class PlayerInteractionBehaviour : MonoBehaviour
    {

        public GameObject Head;
        public GameObject Body;

        public float distance;

        public float HEADminRot;
        public float HEADmaxRot;

        public float BODYminRot;
        public float BODYmaxRot;

        //RAYCAST FROM THE 'origin' A SET 'distance' FROM PLAYER

        void Start()
        {

        }

        //NEEDS WORK
        void FixedUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(Head.transform.position, Head.transform.forward.normalized, out hit, distance))
            {
                hit.collider.gameObject.SendMessage("LightOn");

                Debug.Log("Press F to pick up: " + hit.collider.gameObject.name);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    var p = GetComponent<PlayerBehaviour>().player;
                    hit.collider.gameObject.SendMessage("PickUp", p);
                }

                Debug.Log(hit.collider.tag);
            }
        }

        //NEEDS WORK
        private void LateUpdate()
        {
            //CLAMP THE ROTATIONS
            var r = transform.Find("PlayerCamera").transform.rotation;

            var yRot = r[1];
            Quaternion rot = Body.transform.rotation;
            //rot[1] = yRot * Time.fixedDeltaTime;

            rot[1] = Mathf.Clamp(yRot, BODYminRot, BODYmaxRot);

            Head.transform.rotation = r;
            Body.transform.rotation = rot;

            Debug.DrawRay(Head.transform.position, Head.transform.forward.normalized, Color.red);
        }
    }
}
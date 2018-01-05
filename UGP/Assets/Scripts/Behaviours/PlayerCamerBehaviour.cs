using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamerBehaviour : MonoBehaviour {


    public Transform Target;
    public Vector3 Offset;


	// Use this for initialization
	void Start ()
    {
        transform.SetParent(Target);	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Target)
        {
            var t = Target.position + Offset;
            //GetComponent<Transform>().Translate(t);
            GetComponent<Transform>().position = t;
            //GetComponent<Transform>().LookAt(Target);

        }
    }
}

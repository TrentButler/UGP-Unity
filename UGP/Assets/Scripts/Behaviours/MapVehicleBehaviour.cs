using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVehicleBehaviour : MonoBehaviour {

    public GameObject MapCanvas;
	// Use this for initialization
	void Start ()
    {
        MapCanvas.SetActive(false);
	}
    public void ToggleMap()
    {
        if (Input.GetKey(KeyCode.M))
        {
            MapCanvas.SetActive(true);
        }
        else
        {
            MapCanvas.SetActive(false);
        }

    }
    // Update is called once per frame
    void FixedUpdate () {
        ToggleMap();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalGameManager : ScriptableObject {
    public void ChangeColor(GameObject go)
    {
        go.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    }

	
}

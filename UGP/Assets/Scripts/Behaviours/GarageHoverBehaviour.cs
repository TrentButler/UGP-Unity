using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageHoverBehaviour : MonoBehaviour {


    private Rigidbody rb;
    public float hoverStrength;
    public float TargetHeight;
    public float RotateSpeed;
	// Use this for initialization
	void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        if (!rb)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
      
        //rb.constraints = RigidbodyConstraints.FreezePositionX;
        //rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }
	// Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * RotateSpeed, 0));

    }


    void FixedUpdate ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            var vertForce = (TargetHeight - hit.distance) / TargetHeight;
            Vector3 hoverVector = Vector3.up * vertForce * hoverStrength;
            Debug.Log(hoverVector);

            rb.AddForce(hoverVector);
        }
    }
}

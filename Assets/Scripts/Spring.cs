using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A spring. Connects two gameobjects (spheres).
 * Attaches to the object referenced by objA.
 */
public class Spring : MonoBehaviour {

    public GameObject objA;
    public GameObject objB;
    public float restLength;
    public float ks;

    void Start() {
        restLength = (objA.transform.position - objB.transform.position).magnitude;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 diffAToB = objB.transform.position - objA.transform.position;
        Vector3 dirAToB = diffAToB.normalized;
        float displacement = diffAToB.magnitude - restLength;
        objA.GetComponent<Rigidbody>().AddForce( dirAToB * displacement * ks, ForceMode.Force);
        objB.GetComponent<Rigidbody>().AddForce(-dirAToB * displacement * ks, ForceMode.Force);
	}
}

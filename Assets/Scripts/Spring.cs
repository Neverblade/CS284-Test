using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A spring. Connects two gameobjects (spheres).
 * Attaches to the object referenced by objA.
 */
public class Spring {

    public Mass objA;
    public Mass objB;
    public float ks;
    public float restLength;

    void Start() {
        restLength = (objA.transform.position - objB.transform.position).magnitude;
    }

    /*void Update() {
        if (debugOn) {
            Debug.DrawLine(objA.transform.position, objB.transform.position, Color.red);
        }
    }*/

    // Called once per physics frame
    /*void FixedUpdate () {
        // Add force based off of spring
        Vector3 diffAToB = objB.transform.position - objA.transform.position;
        Vector3 dirAToB = diffAToB.normalized;
        float displacement = diffAToB.magnitude - restLength;
        objA.GetComponent<Rigidbody>().AddForce( dirAToB * displacement * ks, ForceMode.Force);
        objB.GetComponent<Rigidbody>().AddForce(-dirAToB * displacement * ks, ForceMode.Force);
	}*/
}

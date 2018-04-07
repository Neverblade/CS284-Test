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
}

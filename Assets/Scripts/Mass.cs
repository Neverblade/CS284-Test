using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour {

    public float mass;
    public Vector3 force;
    public Vector3 position;
    public Vector3 prevPosition;

    // Collision handling modeled after proj4
    void OnCollisionEnter(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 normal = contact.normal;

        // Find the surface points on the particle sphere
        Vector3 velocityDir = (position - prevPosition).normalized;

    }
}

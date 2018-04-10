using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour {

    static float SURFACE_OFFSET = 0.00001f;
    public static string STATE = "ENABLED";

    public float mass;
    public float friction;
    public Vector3 force;
    public Vector3 position;
    public Vector3 prevPosition;

    // Collision handling modeled after proj4
    void OnCollisionStay(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 normal = contact.normal;

        // Find the surface point on the particle sphere
        // Surface point is defined as the point that portrudes into the plane the most
        // Found by going in the opposite direction of the plane's normal
        Vector3 surfacePoint = position - normal * transform.localScale.x / 2;
        Vector3 surfacePointPrev = prevPosition - normal * transform.localScale.x / 2;

        // Compute the tangent point using ray-plane intersection, then the correction
        float t = Vector3.Dot(hitPoint - surfacePoint, normal) / Vector3.Dot(normal, normal);
        Vector3 tangentPoint = surfacePoint + (t + SURFACE_OFFSET) * normal;
        Vector3 correction = tangentPoint - surfacePointPrev;

        // Update position
        position = prevPosition + correction * (1 - friction);
        transform.position = position;
    }
}

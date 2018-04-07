using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour {

    static float SURFACE_OFFSET = 0.00075f;

    public float mass;
    public float friction;
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
        Vector3 surfacePoint = position + velocityDir * transform.localScale.x / 2;
        Vector3 surfacePointPrev = prevPosition + velocityDir * transform.localScale.x / 2;

        // Compute the tangent point using ray-plane intersection, then the correction
        float t = Vector3.Dot(hitPoint - surfacePoint, normal) / Vector3.Dot(normal, normal);
        Vector3 tangentPoint = surfacePoint + (t + SURFACE_OFFSET) * normal;
        Vector3 correction = tangentPoint - surfacePointPrev;

        print(t);

        /*GameObject hitPointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitPointObj.name = "HitPointObj";
        hitPointObj.transform.position = hitPoint;
        hitPointObj.transform.localScale = Vector3.one * 0.01f;

        GameObject surfacePointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        surfacePointObj.name = "SurfacePointObj";
        surfacePointObj.transform.position = surfacePoint;
        surfacePointObj.transform.localScale = Vector3.one * 0.01f;

        GameObject surfacePointPrevObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        surfacePointPrevObj.name = "SurfacePointPrevObj";
        surfacePointPrevObj.transform.position = surfacePointPrev;
        surfacePointPrevObj.transform.localScale = Vector3.one * 0.01f;*/

        // Update position
        position = prevPosition + correction * (1 - friction);
        transform.position = position;

        print("test");
    }
}

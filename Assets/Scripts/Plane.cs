using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {

    public Vector3 point;
    public Vector3 normal;

    // Collision logic between the plane and a given mass.
    void Collide(Mass m) {
        float dotPos = Vector3.Dot(m.position - point, normal);
        float dotPrevPos = Vector3.Dot(m.prevPosition - point, normal);
        if (dotPos * dotPrevPos < 0) {
            Vector3 velocityDir = (m.position - m.prevPosition).normalized;
            float t = Vector3.Dot(point - m.prevPosition, normal) / Vector3.Dot(velocityDir, normal);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Spawns and initializes a size N cube of spheres.
 * Spawns them from the top left.
 */
public class ParticleCube : MonoBehaviour {

    public static Vector3 gravity = new Vector3(0, -9.8f, 0);

    public bool debugOn;
    public GameObject spherePrefab; // Prefab for the individual sphere

    public int N = 4; // # of spheres on each edge
    public float distanceRatio; // Distance between spheres defined as a multiplier on the sphere size.
    public float springTension; // Tension of spring between masses
    public float damping; // Damping term for motion
    public float mass; // Mass of each sphere

    private List<List<List<GameObject>>> spheres = new List<List<List<GameObject>>>();
    private List<Spring> springs = new List<Spring>();

	// Use this for initialization
	void Start () {
        // Initialize spheres and place into data structure
        float trueDistance = distanceRatio * spherePrefab.transform.localScale.x;
		for (int i = 0; i < N; i++) {
            spheres.Add(new List<List<GameObject>>());
            float y = transform.position.y + trueDistance * i;
            for (int j = 0; j < N; j++) {
                spheres[i].Add(new List<GameObject>());
                float x = transform.position.x + trueDistance * j;
                for (int k = 0; k < N; k++) {
                    float z = transform.position.z + trueDistance * k;
                    Vector3 pos = new Vector3(x, y, z);
                    GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity);
                    sphere.transform.parent = this.transform;
                    Mass m = sphere.AddComponent<Mass>();
                    m.position = pos;
                    m.prevPosition = pos;
                    m.mass = mass;
                    spheres[i][j].Add(sphere);
                }
            }
        }

        // Initialize springs
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                for (int k = 0; k < N; k++) {
                    // Straight edges
                    AddSpring(i, j, k, i + 1, j, k);
                    AddSpring(i, j, k, i, j + 1, k);
                    AddSpring(i, j, k, i, j, k + 1);

                    // Same-plane diagonals
                    AddSpring(i, j, k, i, j + 1, k + 1);
                    AddSpring(i, j, k, i, j + 1, k - 1);
                    AddSpring(i, j, k, i, j - 1, k + 1);
                    AddSpring(i, j, k, i, j - 1, k - 1);

                    // Off-plane straight
                    AddSpring(i, j, k, i + 1, j + 1, k);
                    AddSpring(i, j, k, i + 1, j - 1, k);
                    AddSpring(i, j, k, i + 1, j, k + 1);
                    AddSpring(i, j, k, i + 1, j, k - 1);

                    // Off-plane diagonals
                    AddSpring(i, j, k, i + 1, j + 1, k + 1);
                    AddSpring(i, j, k, i + 1, j + 1, k - 1);
                    AddSpring(i, j, k, i + 1, j - 1, k + 1);
                    AddSpring(i, j, k, i + 1, j - 1, k - 1);
                }
            }
        }
    }

    // Called every physics frame
    void FixedUpdate() {
        // Clear out forces and add gravity
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                for (int k = 0; k < N; k++) {
                    Mass m = spheres[i][j][k].GetComponent<Mass>();
                    m.force = m.mass * gravity;
                }
            }
        }

        // Add spring forces
        foreach (Spring spring in springs) {
            Vector3 diffAToB = spring.objB.position - spring.objA.position;
            Vector3 dirAToB = diffAToB.normalized;
            float displacement = diffAToB.magnitude - spring.restLength;
            Vector3 f = dirAToB * displacement * spring.ks;
            spring.objA.force += f;
            spring.objB.force -= f;
        }

        // Perform verlet integration
        float dt = Time.fixedDeltaTime;
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                for (int k = 0; k < N; k++) {
                    if (i == 0 && j == 0 && k == 0) // Hack for pinned node
                        continue;

                    Mass m = spheres[i][j][k].GetComponent<Mass>();
                    Vector3 tempStorage = m.position;
                    Vector3 accel = m.force / m.mass;
                    m.position += (1 - damping) * (m.position - m.prevPosition) + accel*dt*dt;
                    m.prevPosition = tempStorage;

                    // Transfer mass script's position to actual object's position
                    spheres[i][j][k].transform.position = m.position;
                }
            }
        }
    }

    // Called every frame
    void Update() {
        // Draw lines
        if (debugOn) {
            foreach (Spring spring in springs) {
                Debug.DrawLine(spring.objA.position, spring.objB.position, Color.red);
            }
        }
    }

    // Adds a spring between the two coordinates to the list
    void AddSpring(int i1, int j1, int k1, int i2, int j2, int k2) {
        if (InBounds(i1, j1, k1) && InBounds(i2, j2, k2)) {
            Spring spring = new Spring();
            spring.objA = spheres[i1][j1][k1].GetComponent<Mass>();
            spring.objB = spheres[i2][j2][k2].GetComponent<Mass>();
            spring.ks = springTension;
            spring.restLength = (spring.objA.position - spring.objB.position).magnitude;
            springs.Add(spring);
        }
    }

    // Checks if coordinates are in bounds
    bool InBounds (int i, int j, int k) {
        return 0 <= i && i < N && 0 <= j && j < N && 0 <= k && k < N;
    }
}

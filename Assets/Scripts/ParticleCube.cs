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

    public Vector3 size; // Size of the total cube
    public int xN, yN, zN; // # of spheres along each axis
    public float springTension; // Tension of spring between masses
    public float damping; // Damping term for motion
    public float mass; // Mass of each sphere
    public float friction; // Friction of each sphere
    public float maxDisplacement; // Maximum displacement before a spring snaps, defined as a multiplier on rest length.
    public float particleSize; // Size of each sphere. A size of 1 leaves no gaps.

    private List<List<List<GameObject>>> spheres = new List<List<List<GameObject>>>();
    private List<Spring> springs = new List<Spring>();
    private List<Spring> brokenSprings = new List<Spring>();

	// Use this for initialization
	void Start () {
        // Compute distances between spheres and size
        float yDistance = size.y / yN, xDistance = size.x / xN, zDistance = size.z / zN;
        float trueSize = Mathf.Min(yDistance, xDistance, zDistance) * particleSize;

        // Initialize spheres and place into data structure
		for (int i = 0; i < yN; i++) {
            spheres.Add(new List<List<GameObject>>());
            float y = yDistance * i;
            for (int j = 0; j < xN; j++) {
                spheres[i].Add(new List<GameObject>());
                float x = xDistance * j;
                for (int k = 0; k < zN; k++) {
                    float z = zDistance * k;

                    // Create object
                    Vector3 pos = new Vector3(x, y, z);
                    GameObject sphere = Instantiate(spherePrefab);
                    sphere.transform.parent = this.transform;
                    sphere.transform.localPosition = pos;
                    sphere.transform.localScale = Vector3.one * trueSize;

                    // Initialize mass component
                    Mass m = sphere.AddComponent<Mass>();
                    m.position = sphere.transform.position;
                    m.prevPosition = m.position;
                    m.mass = mass;
                    m.friction = friction;

                    // Add to list
                    spheres[i][j].Add(sphere);
                }
            }
        }

        // Initialize springs
        for (int i = 0; i < yN; i++) {
            for (int j = 0; j < xN; j++) {
                for (int k = 0; k < zN; k++) {
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
        brokenSprings.Clear();
        int brokenSpringCounter = 0;

        // Clear out forces, add gravity
        for (int i = 0; i < yN; i++) {
            for (int j = 0; j < xN; j++) {
                for (int k = 0; k < zN; k++) {
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

            // TEMP: check if spring should break
            if (Mathf.Abs(displacement) > maxDisplacement * spring.restLength) {
                brokenSprings.Add(spring);
                brokenSpringCounter++;
                //Debug.Log("Snap!");
                continue;
            }

            Vector3 f = dirAToB * displacement * spring.ks;
            spring.objA.force += f;
            spring.objB.force -= f;
        }

        // Remove springs that were slated to break
        if (brokenSpringCounter > 0)
            springs.RemoveAll(x => brokenSprings.Contains(x));

        // Perform verlet integration
        float dt = Time.fixedDeltaTime;
        for (int i = 0; i < yN; i++) {
            for (int j = 0; j < xN; j++) {
                for (int k = 0; k < zN; k++) {
                    //if (i == 0 && j == 0 && k == 0) // Hack for pinned node
                    //    continue;

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
        // Draw lines representing every spring
        if (debugOn) {
            foreach (Spring spring in springs) {
                Debug.DrawLine(spring.objA.position, spring.objB.position, Color.red);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            Mass.STATE = "TEMP_ENABLED";
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
        return 0 <= i && i < yN && 0 <= j && j < xN && 0 <= k && k < zN;
    }
}

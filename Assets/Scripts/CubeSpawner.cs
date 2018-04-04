using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Spawns and initializes a size N cube of spheres.
 * Spawns them from the top left.
 */
public class CubeSpawner : MonoBehaviour {

    public static int N; // # of spheres on each edge
    public GameObject spherePrefab; // Prefab for the individual sphere
    public float distanceRatio; // Distance between spheres defined as a multiplier on the sphere size.

    private GameObject[][][] spheres = new GameObject[N][][];

	// Use this for initialization
	void Start () {

        // Initialize spheres and place into data structure
        float trueDistance = distanceRatio * transform.localScale.x;
		for (int i = 0; i < N; i++) {
            float y = transform.position.y + trueDistance * i;
            for (int j = 0; j < N; j++) {
                spheres[i] = new GameObject[N][];
                float x = transform.position.x + trueDistance * j;
                for (int k = 0; k < N; k++) {
                    spheres[i][j] = new GameObject[N];
                    float z = transform.position.z + trueDistance * k;
                    Vector3 pos = new Vector3(x, y, z);
                    spheres[i][j][k] = Instantiate(spherePrefab, pos, Quaternion.identity);
                }
            }
        }

        // 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

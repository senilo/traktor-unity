using UnityEngine;
using System.Collections;

public class BallPlacer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + Terrain.activeTerrain.transform.position.y + 1.0f;
        transform.position = pos;
        rb.isKinematic = false;
    }
}

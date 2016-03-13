using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    
    Vector3 diff;
	// Use this for initialization
	void Start () {
        diff = transform.position - transform.parent.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 traktorRotation = transform.parent.rotation.eulerAngles;
        Vector3 rotatedDiff = Quaternion.Euler(0, traktorRotation.y, 0)*diff;
        transform.position = transform.parent.position + rotatedDiff;
        transform.rotation = Quaternion.LookRotation(-rotatedDiff);
	}
    
}

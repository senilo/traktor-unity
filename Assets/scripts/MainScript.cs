using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {

    // Use this for initialization
    const int LayerIgnoreCollision = 9;
    const int LayerDefault = 0;
	void Start () {
        Physics.IgnoreLayerCollision(LayerDefault, LayerIgnoreCollision);
	}
	
}

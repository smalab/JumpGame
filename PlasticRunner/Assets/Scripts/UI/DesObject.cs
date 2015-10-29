using UnityEngine;
using System.Collections;

public class DesObject : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	void OnTriggerEnter(Collider col) {
		if (gameObject.layer == LayerMask.NameToLayer("Sign")) {
            GameObject.Destroy(col.gameObject);
		}
	}
}

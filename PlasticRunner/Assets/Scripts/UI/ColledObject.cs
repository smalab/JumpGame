using UnityEngine;
using System.Collections;

public class ColledObject : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer == LayerMask.NameToLayer("Sign")) {
			GameObject.Destroy(col.gameObject);
		}
	}
}

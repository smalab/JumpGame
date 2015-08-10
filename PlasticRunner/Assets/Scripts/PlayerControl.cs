using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate( new Vector3(0.0f, 0.0f, 3.0f * Time.deltaTime) );
	}
}

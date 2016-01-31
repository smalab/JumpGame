using UnityEngine;
using System.Collections;

public class PlayerButtom : MonoBehaviour {
	private bool mIsLanded = false;

	public bool IsLanded {
		get {return mIsLanded;}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.layer == LayerMask.NameToLayer("Floor")) {
			mIsLanded = true;
		}
	}

	void OnTriggerExit(Collider collider){
		if (collider.gameObject.layer == LayerMask.NameToLayer("Floor")) {
			mIsLanded = false;
		}
	}
	
}

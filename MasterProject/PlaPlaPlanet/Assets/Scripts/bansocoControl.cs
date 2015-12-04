using UnityEngine;
using System.Collections;

public class bansocoControl : MonoBehaviour {

	void Start () {
	}
	
	void Update () {
	}

	public void getoff(){
		gameObject.AddComponent<Rigidbody>(); // 型指定--.
		gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		this.gameObject.transform.parent = null;
	}


}

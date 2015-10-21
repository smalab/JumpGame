using UnityEngine;
using System.Collections;

public class SignCreator : MonoBehaviour {
	public GameObject signPrefab;

	// Use this for initialization
	void Start () {
		GameObject s_start = GameObject.Instantiate(this.signPrefab) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

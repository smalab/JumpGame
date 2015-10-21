using UnityEngine;
using System.Collections;

public class PlayerCreator : MonoBehaviour {
	public GameObject Ply;

	// Use this for initialization
	void Start () {
		GameObject start = GameObject.Instantiate(this.Ply) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

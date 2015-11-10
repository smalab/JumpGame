using UnityEngine;
using System.Collections;

public class CloneSign : MonoBehaviour {
    public GameObject mCloneobj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        mCloneobj = GameObject.Instantiate(mCloneobj) as GameObject;
	}
}

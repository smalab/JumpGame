using UnityEngine;
using System.Collections;

public class CloneSign : MonoBehaviour {
    public GameObject mCloneobj;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        mCloneobj = Instantiate(mCloneobj) as GameObject;
        mCloneobj.transform.Rotate(new Vector3(0f, 90f, 0f));
    }
}

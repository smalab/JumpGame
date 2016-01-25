using UnityEngine;
using System.Collections;

public class TextMeshTester : MonoBehaviour {
    TextMesh mTextMesh = null;
	// Use this for initialization
	void Start () {
        mTextMesh = GetComponent<TextMesh>();
        mTextMesh.text = "A";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

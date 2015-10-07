using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	// Update is called once per frame
	public void BackTitle () {
		Application.LoadLevel("TitleScene");
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			Application.LoadLevel("GameScene");
		}
	}
}

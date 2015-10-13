using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	public void BackTitle () {
		Application.LoadLevel("TitleScene");
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			Application.LoadLevel("GameScene");
		}
	}
}

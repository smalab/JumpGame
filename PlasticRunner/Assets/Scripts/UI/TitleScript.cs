using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	public void BackTitle () {
		Application.LoadLevel("TitleScene");
	}

	public void StartGame () {
		Application.LoadLevel("GameScene");
	}
}

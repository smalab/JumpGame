using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {
	public void BackTitle () {
		Application.LoadLevel("Title");
	}

	public void StartGame () {
		Application.LoadLevel("GameMain");
	}
}

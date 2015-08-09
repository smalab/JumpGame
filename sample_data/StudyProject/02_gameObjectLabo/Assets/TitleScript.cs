using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour {

	void Update() {
		if(Input.GetMouseButtonDown(0)) { // 左クリックされたら.
			Application.LoadLevel("studyScene"); // studySceneに移行する.
		}
	}
	void OnGUI() {
		// 画面に「title」と表示する.
		GUI.Label(new Rect(Screen.width/2, Screen.height/2, 128, 32), "title");
	}
}

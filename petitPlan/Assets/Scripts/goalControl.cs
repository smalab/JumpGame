using UnityEngine;
using System.Collections;

public class goalControl : MonoBehaviour {
	// ぶつかっているか、いないかを格納
	private bool is_collided = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// 他のGameObjectとぶつかっている間、呼ばれ続ける
	void OnCollisionStay(Collision other) {
			this.is_collided = true;
	}
	
	void OnGUI() {
		if(is_collided) { // ぶつかっているなら
			// 画面に「成功」を表示する
			GUI.Label(new Rect(Screen.width/2, 80, 100, 20), "成功");
		}
	}
	
}

using UnityEngine;
using System.Collections;

public class goalControl : MonoBehaviour {
	// ぶつかっているか、いないかを格納
	private bool is_collided = false;
	
	// ゴールの位置を設定
	public float GOAL_MIN = 5.0f;	// 最小値
	public float GOAL_MAX = 10.0f;	// 最大値
	
	// Use this for initialization
	void Start () {
		// GOAL_MIN － MAXの間のランダムな値を取得
		float rnd = Random.Range(GOAL_MIN, GOAL_MAX);
		
		// GoalのX位置をランダムな値に
		this.transform.position = new Vector3(rnd, -1.0f, 0.0f);
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

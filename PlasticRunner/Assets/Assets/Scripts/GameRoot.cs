using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {
	public float step_timer = 0.0f;	// 経過時間を格納

	// Update is called once per frame
	void Update () {
	this.step_timer += Time.deltaTime;
	}

	public float getPlayTime() {
		float time;
		time = this.step_timer;
		retun(time);
	}
}

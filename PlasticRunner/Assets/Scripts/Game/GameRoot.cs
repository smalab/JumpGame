using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {
	public float step_timer = 0.0f;	// 経過時間を格納
	private PlayerControl player = null;

	void Start() {
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}

	// Update is called once per frame
	void Update () {
		this.step_timer += Time.deltaTime;

		if(this.player.isPlayEnd()) {
			Application.LoadLevel("TitleScene");
		}
	}

	public float getPlayTime() {
		float time;
		time = this.step_timer;
		return(time);
	}
}

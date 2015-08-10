using UnityEngine;
using System.Collections;

public class GameRootScript : MonoBehaviour {

	public GameObject prefab = null;

	private AudioSource audio;
	public AudioClip jumpSound;

	public Texture2D icon = null;
	public static string mes_text = "test";

	void Start () {
		this.audio = this.gameObject.AddComponent<AudioSource>();
		// jumpSoundに格納した音を鳴らすように準備.
		this.audio.clip = this.jumpSound;
		// ループ再生（繰り返し再生）を無効に.
		this.audio.loop = false;
	}



	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			// Instantiate(prefab);
			// prefab変数から作られたGameObjectを取得.
			GameObject go =
				GameObject.Instantiate(this.prefab) as GameObject;
			// 取得したGameObjectの設定を変更.
			go.transform.position =
				new Vector3(Random.Range(-2.0f, 2.0f), 1.0f, 1.0f);

			this.audio.Play(); // audioに入っている音を再生.
		}

	}


	void OnGUI() {
		GUI.DrawTexture(new Rect(Screen.width / 2, 64, 64, 64), icon);
		GUI.Label(new Rect(Screen.width / 2, 128, 128, 32), mes_text);
	}
}

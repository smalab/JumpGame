using UnityEngine;
using System.Collections;

// 空のAudio Sourceを作っておく
[RequireComponent (typeof (AudioSource))]

public class GetMicVolume : MonoBehaviour {
	// Use this for initialization
	void Start () {
		// 空のAudio Sourceを取得
		var audio = GetComponent<AudioSource>();

		// Audio SourceのAudio Clipをマイク入力に設定
		// 引数は、デバイス名(nullならデフォルト)、ループ、何秒取るか、サンプリング周波数
		audio.clip = Microphone.Start(null, true, 10, 44100);

		// マイクがReadyになるまで待機
		while (Microphone.GetPosition(null) <= 0) {}

		// 再生開始(録った先から再生、スピーカーから出力するとハウリングするので注意)
		audio.Play();
	}

		// Update is called once per frame
	void Update () {
		float vol = GetAveragedVolume();
		Debug.Log(vol);
	}

		float GetAveragedVolume() {
		var audio = GetComponent<AudioSource>();
		float[] data = new float[256];
		float a = 0;
		audio.GetOutputData(data, 0);
		foreach(float s in data) {
			a += Mathf.Abs(s);
		}
		return a/256.0f;
	}
	
}

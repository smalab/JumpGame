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
		audio.clip = Microphone.Start(null, false, 10, 44100);

		// マイクがReadyになるまで待機
		while (Microphone.GetPosition(null) <= 0) {}

		// 再生開始(録った先から再生、スピーカーから出力するとハウリングするので注意)
		audio.Play();
	}
}

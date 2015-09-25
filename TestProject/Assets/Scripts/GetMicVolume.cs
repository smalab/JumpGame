// 参考URL:http://unity-michi.com/post-396/

using UnityEngine;
using System.Collections;

// 空のAudio Sourceを作っておく
[RequireComponent (typeof (AudioSource))]
[DisallowMultipleComponent]	// 複数アタッチを禁止

public class GetMicVolume : MonoBehaviour {
	// 外部から現在の音量を読み取る
	// 最大値はsensitivityによる
	public float GetLoudness() {
		return loudness;
	}
	
	public float sensitibity = 100;	// 感度,音量の最大値
	
	float loudness;    // 音量
	float lastLoudness;// 前フレームの音量
	
	[Range(0,0.95f)]   // 最大1に出来ると全く変動しなくなるのを防ぐ
	public float lastLoudnessInfluence;// 前フレームの影響度合い

	// Use this for initialization
	// void Start () {
	// }

	// void InitRecord() {
	// 	var audio = GetComponent<AudioSource>();
	// 	audio.clip = Microphone.Start(null, true, 10, 44100);
	// 	audio.loop = true;
	// 	audio.mute = true;
	// 	while(!(Microphone.GetPosition("") > 0)){}
	// 	audio.Play();
	// }

	// Use this for initialization
	void Start () {
		var audio = GetComponent<AudioSource>();
		audio.clip = Microphone.Start(null, true, 999, 44100);
		audio.loop = true;
		audio.mute = true;
		while (!(Microphone.GetPosition("") > 0)) {}
		audio.Play();
	}

	// Update is called once per frame
	void Update () {
		CalcLoudness();
		Debug.Log(loudness);
	}

	// 現フレームの音量を計算する
	// マイク感度が過剰な時はlastLoudnessInfluenceを上げたりして調整すること
	void CalcLoudness() {
		lastLoudness = loudness;
		loudness = GetAveragedVolume() * sensitibity
			* (1 - lastLoudnessInfluence) + lastLoudness * lastLoudnessInfluence;
	}

	float GetAveragedVolume() {
		float[] data = new float[256];
		float a = 0;
		var audio = GetComponent<AudioSource>();
		audio.GetOutputData(data, 0);
		foreach (float s in data) {
			a += Mathf.Abs(s);
		}
		return a/256;
	}

}

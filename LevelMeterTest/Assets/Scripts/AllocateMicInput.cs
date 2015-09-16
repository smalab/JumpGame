using UnityEngine;
using System.Collections;

public class AllocateMicInput : MonoBehaviour {

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

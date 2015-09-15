using UnityEngine;
using System.Collections;

public class GetAudioClipData : MonoBehaviour {
	public AudioClip clip;
	public int lenghthYouWant;

	// Use this for initialization
	void Start () {
		var data = new float[lenghthYouWant];
		clip.GetData(data, 0);
	}
}

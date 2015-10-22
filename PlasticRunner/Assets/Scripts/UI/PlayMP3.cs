using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {
	public float speed = 0.07f;
	public AudioClip audioclip;
	AudioSource audiosource;

	// Use this for initialization
	void Start () {
		audiosource = gameObject.GetComponent<AudioSource> ();
		audiosource.clip = audioclip;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider col) {
		// if (col.gameObject.name == "Canvas Sign") {
			// audiosource.PlayOneShot (audioclip);
			// GameObject.Destroy(Sign);
		//}

		if (col.gameObject.layer == LayerMask.NameToLayer("Sign")) {
			audiosource.PlayOneShot (audioclip);
			GameObject.Destroy(col.gameObject);
		}
	}
}

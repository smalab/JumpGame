using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {
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
		if (col.gameObject.layer == LayerMask.NameToLayer("Event")) {
			audiosource.PlayOneShot (audioclip);
			//GameObject.Destroy(this.gameObject);
		}
	}

    //public void Playmp3 ()
    //{
    //   audiosource.PlayOneShot(audioclip);
    //}
}
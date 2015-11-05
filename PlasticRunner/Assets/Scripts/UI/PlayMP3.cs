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
	void Update () {}

    public void Playmp3 ()
    {
       audiosource.PlayOneShot(audioclip);
    }
}
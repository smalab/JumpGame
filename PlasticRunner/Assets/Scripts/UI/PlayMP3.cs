using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {
    public AudioClip[] mAudioclip;
    AudioSource mAudiosource;

	// Use this for initialization
	void Start () {
        mAudioclip = new AudioClip[26];
        for(int i = 0; i < 26; i++)
        {
            mAudioclip[i] = Resources.Load("Sounds/" + i.ToString()) as AudioClip;
        }
		mAudiosource = gameObject.GetComponent<AudioSource> ();

    }
	
	// Update is called once per frame
	void Update () {}

    public void Playmp3 ()
    {
        mAudiosource.clip = mAudioclip[LoadTextData.idx];
        mAudiosource.PlayOneShot(mAudiosource.clip);     
    }
}
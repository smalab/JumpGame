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
            //アルファベットの音声を配列に格納
            mAudioclip[i] = Resources.Load("Sounds/" + i.ToString()) as AudioClip;
        }
		mAudiosource = gameObject.GetComponent<AudioSource> ();

    }
	
	// Update is called once per frame
	void Update () {}

    public void PlayAlphaMp3 ()
    {
        //看板の文字に応じた音声の再生
        mAudiosource.clip = mAudioclip[LoadTextData.idx];
        mAudiosource.PlayOneShot(mAudiosource.clip);
    }
}
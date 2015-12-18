using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {
    //AudioClip mAudioclip;
	AudioSource mAudiosource;
    //LoadTextData mLoadChr = null;
    //string mLoadSound = "Sounds/";

	// Use this for initialization
	void Start () {
		mAudiosource = gameObject.GetComponent<AudioSource> ();
        //mLoadChr = GetComponent<LoadTextData>();
        //mAudiosource.clip = /*mAudioclip*/ Resources.Load("Sounds/A") as AudioClip;
    }
	
	// Update is called once per frame
	void Update () {}

    public void Playmp3 ()
    {
        Debug.Log(LoadTextData.mText.text);
        //mAudiosource.clip = Resources.Load("Sounds/" + LoadTextData.mText.text) as AudioClip;
        mAudiosource.PlayOneShot(Resources.Load("Sounds/" + LoadTextData.mText.text) as AudioClip);
       
    }
}
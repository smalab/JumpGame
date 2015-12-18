using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {
    //AudioClip mAudioclip;
	AudioSource mAudiosource;
    LoadTxtData mLoadChr = null;
    string mLoadSound = "";

	// Use this for initialization
	void Start () {
		mAudiosource = gameObject.GetComponent<AudioSource> ();
        mLoadChr = GetComponent<LoadTxtData>();
		//mAudiosource.clip = /*mAudioclip*/ Resources.Load("Sounds/A") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(mLoadSound);
    }

    public void Playmp3 ()
    {
        mLoadSound = mLoadChr.GetLinesIndex;
        mAudiosource.clip = Resources.Load(mLoadSound) as AudioClip;
        mAudiosource.PlayOneShot(mAudiosource.clip);
    }
}
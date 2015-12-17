using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RecordAudio : MonoBehaviour {
    private AudioSource mRecordAudio = null;

	// Use this for initialization
	void Start () {
        mRecordAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

    void StartRecording()
    {
        mRecordAudio.clip = Microphone.Start(null, false, 3, 44100);
    }
}

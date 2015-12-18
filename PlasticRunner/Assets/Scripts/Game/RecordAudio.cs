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
        //看板に表示されたアルファベットに応じてmp3ファイルをロード
        mRecordAudio.clip = Microphone.Start(null, false, 3, 44100);
    }
}

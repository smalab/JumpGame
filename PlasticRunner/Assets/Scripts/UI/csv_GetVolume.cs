using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

public class csv_GetVolume : MonoBehaviour {

	private float time = 0.0f;
    GetMicInput AverageVoice = null;

	// Use this for initialization
	void Start () {
        AverageVoice = GetComponent<GetMicInput>();
		if (File.Exists("Assets/log.csv")) {
			// Debug.Log("fileatta");
			FileStream f = new System.IO.FileStream("Assets/log.csv", FileMode.Append, FileAccess.Write);
			Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
			StreamWriter writer = new StreamWriter(f, utf8Enc);
			writer.WriteLine("");
			writer.Close();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//0.2s gotoni data wo toru
		time += Time.deltaTime;
		if (time >= 0.1f) {
			logSave ( GetMicInput.loudness.ToString(), AverageVoice.aveVoice.ToString() );
			time = 0.0f;
		}

	}

	public void logSave(string PlayVolume, string VoiceVolume){
		FileStream f = new FileStream("Assets/log.csv", FileMode.Append, FileAccess.Write);
		Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
		StreamWriter writer = new StreamWriter(f, utf8Enc);
		writer.Write(PlayVolume + ",");
        writer.Write(VoiceVolume + ",");
        writer.Write(" " + ",");
		writer.Close();
	}
}

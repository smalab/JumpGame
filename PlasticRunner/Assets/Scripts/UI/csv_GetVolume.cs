using UnityEngine;
using System.IO;
using System.Text;

public class csv_GetVolume : MonoBehaviour {

	//private float mTime = 0.0f;
    GetMicInput mAverageVoice = null;

	// Use this for initialization
	public void Start () {
        mAverageVoice = GetComponent<GetMicInput>();
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
		// mTime += Time.deltaTime;
		// if (mTime >= 0.1f) {
		// 	logSave ( GetMicInput.loudness.ToString(), AverageVoice.aveVoice.ToString() );
		// 	mTime = 0.0f;
		// }


	}

	public void LogSave(){
		FileStream mLogSave = new FileStream("Assets/log.csv", FileMode.Append, FileAccess.Write);
		Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
		StreamWriter writer = new StreamWriter(mLogSave, utf8Enc);
		writer.Write(GetMicInput.loudness.ToString() + ",");
        writer.Write(mAverageVoice.aveVoice.ToString() + ",");
        writer.Write(" " + ",");
		writer.Close();
	}
}

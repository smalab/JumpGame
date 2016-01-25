using UnityEngine;
using System.IO;
using System;
using System.Text;

public class csv_GetVolume : MonoBehaviour {

	//private float mTime = 0.0f;
    public static GetMicInput mAverageVoice = null;

	// Use this for initialization
	public void Start () {

		if (File.Exists("Assets/log.csv")) {
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
		//mTime += Time.deltaTime;
		//if (PlayerControl.step == PlayerControl.STEP.JUMP) {
		//	LogSave (GetMicInput.loudness.ToString());
		//}
	}

    public void LogSave(string aVolume)
    {
        FileStream mLogSave = new FileStream("Assets/log.csv", FileMode.Append, FileAccess.Write);
        Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
        StreamWriter writer = new StreamWriter(mLogSave, utf8Enc);
        writer.Write(aVolume /*+ ","*/);
        writer.WriteLine("");
        writer.Close();
    }

    //public void logSave(string argVolume)
    //{
    //    StreamWriter sw;
    //    FileStream fs;
    //    fs = new FileStream(Application.dataPath + "log.csv", FileMode.Append, FileAccess.Write);
    //    Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
    //    sw = new StreamWriter(fs, utf8Enc);
    //    sw.Write(argVolume + "");
    //    //sw.Flush();
    //    sw.Close();
    //}

}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
	private float mTime = 20.0f;
	public float remainTime {
		get {return mTime;}
	}

	// Use this for initialization
	void Start () {
		// float型からint型へcastし、String型に変換して表示
		GetComponent<Text>().text = ((int) mTime).ToString();
	}

	// Update is called once per frame
	void Update () {
		// 時間を1秒ずつ減らしていく
		mTime -= Time.deltaTime;

		// マイナス表示をしないように
		if (mTime < 0) mTime = 0.0f;
		GetComponent<Text>().text = ((int) mTime).ToString();
	}
}

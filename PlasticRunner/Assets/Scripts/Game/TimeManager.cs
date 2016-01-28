using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
	private float m_time = 3.0f;
	public float remainTime {
		get {return m_time;}
	}
	public GameObject gameOverText;
	public GameObject backTitlebtn;

	// Use this for initialization
	void Start () {
		// float型からint型へcastし、String型に変換して表示
		GetComponent<Text>().text = ((int) m_time).ToString();
	}

	// Update is called once per frame
	void Update () {
		// 時間を1秒ずつ減らしていく
		m_time -= Time.deltaTime;

		// マイナス表示をしないように
		if (m_time < 0) m_time = 0.0f;
		GetComponent<Text>().text = ((int) m_time).ToString();
	}
}



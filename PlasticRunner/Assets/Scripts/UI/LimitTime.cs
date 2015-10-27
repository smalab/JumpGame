using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LimitTime : MonoBehaviour {
	private float m_time = 300.0f;
	public float remainTime {
		get {return m_time;}
	}
	public GameObject gameOverText;
	public GameObject backTitlebtn;

	// Use this for initialization
	void Start () {
		gameOverText.SetActive(false);
		backTitlebtn.SetActive(false);
		// float型からint型へcastし、String型に変換して表示
		GetComponent<Text>().text = ((int) m_time).ToString();
	}

	// Update is called once per frame
	void Update () {
		// 時間を1秒ずつ減らしていく
		m_time -= Time.deltaTime;

		if (m_time < 0) {
			StartCoroutine("GameOver");
		}

		// マイナス表示をしないように
		if (m_time < 0) m_time = 0.0f;
		GetComponent<Text>().text = ((int) m_time).ToString();
	}

	IEnumerator GameOver () {
		gameOverText.SetActive(true);
		backTitlebtn.SetActive(true);
		yield return new WaitForSeconds(2.0f);
	}
}



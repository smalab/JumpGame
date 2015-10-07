using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LimitTime : MonoBehaviour {
	private float time= 5;
	public GameObject gameOverText;
	public GameObject backTitlebtn;

	// Use this for initialization
	void Start () {
		gameOverText.SetActive(false);
		backTitlebtn.SetActive(false);
		// float型からint型へcastし、String型に変換して表示
		GetComponent<Text>().text = ((int)time).ToString();
	}

	// Update is called once per frame
	void Update () {
		// 時間を1秒ずつ減らしていく
		time -= Time.deltaTime;

		if (time < 0) {
			StartCoroutine("GameOver");
		}

		// マイナス表示をしないように
		if (time < 0) time = 0;
		GetComponent<Text>().text = ((int)time).ToString();
	}

	IEnumerator GameOver () {
		gameOverText.SetActive(true);
		backTitlebtn.SetActive(true);
		yield return new WaitForSeconds(2.0f);

		if (Input.GetMouseButtonDown(0)) {
			Application.LoadLevel ("TitleScene");
		}
	}

}

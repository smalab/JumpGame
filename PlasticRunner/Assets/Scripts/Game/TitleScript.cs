using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour {
	public void BackTitle () {
        SceneManager.LoadScene("Title");
	}

	public void StartGame () {
        SceneManager.LoadScene("GameMain");
	}
}

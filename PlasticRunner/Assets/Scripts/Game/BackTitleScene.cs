using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackTitleScene : MonoBehaviour {
    public GameObject mBackTitleButton = null;

    public void BackToTitleScene()
    {
        mBackTitleButton.SetActive(true);
        SceneManager.LoadScene("Title");
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class tmpBackTitle : MonoBehaviour {
    public GameObject mBackTitleButton = null;

    public void BackTitleScene()
    {
        mBackTitleButton.SetActive(true);
        SceneManager.LoadScene("Title");
    }
}

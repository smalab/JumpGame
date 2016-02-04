using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int mScore = 100;
    public int proScore
    {
        get {return mScore;}
    }

	// Use this for initialization
	void Start () {
        GetComponentInChildren<Text>().text = mScore.ToString();
	}

	// Update is called once per frame
	void Update () {}

    public void CallAddScore()
    {

        StartCoroutine("AddScore");
    }

    public IEnumerator AddScore()
    {
        for (int i = 0; i < 50; i++)
        {
            mScore += 2;
            GetComponentInChildren<Text>().text = mScore.ToString();
            yield return new WaitForSeconds(0.01f);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int mScore = 0;
    
	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = mScore.ToString();
	}
	
	// Update is called once per frame
	void Update () {}

    public void CallAddScore()
    {

        StartCoroutine("AddScore");
    }

    IEnumerator AddScore()
    {
        for (int i = 0; i < 50; i++)
        {
            mScore += 2;
            GetComponent<Text>().text = mScore.ToString();
            yield return new WaitForSeconds(0.01f);
        }
    }
}

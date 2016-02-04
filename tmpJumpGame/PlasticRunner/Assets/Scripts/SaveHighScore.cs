using UnityEngine;
using System.Collections;

public class SaveHighScore : MonoBehaviour {
    private HighScoreManager mSaveScore = null;
    private ScoreManager mScoreManager = null;
	// Use this for initialization
	void Start () {
        mSaveScore = GameObject.Find("RankingManager").GetComponent<HighScoreManager>();
        mScoreManager = GameObject.Find("RankingManager").GetComponent<ScoreManager>();
	}

	// Update is called once per frame
	void Update () {
        TestScore();
	}

    void TestScore()
    {
        mSaveScore.SaveRanking(mScoreManager.proScore);
    }
}

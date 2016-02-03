using UnityEngine;
using System.Collections;

public class HighScoreManager : MonoBehaviour
{
    private string mRankingPrefKey = "ranking";
    private int mRankingNum = 5;
    private int[] mRankedScore = null;

    // Use this for initialization
    void Start() {}

    // Update is called once per frame
    void Update() {}

    //ランキングをPlayerPrefsから取得
    //mRankedScoreに格納
    void GetRanking()
    {
        var _ranking = PlayerPrefs.GetString(mRankingPrefKey);
        if(_ranking.Length > 0)
        {
            string[] _score = _ranking.Split(","[0]);
            mRankedScore = new int[mRankingNum];
            for(int i = 0; i < _score.Length && i < mRankingNum; i++)
            {
                mRankedScore[i] = int.Parse(_score[i]);
            }
        }
    }

    //新しくスコアを保存する
    void SaveRanking(int aNewScore)
    {
        if (mRankedScore.Length != 0)
        {
            int _tmp = 0;
            for(int i= 0; i < mRankedScore.Length; i++)
            {
                if(mRankedScore[i] < aNewScore)
                {
                    _tmp = mRankedScore[i];
                    mRankedScore[i] = aNewScore;
                    aNewScore = _tmp;
                }
            }
        }
        else
        {
            mRankedScore[0] = aNewScore;
        }
    }

    //ランキングの削除
    
}

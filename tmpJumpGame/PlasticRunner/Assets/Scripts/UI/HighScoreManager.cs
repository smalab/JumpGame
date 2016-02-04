using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreManager : MonoBehaviour
{
    private string mRankingPrefKey = "ranking";
    private int mRankingNum = 5;
    private int[] mRankedScore = null;

    public Text mHighScoreText = null; //ハイスコアを表示させるテキストオブジェクト
    private int mHighScore; //ハイスコア
    private string mHighScoreKey = "HIGH SCORE"; //ハイスコア保存キー

    // Use this for initialization
    void Start()
    {
            mHighScoreText = GetComponent<Text>();
            //保存しているハイスコアをキーで呼び出して取得
            //なければ0を返す
            mHighScore = PlayerPrefs.GetInt(mHighScoreKey, 0);
            mHighScoreText.text = "HighScore: " + mHighScore.ToString();
    }

    // Update is called once per frame
    void Update() { }

    //ランキングをPlayerPrefsから取得
    //mRankedScoreに格納
    public void GetRanking()
    {
        var _ranking = PlayerPrefs.GetString(mRankingPrefKey);
        if (_ranking.Length > 0)
        {
            string[] _score = _ranking.Split(","[0]);
            mRankedScore = new int[mRankingNum];
            for (int i = 0; i < _score.Length && i < mRankingNum; i++)
            {
                mRankedScore[i] = int.Parse(_score[i]);
            }
        }
    }

    //新しくスコアを保存する
    public void SaveRanking(int aNewScore)
    {
        if (mRankedScore.Length != 0)
        {
            int _tmp = 0;
            for (int i = 0; i < mRankedScore.Length; i++)
            {
                if (mRankedScore[i] < aNewScore)
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
    public void DeleteRanking()
    {
        PlayerPrefs.DeleteKey(mRankingPrefKey);
    }

    //ランキングの表示
    public void ShowRanking()
    {
        string[] RankingString = null;
        for (int i = 0; i < mRankedScore.Length; i++)
        {
            RankingString[i] = mRankedScore[i].ToString();
        }
    }

    void ChangeHighScore()
    {

    }
}

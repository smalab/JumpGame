using UnityEngine;
using System.Collections;

// スコアー管理.
// コンボの計算等もここでやる予定.
public class ScoreControl : MonoBehaviour {

	public struct Score {
		public int		score;		// 点数.
		public int		coins;		// コインの枚数.
	};

	//private ScoreDisp		score_disp = null;			// スコアー表示.

	// ================================================================ //

	//private static float	POS_X 	= 0.0f;			// 表示位置.
	//private static float	POS_Y 	= 10.0f;			// 表示位置.

	private Score		score;							// 現在のスコアー.

	// ================================================================ //

	public static ScoreControl	getInstance()
	{
		ScoreControl	score_control = GameObject.FindGameObjectWithTag("GameRoot").GetComponent<ScoreControl>();
		return(score_control);
	}

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.score.score = 100;
		this.score.coins = 0;

		//this.score_disp = GameObject.FindGameObjectWithTag("Score Disp").GetComponent<ScoreDisp>();
	}
	
	void	Update()
	{
	
	}

	void	OnGUI()
	{
		//this.score_disp.dispNumber(new Vector2(POS_X, POS_Y), this.score.score, 64.0f);
	}


	// ================================================================ //

	// 現在のスコアーを取得する.
	public ScoreControl.Score	getCurrentScore()
	{
		return(this.score);
	}
}

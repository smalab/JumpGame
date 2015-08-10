using UnityEngine;
using System.Collections;

// シーンをまたいで使いたいパラメーター.
public class GlobalParam : MonoBehaviour {
	

	public struct Score {
		public float	score;		// 点数.
		public int		ignit;		// 着火数.
	};

	// ================================================================ //

	private Score		high_score;				// ハイスコア―.
	private Score		last_socre;				// 今回のスコアー.	

	// ================================================================ //

	public void		initialize()
	{
		this.high_score.score = 99;
		this.high_score.ignit = 0;

		this.last_socre.score = 99;
		this.last_socre.ignit = 0;
	}

	// ゲームオーバー時に、今回のスコアーをセットする.
	public void		setLastScore(float s, int i)
	{
		this.last_socre.score = s;
		this.last_socre.ignit = i;

		// ハイスコア―更新チェック.
		this.high_score.score = Mathf.Min(this.high_score.score, this.last_socre.score);
		this.high_score.ignit = Mathf.Max(this.high_score.ignit, this.last_socre.ignit);
	}

	// ハイスコア―を取得する.
	public Score	getHighScore()
	{
		return(this.high_score);
	}

	// 今回のスコアーを取得する.
	public Score	getLastScore()
	{
		return(this.last_socre);
	}

	// ================================================================ //

	private static	GlobalParam instance = null;

	public static GlobalParam	getInstance()
	{
		if(instance == null) {
			GameObject	go = new GameObject("GlobalParam");
			instance = go.AddComponent<GlobalParam>();
			instance.initialize();
			DontDestroyOnLoad(go);
		}
		return(instance);
	}

}

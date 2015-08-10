using UnityEngine;
using System.Collections;

// シーンをまたいで使いたいパラメーター.
public class GlobalParam : MonoBehaviour {
	
	// ================================================================ //

	private SaveData	save_data = null;

	public bool	save_data_loaded = false;				// セーブデーターをロードした？.

	private ScoreControl.Score		high_score;				// ハイスコア―.
	private ScoreControl.Score		last_socre;				// 今回のスコアー.	

	// ================================================================ //

	public void		initialize()
	{
		this.high_score.score = 100;
		this.high_score.coins = 0;

		this.last_socre.score = 100;
		this.last_socre.coins = 0;

		// セーブデーター（を、読み書きするためのオブジェクト）を作っておく.
		this.create_save_data();
	}

	// ゲームオーバー時に、今回のスコアーをセットする.
	public void		setLastScore(ScoreControl.Score last_score)
	{
		this.last_socre = last_score;

		// ハイスコア―更新チェック.
		this.high_score.score = Mathf.Max(this.high_score.score, this.last_socre.score);
		this.high_score.coins = Mathf.Max(this.high_score.coins, this.last_socre.coins);
	}

	// ハイスコア―を取得する.
	public ScoreControl.Score	getHighScore()
	{
		return(this.high_score);
	}

	// 今回のスコアーを取得する.
	public ScoreControl.Score	getLastScore()
	{
		return(this.last_socre);
	}

	// ================================================================ //
	// セーブデーター.

	// セーブデーターをロードする（初回のみ）.
	public void		loadSaveData()
	{
		if(!this.save_data_loaded) {

			// 実際にロードするのは最初の一回だけ（遅いから）.
			this.save_data.load();
			this.save_data_loaded = true;

			this.high_score.score = this.save_data.getInt("Hi-Score",  this.high_score.score);
			this.high_score.coins = this.save_data.getInt("Max-Coins", this.high_score.coins);

			foreach(SaveData.Item item in this.save_data.items) {

				DebugWindow.get().add_text(item.name + " " + item.value);
			}
		}
	}

	// セーブデーターをセーブする.
	public void		saveSaveData()
	{
		this.save_data.setInt("Hi-Score",  this.high_score.score);
		this.save_data.setInt("Max-Coins", this.high_score.coins);

		this.save_data.save();
	}

	// セーブデーターをゲットする.
	public SaveData		getSaveData()
	{
		return(this.save_data);
	}

	// セーブデーター（を、読み書きするためのオブジェクト）を作る.
	protected void		create_save_data()
	{
		this.save_data = new SaveData();

		this.save_data.addInt("Hi-Score",  -1);
		this.save_data.addInt("Max-Coins", -1);
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

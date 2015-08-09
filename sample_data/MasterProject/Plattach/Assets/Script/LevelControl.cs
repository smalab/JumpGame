using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// レベルデーター.
public class LevelData {
	
	public LevelData()
	{
		this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];

		int		block_num = (int)(Block.COLOR.MAGENTA - Block.COLOR.PINK + 1);

		for(int i = 0;i < block_num;i++) {

			this.probability[i] = 1.0f/(float)block_num;
		}
	}

	// ブロックの確率をぜんぶ 0.0 にする.
	public void		clear()
	{
		for(int i = 0;i < this.probability.Length;i++) {

			this.probability[i] = 0.0f;
		}
	}

	// ブロックの確率の合計を 1.0 にする.
	public void		normalize()
	{
		float	sum = 0.0f;

		for(int i = 0;i < this.probability.Length;i++) {

			sum += this.probability[i];
		}

		for(int i = 0;i < this.probability.Length;i++) {

			this.probability[i] /= sum;

			if(float.IsInfinity(this.probability[i])) {

				this.clear();
				this.probability[0] = 1.0f;
				break;
			}
		}
	}

	public float[]	probability;			// ブロックの出現確率
	public float	heat_time;				// 再着火受付時間.
};

// ブロック配置の管理（マップパターン。次に作るブロックのタイプを決める）.
public class LevelControl {

	private List<LevelData>		level_datas = null;		// テキストから読んだレベルデーター.

	private int		select_level = 0;					// 選択されたレベル（level_datas[] のインデックス）.

	// ================================================================ //

	public void		initialize()
	{
		this.level_datas = new List<LevelData>();
	}

	// レベルデーターをテキストファイルからよむ.
	public void		loadLevelData(TextAsset level_data_text)
	{
		// テキスト全体をひとつの文字列に.
		string		level_texts = level_data_text.text;

		// 改行コードで区切ることで、
		// テキスト全体を一行単位の配列にする.
		string[]	lines = level_texts.Split('\n');

		foreach(var line in lines) {

			if(line == "") {

				continue;
			}

			// 空白で区切って、単語の配列にする.
			string[]	words = line.Split();

			int			n = 0;
			LevelData	level_data = new LevelData();

			foreach(var word in words) {

				// "#" 以降はコメントなのでそれ以降はスキップ.
				if(word.StartsWith("#")) {

					break;
				}
				if(word == "") {

					continue;
				}

				switch(n) {

					case 0:		level_data.probability[(int)Block.COLOR.PINK]    = float.Parse(word);	break;
					case 1:		level_data.probability[(int)Block.COLOR.BLUE]    = float.Parse(word);	break;
					case 2:		level_data.probability[(int)Block.COLOR.GREEN]   = float.Parse(word);	break;
					case 3:		level_data.probability[(int)Block.COLOR.ORANGE]  = float.Parse(word);	break;
					case 4:		level_data.probability[(int)Block.COLOR.YELLOW]  = float.Parse(word);	break;
					case 5:		level_data.probability[(int)Block.COLOR.MAGENTA] = float.Parse(word);	break;
					case 6:		level_data.heat_time = float.Parse(word);	break;
				}

				n++;
			}

			if(n >= 7) {

				level_data.normalize();
				this.level_datas.Add(level_data);

			} else {

				if(n == 0) {
					// 単語がなかった＝行全体がコメントだった.
				} else {
					// パラメーターが足りない.
					Debug.LogError("[LevelData] Out of parameter.\n");
				}
			}
		}

		if(this.level_datas.Count == 0) {

			// データーがいっこもなかったとき.

			Debug.LogError("[LevelData] Has no data.\n");

			// デフォルトのデーターをいっこ追加しておく.
			this.level_datas.Add(new LevelData());
		}
	}

	// レベルをランダムに選ぶ.
	public void		selectLevel()
	{
		this.select_level = Random.Range(0, this.level_datas.Count);

		//Debug.Log("select level = " + this.select_level.ToString());
	}

	// 今のレベルの、レベルデーターを取得する.
	public LevelData	getCurrentLevelData()
	{
		return(this.level_datas[this.select_level]);
	}

	// ブロックの燃焼時間を取得する.
	public float	getVanishTime()
	{
		return(this.level_datas[this.select_level].heat_time);
	}
}

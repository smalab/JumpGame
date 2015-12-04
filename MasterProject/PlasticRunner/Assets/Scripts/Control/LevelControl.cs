using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// レベルデーター.
public class LevelData {
	
	public LevelData()
	{
		this.end_time        = 15.0f;
		this.player_speed    = 6.0f;
		this.floor_count.min = 10;
		this.floor_count.max = 10;
		this.hole_count.min  = 2;
		this.hole_count.max  = 6;
		this.height_diff.min = 0;
		this.height_diff.max = 0;
		this.coin_interval.min = 10;
		this.coin_interval.max = 10;
		this.enemy_interval.min = 30;
		this.enemy_interval.max = 30;
	}

	public struct Range {
		
		public	int		min;
		public	int		max;
	};

	public float	end_time;			// おしまいの時間.
	public float	player_speed;		// プレイヤーの走りスピード.

	public Range	floor_count;		// 床が続く数.
	public Range	hole_count;			// 穴が続く数.
	public Range	height_diff;		// 床の高さの変化.

	public Range	coin_interval;		// コインがでる間隔.
	public Range	enemy_interval;		// おじゃまキャラがでる間隔.
};

// ブロック配置の管理（マップパターン。次に作るブロックのタイプを決める）.
public class LevelControl {

	public int		HEIGHT_MAX = 20;					// 床の高さ　最高.
	public int		HEIGHT_MIN = -4;					// 床の高さ　最低.


	// 作るブロックなどの情報.
	public struct CreationInfo {

		public	Block.TYPE		block_type;				// ブロックのタイプ.
		public	int				max_count;				// 連続して作る個数.
		public	int				height;					// たかさ.

		public	int				current_count;			// 実際に作った個数.
	};

	public CreationInfo		current_block;				// 次につくるブロック.
	public CreationInfo		next_block;					// 次の次につくるブロック.
														// コインを対岸まで並べるときのため、先読みしておく.

	public	int				block_count = 0;			// 作ったブロックの数.

	public int				current_level = 0;			// 現在のレベル.

	private List<LevelData>			level_datas = new List<LevelData>();

	// ================================================================ //

	public void		initialize()
	{
		this.block_count = 0;
		this.current_level = 0;

		this.clear_next_block(ref this.current_block);
		this.clear_next_block(ref this.next_block);
	}

	// 毎フレームの更新処理.
	public void		update(float passage_time)
	{
		this.current_block.current_count++;

		
		// 同じブロックを一定個数以上作ったら、ブロックのタイプを更新.
		//
		if(this.current_block.current_count >= this.current_block.max_count) {

			this.current_block  = this.next_block;
			this.clear_next_block(ref this.next_block);

			this.update_level(ref this.next_block, this.current_block, passage_time);
	
		}

		this.block_count++;
	}

	// プレイヤーのスピードをゲットする.
	public float	getPlayerSpeed()
	{
		return(this.level_datas[this.current_level].player_speed);
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

					case 0:		level_data.end_time           = float.Parse(word);	break;
					case 1:		level_data.player_speed       = float.Parse(word);	break;
					case 2:		level_data.floor_count.min    = int.Parse(word);	break;
					case 3:		level_data.floor_count.max    = int.Parse(word);	break;
					case 4:		level_data.hole_count.min     = int.Parse(word);	break;
					case 5:		level_data.hole_count.max     = int.Parse(word);	break;
					case 6:		level_data.height_diff.min    = int.Parse(word);	break;
					case 7:		level_data.height_diff.max    = int.Parse(word);	break;
					case 8:		level_data.coin_interval.min  = int.Parse(word);	break;
					case 9:		level_data.coin_interval.max  = int.Parse(word);	break;
					case 10:	level_data.enemy_interval.min = int.Parse(word);	break;
					case 11:	level_data.enemy_interval.max = int.Parse(word);	break;
				}

				n++;
			}

			if(n >= 10) {

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

	// 今のレベルの、レベルデーターを取得する.
	public LevelData	getCurrentLevelData()
	{
		return(this.level_datas[this.current_level]);
	}

	// -------------------------------------------------------------------- //

	private void	update_level(ref CreationInfo current, CreationInfo previous, float passage_time)
	{
		// ---------------------------------------------------------------- //
		// 現在の時間が含まれるところまでレベルを進める.
		
		// 『最後のデーターの終了時間』で繰り返すように.
		float	local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);

		int		i;

		for(i = 0;i < this.level_datas.Count - 1;i++) {

			if(local_time <= this.level_datas[i].end_time) {

				break;
			}
		}

		this.current_level = i;

		// ---------------------------------------------------------------- //

		current.block_type = Block.TYPE.FLOOR;
		current.max_count  = 1;

		if(this.block_count >= 10) {

			LevelData	level_data;

			level_data = this.level_datas[this.current_level];

			switch(previous.block_type) {
	
				case Block.TYPE.FLOOR:
				{
					// ブロック hole_size 個分のはばの穴を作る.
	
					current.block_type = Block.TYPE.HOLE;
					current.max_count  = Random.Range(level_data.hole_count.min, level_data.hole_count.max + 1);
					current.height     = previous.height;
				}
				break;
	
				case Block.TYPE.HOLE:
				{
					// ブロック１０個幅分の床を作る.
	
					current.block_type = Block.TYPE.FLOOR;
					current.max_count  = Random.Range(level_data.floor_count.min, level_data.floor_count.max);

					int		height_min = previous.height + level_data.height_diff.min;
					int		height_max = previous.height + level_data.height_diff.max;

					height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX);
					height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);

					current.height = Random.Range(height_min, height_max + 1);

					//Debug.Log(height_min.ToString() + " " + height_max + " " + current.height.ToString());
				}
				break;
			}
		}

	}

	// ブロックの生成情報をクリアーする.
	private void	clear_next_block(ref CreationInfo block)
	{
		block.block_type         = Block.TYPE.FLOOR;
		block.max_count          = 15;
		block.height             = 0;

		block.current_count = 0;
	}


}

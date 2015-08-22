using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData {
	public struct Range {	// 範囲を表す構造体
		public int min;
		public int max;
	};

	public float end_time;
	public float player_speed;

	public Range floor_count;
	public Range hole_count;
	public Range height_diff;	// 足場の高さの範囲

	public LevelData(){
		// レベルデータの初期化
		this.end_time = 15.0f;
		this.player_speed = 6.0f;
		this.floor_count.min = 10;
		this.floor_count.max = 10;
		this.hole_count.min = 2;
		this.hole_count.max = 6;
		this.height_diff.min = 0;
		this.height_diff.max = 0;
	}
}

public class LevelControl : MonoBehaviour {
	private List<LevelData> level_datas = new List<LevelData>();

	public int HEIGHT_MAX = 20;
	public int HEIGHT_MIN = -4;

	// ブロックに関する構造体
	public struct CreationInfo {
		public Block.TYPE block_type;	// ブロックの種類
		public int max_count;			// ブロックの最大個数
		public int height;				// ブロックを配置する高さ
		public int current_count;		// 作成したブロックの個数
	};

	public CreationInfo previous_block;	// 前回どういうブロックを作ったか
	public CreationInfo current_block;	// 今回どういうブロックを作るべきか
	public CreationInfo next_block;		// 次回どういうブロックを作るべきか

	public int block_count = 0;			// 作成したブロックの総数
	public int level = 0;				// 難易度

	private void clear_next_block(ref CreationInfo block) {
		// 受け取ったブロック(block)の中身を初期化
		block.block_type = Block.TYPE.FLOOR;
		block.max_count = 15;
		block.height = 0;
		block.current_count = 0;
	}

	public void initialize() {
		this.block_count = 0;	// ブロックの総数をゼロに

		// 前回、今回、次回のブロックのそれぞれを
		// clear_next_block()に渡して初期化する
		this.clear_next_block(ref this.previous_block);
		this.clear_next_block(ref this.current_block);
		this.clear_next_block(ref this.next_block);
	}

	private void update_level(ref CreationInfo current, CreationInfo previous, 
		float passage_time) {	// passage_timeでプレイ時間を受け取る
		// レベル1から5の繰り返しに
		float local_time = Mathf.Repeat(passage_time,
			this.level_datas[this.level_datas.Count - 1].end_time);

		// 現在のレベルを求める
		int i;
		for(i=0; i<this.level_datas.Count-1; i++) {
			if(local_time <= this.level_datas[i].end_time) {
				break;
			}
		}
		this.level = i;
	}

	public void update() {
		// 今回作ったブロックの個数をインクリメント
		this.current_block.current_count++;

		// 今回作ったブロックの個数が予定数(max_count)以上なら
		if(this.current_block.current_count >= this.current_block.max_count) {
			this.previous_block = this.current_block;
			this.current_block = this.next_block;

			// 次に作るべきブロックの内容を初期化
			this.clear_next_block(ref this.next_block);
			// 次に作るべきブロックを設定
			this.update_level(ref this.next_block, this.current_block);
		}
		this.block_count++;	// ブロックの総数をインクリメント
	}

	public void loadLevelData(TextAsset level_data_text) {
		// テキストデータを文字列として取り込む
		string level_texts = level_data_text.text;

		// 改行コード'\'ごとに分解し、文字列の配列に格納
		string[] lines = level_texts.Split('\n');

		// lines内の各行に対して、順に処理していくループ
		foreach(var line in lines) {
			if(line == "") {	// 行が空の場合
				continue;	// 以下の処理はせずにループの先頭にジャンプ
			};
			Debug.Log(line);
			string[] words = line.Split();	// 行内のワードを配列に格納
			int n = 0;

			// LevelData型の変数を作成
			// ここに現在処理している行のデータを格納していく
			LevelData level_data = new LevelData();

			// words内の各ワードに対して、順に処理していく
			foreach(var word in words) {
				if(word.StartsWith("#")) {
					break;
				}

				if(word == "") {
					continue;
				}

				//	nの値を0, 1, 2 …7と変化させていき、8項目を処理
				switch (n) {
					case 0:	level_data.end_time = float.Parse(word);
							break;
					case 1:	level_data.player_speed = float.Parse(word);
							break;
					case 2:	level_data.floor_count.min = int.Parse(word);
							break;
					case 3:	level_data.floor_count.max = int.Parse(word);
							break;
					case 4:	level_data.hole_count.min = int.Parse(word);
							break;
					case 5:	level_data.hole_count.max = int.Parse(word);
							break;
					case 6:	level_data.height_diff.min = int.Parse(word);
							break;
					case 7:	level_data.height_diff.max = int.Parse(word);
							break;
				}
				n++;
			}

			if(n >= 8) {
				// List構造のlevel_datasにlevel_dataを追加
				this.level_datas.Add(level_data);
			} else {
				if(n == 0) {
					// 何もしない
				} else {
					// データの個数が合っていないことを示すエラーメッセージを表示
					Debug.LogError("[LevelData] Out of parameter.\n");
				}
			}
		}

		// level_datasにデータがひとつもない場合
		if(this.level_datas.Count == 0) {
			// エラーメッセージを表示
			Debug.LogError("[LevelData] Has no data.\n");
			// level_datasにデフォルトのLevelDataを1つ追加
			this.level_datas.Add(new LevelData());
		}
	}

}

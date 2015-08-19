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

	private List<LevelData> level_datas = new List<LevelData>();

	public int HEIGHT_MAX = 20;
	public int HEIGHT_MIN = -4;

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

	private void update_level(ref CreationInfo current, CreationInfo previous) {
		switch(previous.block_type) {
			case Block.TYPE.FLOOR:						// 今回のブロックが床の場合
				current.block_type = Block.TYPE.HOLE;	// 次回は穴を作成
				current.max_count = 5;					// 穴を5個作る
				current.height = previous.height;		// 高さを前回と同じにする
				break;
			case Block.TYPE.HOLE:						// 今回のブロックが穴の場合
				current.block_type = Block.TYPE.FLOOR;	// 次回は床を作成
				current.max_count = 10;					// 床を10個作る
				break;
		}
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
}

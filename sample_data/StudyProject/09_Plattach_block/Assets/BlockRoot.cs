﻿using UnityEngine;
using System.Collections;

public class BlockRoot : MonoBehaviour {

	public GameObject BlockPrefab = null; // 作り出すべきブロックのPrefab.
	public BlockControl[,] blocks; // マス目（グリッド）.

	private GameObject main_camera = null; // メインカメラ.
	private BlockControl grabbed_block = null; // つかんだブロック.


	void Start() {
		this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
	}


	void Update() {
		Vector3 mouse_position; // マウスの位置.
		this.unprojectMousePosition( // マウスの位置を取得.
		                            out mouse_position, Input.mousePosition);
		// 取得したマウス位置を1つのVector2にまとめる.
		Vector2 mouse_position_xy =
			new Vector2(mouse_position.x, mouse_position.y);
		if(this.grabbed_block == null) { // つかんだブロックが空っぽなら.
			// if(!this.is_has_falling_block()) {
			if(Input.GetMouseButtonDown(0)) { // マウスボタンが押されたら.
				// blocks配列のすべての要素を順に処理する.
				foreach(BlockControl block in this.blocks) {
					if(! block.isGrabbable()) { // ブロックがつかめないなら.
						continue; // ループの先頭にジャンプ.
					}
					// マウス位置がブロックの領域内にないなら.
					if(!block.isContainedPosition(mouse_position_xy)) {
						continue; // ループの先頭にジャンプ.
					}
					// 処理中のブロックをgrabbed_blockに登録.
					this.grabbed_block = block;
					// つかんだときの処理を実行.
					this.grabbed_block.beginGrab();
					break;
				}
			}
			// }
		} else { // つかんだブロックが空っぽでないなら.


			do {
				// スライドさせる先のブロックを取得.
				BlockControl swap_target =
					this.getNextBlock(grabbed_block, grabbed_block.slide_dir);
				// スライド先ブロックが空っぽなら.
				if(swap_target == null) {
					break; // ループを脱出.
				}
				// スライド先ブロックがつかめる状態にないなら.
				if(! swap_target.isGrabbable()) {
					break; // ループを脱出.
				}
				// 現在位置からスライド先までの距離を取得.
				float offset = this.grabbed_block.calcDirOffset(
					mouse_position_xy, this.grabbed_block.slide_dir);
				// 移動距離がブロックサイズの半分より小さいなら.
				if(offset < Block.COLLISION_SIZE / 2.0f) {
					break; // ループを脱出.
				}
				// ブロックを入れ替える.
				this.swapBlock(
					grabbed_block, grabbed_block.slide_dir, swap_target);
				this.grabbed_block = null; // いまや、ブロックをつかんでいない.
			} while(false);



			if(! Input.GetMouseButton(0)) { // マウスボタンが押されていないなら.
				this.grabbed_block.endGrab(); // ブロックを離したときの処理を実行.
				this.grabbed_block = null; // grabbed_blockを空っぽに設定.
			}
		}
	}







	// ブロックを作り出して、横9マス、縦9マスに配置.
	public void initialSetUp()
	{
		// マス目のサイズを9×9にする.
		this.blocks =
			new BlockControl [Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
		// ブロックの色番号.
		int color_index = 0;
		for(int y = 0; y < Block.BLOCK_NUM_Y; y++) { // 先頭行から最終行まで.
			for(int x = 0; x < Block.BLOCK_NUM_X; x++) {// 左端から右端まで.
				// BlockPrefabのインスタンスをシーン上に作る.
				GameObject game_object =
					Instantiate(this.BlockPrefab) as GameObject;
				// 上で作ったブロックのBlockControlクラスを取得.
				BlockControl block = game_object.GetComponent<BlockControl>();
				// ブロックをマス目に格納.
				this.blocks[x, y] = block;
				// ブロックの位置情報（グリッド座標）を設定.
				block.i_pos.x = x;
				block.i_pos.y = y;
				// 各BlockControlが連携するGameRootは自分だと設定.
				block.block_root = this;
				// グリッド座標を実際の位置（シーン上の座標）に変換.
				Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
				// シーン上のブロックの位置を移動.
				block.transform.position = position;
				// ブロックの色を変更.
				block.setColor((Block.COLOR)color_index);
				// ブロックの名前を設定（後述）.
				block.name = "block(" + block.i_pos.x.ToString() +
					"," + block.i_pos.y.ToString() + ")";
				// 全種類の色の中から、ランダムに1色を選択.
				color_index =
					Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
			}
		}
	}


	// 指定されたグリッド座標から、シーン上の座標を求める.
	public static Vector3 calcBlockPosition(Block.iPosition i_pos) {
		// 配置する左上隅の位置を初期値として設定.
		Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f),
		                               -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
		// 初期値＋グリッド座標×ブロックサイズ.
		position.x += (float)i_pos.x * Block.COLLISION_SIZE;
		position.y += (float)i_pos.y * Block.COLLISION_SIZE;
		return(position); // シーン上の座標を返す.
	}


	public bool unprojectMousePosition(	out Vector3 world_position, Vector3 mouse_position)
	{
		bool ret;
		// 板を作成。この板はカメラに対して後ろ向き(Vector3.back)で.
		// ブロックの半分のサイズ分、手前に置かれる.
		Plane plane = new Plane(Vector3.back, new Vector3(
			0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
		// カメラとマウスを通る光線を作成.
		Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(
			mouse_position);
		float depth;
		// 光線（ray）が板（plane）に当たっているなら.
		if(plane.Raycast(ray, out depth)) {
			// 引数world_positionを、マウスの位置で上書き.
			world_position = ray.origin + ray.direction * depth;
			ret = true;
			// 当たっていないなら.
		} else {
			// 引数world_positionをゼロのベクターで上書き.
			world_position = Vector3.zero;
			ret = false;
		}
		return(ret);
	}




	public BlockControl getNextBlock(
		BlockControl block, Block.DIR4 dir)
	{
		// スライド先のブロックをここに格納.
		BlockControl next_block = null;
		switch(dir) {
		case Block.DIR4.RIGHT:
			if(block.i_pos.x < Block.BLOCK_NUM_X - 1) {
			// グリッド内なら.
			next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
			}
			break;

		case Block.DIR4.LEFT:
			if(block.i_pos.x > 0) { // グリッド内なら.
				next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
			}
			break;
		case Block.DIR4.UP:
			if(block.i_pos.y < Block.BLOCK_NUM_Y - 1) { // グリッド内なら.
				next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
			}
			break;
		case Block.DIR4.DOWN:
			if(block.i_pos.y > 0) { // グリッド内なら.
				next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
			}
			break;
		}
		return(next_block);
	}

	public static Vector3 getDirVector(Block.DIR4 dir)
	{
		Vector3 v = Vector3.zero;
		switch(dir) {
		case Block.DIR4.RIGHT: v = Vector3.right; break; // 右に1単位ずらす.
		case Block.DIR4.LEFT: v = Vector3.left; break; // 左に1単位ずらす.
		case Block.DIR4.UP: v = Vector3.up; break; // 上に1単位ずらす.
		case Block.DIR4.DOWN: v = Vector3.down; break; // 下に1単位ずらす.
		}
		v *= Block.COLLISION_SIZE; // ブロックのサイズを掛ける.
		return(v);
	}

	public static Block.DIR4 getOppositDir(Block.DIR4 dir)
	{
		Block.DIR4 opposit = dir;
		switch(dir) {
		case Block.DIR4.RIGHT: opposit = Block.DIR4.LEFT; break;
		case Block.DIR4.LEFT: opposit = Block.DIR4.RIGHT; break;
		case Block.DIR4.UP: opposit = Block.DIR4.DOWN; break;
		case Block.DIR4.DOWN: opposit = Block.DIR4.UP; break;
		}
		return(opposit);
	}



	public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		// それぞれのブロックの色を覚えておく.
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;
		// それぞれのブロックの.
		// 拡大率を覚えておく.
		Vector3 scale0 =
			block0.transform.localScale;
		Vector3 scale1 =
			block1.transform.localScale;
		// それぞれのブロックの「消える時間」を覚えておく.
		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;
		// それぞれのブロックの移動先を求める.
		Vector3 offset0 = BlockRoot.getDirVector(dir);
		Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
		block0.setColor(color1); // 色を入れ替える.
		block1.setColor(color0);
		block0.transform.localScale = scale1; // 拡大率を入れ替える.
		block1.transform.localScale = scale0;
		block0.vanish_timer = vanish_timer1; // 「消える時間」を入れ替える.
		block1.vanish_timer = vanish_timer0;
		block0.beginSlide(offset0); // 元のブロックの移動を開始.
		block1.beginSlide(offset1); // 移動先ブロックの移動を開始.
	}


}

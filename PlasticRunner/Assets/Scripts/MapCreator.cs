using UnityEngine;
using System.Collections;

public class MapCreator : MonoBehaviour {
	public static float BLOCK_WIDTH = 1.0f;	// ブロックの幅
	public static float BLOCK_HEIGHT = 0.2f;	// ブロックの高さ
	public static int	BLOCK_NUM_IN_SCREEN = 24;	// 画面内に収まるブロックの個数
	
	// ブロックに関する情報をまとめて管理するための構造体
	private struct FloorBlock {
		public bool is_created;	// ブロックが作成済みか否か
		public Vector3 position;	// ブロックの位置
	};
	
	private FloorBlock last_block;	// 最後に作成したブロック
	private PlayerControl player = null;	// シーン上のPlayerを保管
	private BlockCreator block_creator;	// BlockCreatorを保管

	// Use this for initialization
	void Start () {
		this.player = GameObject.FindGameObjectWithTag
			("Player").GetComponent<PlayerControl>();
		this.last_block.is_created = false;
		this.block_creator = this.gameObject.GetComponent<BlockCreator>();
	}
	
	// Update is called once per frame
	void Update () {
		// プレイヤーのX位置を取得
		float block_generate_x = this.player.transform.position.x;
		
		// 約半画面分、右へ移動
		// ブロックを生み出すしきい値
		block_generate_x += BLOCK_WIDTH * ( (float)BLOCK_NUM_IN_SCREEN + 1 ) / 2.0f;
		
		// 最後に作成したブロックの位置がしきい値より小さい間
		while(this.last_block.position.x < block_generate_x) {
			// ブロックを生成
			this.create_floor_block();
		}
	}
	
	private void create_floor_block() {
		Vector3 block_position;	// これから作るブロックの位置
		
		if(! this.last_block.is_created) {	// last_blockが未作成の場合
			// ブロックの位置をPlayerと同じにする
			block_position = this.player.transform.position;
			// ブロックのX位置を半画面分、左に移動
			block_position.x -= BLOCK_WIDTH * ( (float)BLOCK_NUM_IN_SCREEN / 2.0f );
			// ブロックのY位置をゼロに
			block_position.y = 0.0f;
		} else {	// last_blockが作成済みの場合
			// 今回作るブロックの位置を前回作ったブロックと同じに
			block_position = this.last_block.position;
		}
		
		// ブロックを1ブロック分、右に移動
		block_position.x += BLOCK_WIDTH;
		
		// BlockCreatorスクリプトのcreateBlock()メソッドに作成指示
		// これまでのコードで設定したblock_positionを渡す
		this.block_creator.createBlock(block_position);
		
		// last_blockの位置を今回の位置に更新
		this.last_block.position = block_position;
		// ブロック作成済みのためlast_blockのis_createdをtrueに
		this.last_block.is_created = true;
	}
	
		public bool isDelete(GameObject block_object) {
		bool ret = false;	// 戻り値

		// Playerから半画面分左の位置
		// これが消えるべきか否かを決めるしきい値
		float left_limit = this.player.transform.position.x
			- BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

		// ブロックの位置がしきい値より小さい（左）なら
		if (block_object.transform.position.x < left_limit) {
			ret = true;	// 戻り値をtrue(消す)に
		}

		return(ret);
	}
	
}

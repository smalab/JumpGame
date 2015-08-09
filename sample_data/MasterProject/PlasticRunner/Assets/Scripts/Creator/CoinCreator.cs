using UnityEngine;
using System.Collections;

//! コインをつくる.
public class CoinCreator : MonoBehaviour {

	public GameObject	coin_prefab = null;				// コインのプレハブ。Inspector でセットする.
	public GameObject	dropped_coin_prefab = null;		// ジャンプ中に落とすコインのプレハブ。Inspector でセットする.

	private int			next_block = 10;				// 次にコインをつくるブロック.

	// ---------------------------------------------------------------- //

	private ScoreControl	score_control  = null;		// スコアー管理.
	public	MapCreator		map_creator = null;			// マップツクラー.

	// ================================================================ //

	public static CoinCreator	getInstance()
	{
		CoinCreator	coin_creator = GameObject.FindGameObjectWithTag("Game Root").GetComponent<CoinCreator>();

		return(coin_creator);
	}

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.score_control  = this.gameObject.GetComponent<ScoreControl>();
	}
	
	void	Update()
	{
	
	}

	// ================================================================ //

	// コインを作る.
	public void		createCoin(LevelData level_data, int block_count, Vector3 block_position)
	{
		// ブロックを一定個数（ランダム）作るたびに、コインを作る.
		if(block_count >= this.next_block) {

			Vector3		p0 = block_position;
	
			// ブロックの上にぴったり乗っかっているときの高さ.
			p0.y += MapCreator.BLOCK_HEIGHT/2.0f + CoinControl.COLLISION_SIZE/2.0f;
	
			p0.y += PlayerControl.COLLISION_SIZE;
	
			this.create_coin_object(p0);

			// 次にコインを作るブロックを更新しておく.
			this.next_block += Random.Range(level_data.coin_interval.min, level_data.coin_interval.max + 1);
		}
	}

	// 『ジャンプ中に落としちゃったコイン』を作る.
	public void		createDroppedCoin(Vector3 position)
	{
		DroppedCoinControl	coin = this.create_dropped_coin_object(position);

		// 左方向に.
		coin.rigidbody.velocity = new Vector3(-1.0f, 1.0f, 0.0f);
		// くるくる回りながら.
		coin.rigidbody.angularVelocity = new Vector3(0.0f, 10.0f, 0.0f);
	}


	// ================================================================ //

	// コインのゲームオブジェクトを作る.
	private CoinControl	create_coin_object(Vector3 position)
	{
		GameObject	go = GameObject.Instantiate(this.coin_prefab) as GameObject;

		CoinControl	coin = go.GetComponent<CoinControl>();

		coin.score_control = this.score_control;
		coin.map_creator   = this.map_creator;

		coin.transform.position = position;

		return(coin);
	}

	// 『落としたコイン』のゲームオブジェクトを作る.
	private DroppedCoinControl	create_dropped_coin_object(Vector3 position)
	{
		GameObject	go = GameObject.Instantiate(this.dropped_coin_prefab) as GameObject;

		DroppedCoinControl	coin = go.GetComponent<DroppedCoinControl>();

		coin.map_creator = this.map_creator;

		coin.transform.position = position;

		return(coin);
	}

}

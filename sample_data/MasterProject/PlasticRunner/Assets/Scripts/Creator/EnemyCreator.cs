using UnityEngine;
using System.Collections;

// おじゃまキャラクターをつくる.
public class EnemyCreator : MonoBehaviour {

	public GameObject	enemy_prefab = null;			// コインのプレハブ。Inspector でセットする.

	private int			next_block = 10;				// 次におじゃまキャラをつくるブロック.

	// ---------------------------------------------------------------- //

	public	MapCreator		map_creator = null;			// マップツクラー.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
	
	}


	// ================================================================ //
	// おじゃまキャラクターを作る.
	public void		createEnemy(LevelData level_data, int block_count, Vector3 block_position)
	{
		// ブロックを一定個数（ランダム）作るたびに、おじゃまキャラを作る.
		if(block_count >= this.next_block) {

			Vector3		p0 = block_position;
	
			// ブロックの上にぴったり乗っかっているときの高さ.
			p0.y += MapCreator.BLOCK_HEIGHT/2.0f + CoinControl.COLLISION_SIZE/2.0f;
			p0.y += PlayerControl.COLLISION_SIZE;
	
			this.create_enemy_object(p0);

			// 次におじゃまキャラを作るブロックを更新しておく.
			this.next_block += Random.Range(level_data.enemy_interval.min, level_data.enemy_interval.max + 1);
		}
	}

	// ================================================================ //

	// おじゃまキャラのゲームオブジェクトを作る.
	private EnemyControl	create_enemy_object(Vector3 position)
	{
		GameObject	go = GameObject.Instantiate(this.enemy_prefab) as GameObject;

		EnemyControl	enemy = go.GetComponent<EnemyControl>();

		enemy.map_creator = this.map_creator;

		enemy.transform.position = position;

		return(enemy);
	}
}

using UnityEngine;
using System.Collections;

// ブロックツクラー.
public class BlockCreator : MonoBehaviour {

	public	MapCreator		map_creator = null;			// マップツクラー.

	public GameObject[]	blockPrefabs;					// 床ブロックのプレハブ。Inspector でセットする.

	private int			block_count = 0;				// 作ったブロックの個数.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{	
	}
	
	void	Update()
	{
	}

	// ================================================================ //

	// ブロックを作る.
	public void		createBlock(LevelControl.CreationInfo current_block, Vector3 block_position)
	{
		if(current_block.block_type == Block.TYPE.FLOOR) {

			// 次につくるブロックの種類を決める.
			// blockPrefabs にセットされたブロックがじゅんばんに出てくる.
			//
			int		next_block_type = this.block_count%this.blockPrefabs.Length;

			GameObject		go        = GameObject.Instantiate(this.blockPrefabs[next_block_type]) as GameObject;
			BlockControl	new_block = go.GetComponent<BlockControl>();

			new_block.transform.position = block_position;

			// BlockControl クラスに MapCreator を記録しておく.
			// （BlockControl クラスから MapCreator クラスのメソッドを呼び出すため）.
			//
			new_block.map_creator = this.map_creator;

			this.block_count++;
		}
	}
}

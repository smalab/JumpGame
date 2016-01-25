using UnityEngine;
using System.Collections;

public class BlockCreator : MonoBehaviour {
    public MapCreator map_creator = null;
	public GameObject[] blockPrefabs;	// ブロックを格納する配列
	private int block_count = 0;	// 作成したブロックの個数

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
	
	public void createBlock(LevelControl.CreationInfo current_block, Vector3 block_position)
    {
        if (current_block.block_type == Block.TYPE.FLOOR)
        {

            // 作成すべきブロックの種類を求める
            int next_block_type = this.block_count % this.blockPrefabs.Length;

            // ブロックを生成し、goに保管
            GameObject go = Instantiate(blockPrefabs[next_block_type]) as GameObject;
            BlockControl new_block = go.GetComponent<BlockControl>();

            new_block.transform.position = block_position; // ブロックの位置を移動

            // BlockControl クラスに MapCreator を記録しておく.
            // （BlockControl クラスから MapCreator クラスのメソッドを呼び出すため）.
            //
            new_block.map_creator = this.map_creator;

            this.block_count++; // ブロックの個数をインクリメント
        }
	}
	
}

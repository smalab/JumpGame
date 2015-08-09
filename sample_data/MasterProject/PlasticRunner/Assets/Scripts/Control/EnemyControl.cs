using UnityEngine;
using System.Collections;

// おじゃまキャラクター.
public class EnemyControl : MonoBehaviour {

	public	MapCreator		map_creator = null;			// マップツクラー.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
	
		// 画面左端からはみ出たら、消す.
		if(this.map_creator.isDelete(this.gameObject)) {

			GameObject.Destroy(this.gameObject);
		}
	}

	// ---------------------------------------------------------------- //

	void 	OnCollisionEnter(Collision other)
	{
		if(other.collider.tag == "Player") {
			// プレイヤーとぶつかったら、プレイヤーにお知らせする.
			PlayerControl	player = other.collider.gameObject.GetComponent<PlayerControl>();
			player.onTouchEnemy(this);
		}
	}
}

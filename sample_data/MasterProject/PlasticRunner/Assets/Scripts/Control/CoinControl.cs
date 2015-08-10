using UnityEngine;
using System.Collections;

//! コイン.
public class CoinControl : MonoBehaviour {

	public static float	COLLISION_SIZE = 1.0f;			// コリジョン球の大きさ（直径）.


	public	ScoreControl	score_control = null;		// スコアー管理.
	public	MapCreator		map_creator = null;			// マップツクラー.
	public	Vector3			goal_position;
	public	float			height_offset = 0.0f;





	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.goal_position = this.transform.position;

	}
	
	void	Update()
	{
		// ぴょん.
		this.height_offset *= 0.90f*(Time.deltaTime/(1.0f/60.0f));
		this.transform.position = this.goal_position + this.height_offset*Vector3.up;

		// くるくる.
		float	spin_speed = (360.0f/2.0f);
		this.transform.rotation *= Quaternion.AngleAxis(spin_speed*Time.deltaTime, Vector3.up);

		// 画面左端からはみ出たら、消す.
		if(this.map_creator.isDelete(this.gameObject)) {
			GameObject.Destroy(this.gameObject);
		}
	}

	// ---------------------------------------------------------------- //

	void 	OnTriggerEnter(Collider other)
	{
		// プレイヤーに拾われたら、スコアーを足す.
		if(other.tag == "Player") {
			this.score_control.addCoinScore();
		}

		GameObject.Destroy(this.gameObject);
	}

	// ================================================================ //
}

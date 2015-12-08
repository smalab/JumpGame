using UnityEngine;
using System.Collections;

// 床ブロック.
public class BlockControl : MonoBehaviour {

	public MapCreator		map_creator = null;			// マップツクラー.

	private	GameObject		model = null;				// 表示用モデルたち.
	private bool			trigger_stepped = false;	// 踏まれた瞬間？.

	// 踏んづけられたときのエフェクト.
	private struct Spring {

		public	float	velocity;
		public	float	position;
	};
	private	Spring	spring;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.model = this.transform.FindChild("model").gameObject;

		this.spring.velocity = 0.0f;
		this.spring.position = 0.0f;
	}
	
	void	Update()
	{
		// ---------------------------------------------------------------- //
		// 踏んづけられたときのエフェクト.

		// 踏まれた瞬間に下がる（速度を下向きに）.
		if(this.trigger_stepped) {

			this.spring.velocity -= 2.0f;

			this.trigger_stepped = false;
		}

		// 速度を JoJo に上向きに.
		if(this.spring.velocity < 1.0f) {

			this.spring.velocity += 6.0f*Time.deltaTime;
		}

		this.spring.position += this.spring.velocity*Time.deltaTime;

		// 最初の位置まで戻った.
		if(this.spring.position >= 0.0f) {

			this.spring.position = 0.0f;
			this.spring.velocity = 0.0f;
		}

		// ---------------------------------------------------------------- //
		// 画面左端からはみ出たら、消す.

		if(this.map_creator.isDelete(this.gameObject)) {

			GameObject.Destroy(this.gameObject);
		}
	}

	void	LateUpdate()
	{
		// アニメーションに上書きされないよう、Update() ではなく
		// LateUpdate() でセットする.
		//
		this.model.transform.localPosition += Vector3.up*this.spring.position;
	}

	// ================================================================ //

	// 踏んづけられたときに呼ばれるメソッド.
	public void		onStepped()
	{
		this.trigger_stepped = true;
	}
}

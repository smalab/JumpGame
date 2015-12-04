using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private PlayerControl	player = null;

	private	Vector3			position_offset = Vector3.zero;
	private Vector3			move_vector = Vector3.zero;					// 前のフレームからの移動量.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

		// プレイヤーとカメラのポジションのオフセット（位置のずれ）を求めておく.
		// プレイヤーと一緒に移動するようにするため.
		this.position_offset = this.transform.position - this.player.transform.position;
	}
	
	void	Update ()
	{

	}

	void	LateUpdate()
	{
		// プレイヤーと一緒に移動する.

		Vector3		new_position = this.transform.position;

		new_position.x = this.player.transform.position.x + this.position_offset.x;

		this.move_vector = (new_position - this.transform.position)/Time.deltaTime;

		this.transform.position = new_position;
	}

	public Vector3	getMoveVector()
	{
		return(this.move_vector);
	}
}

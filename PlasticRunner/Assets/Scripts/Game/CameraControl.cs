using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	private GameObject player = null;
	private Vector3 position_offset = Vector3.zero;	// Vector3(0, 0, 0);

	// Use this for initialization
	void Start () {
		// メンバー変数playerに、Playerオブジェクトを取得
		this.player = GameObject.FindGameObjectWithTag("Player");
		
		// カメラの位置(this.transform.positon)と
		// プレイヤーの位置(this.player.transform.position)との差分を格納
		this.position_offset = this.transform.position - this.player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// カメラの現在位置をnew_positoinに取得
		Vector3 new_position = this.transform.position;
		
		// プレイヤーのX座標に差分を足して、変数new_positionのXに代入する
		new_position.x = this.player.transform.position.x + this.position_offset.x;
		
		// カメラの位置を、新しい位置(new_position)に更新
		this.transform.position = new_position;
	}
}

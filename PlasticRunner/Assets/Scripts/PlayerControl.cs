using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public static float ACCELERATION = 10.0f;	// 加速度
	public static float SPEED_MIN = 4.0f;	// 速度の最小値
	public static float SPEED_MAX = 8.0f;	// 速度の最大値
	public static float JUMP_HEIGHT_MAX = 3.0f;	// ジャンプの高さ
	public static float JUMP_KEY_RELEASE_REDUCE = 0.5f;	// ジャンプからの減速値

	public enum STEP {	// Playerの各種状態を表すデータ型
		NONE = -1,		// 状態情報なし
		RUN = 0,		// 走る
		JUMP,			// ジャンプ
		MISS,			// ミス
		NUM,			// 状態が何種類があるかを示す（=3）
	};

	public STEP step = STEP.NONE;	// Playerの現在の状態
	public STEP next_step = STEP.NONE;	// Playerの次の状態

	public float step_timer = 0.0f;	// 経過時間
	private bool is_landed = false;	// 着地しているかの判定
	private bool is_collided = false;	// 何かとぶつかっているかの判定
	private bool is_key_released = false;	// ボタンが離されているかの判定

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate( new Vector3(0.0f, 0.0f, 3.0f * Time.deltaTime) );
	}
}

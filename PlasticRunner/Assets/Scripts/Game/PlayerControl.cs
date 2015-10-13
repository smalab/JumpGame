using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public float current_speed = 0.0f;	// 現在のスピード
	public LevelControl level_control = null;	// LevelControlを保持
	public static float ACCELERATION = 10.0f;	// 加速度
	public static float SPEED_MIN = 4.0f;	// 速度の最小値
	public static float SPEED_MAX = 8.0f;	// 速度の最大値
	public static float JUMP_HEIGHT_MAX = 3.0f;	// ジャンプの高さ
	public static float JUMP_KEY_RELEASE_REDUCE = 0.5f;	// ジャンプからの減速値
	public static float NARAKU_HEIGHT = -5.0f;
	public bool isPlaying = true;	// プレイ可能かを判断する

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
	//private bool is_collided = false;	// 何かとぶつかっているかの判定
	private bool is_key_released = false;	// ボタンが離されているかの判定
	private float click_timer = -1.0f;	// ボタンが押されてからの時間
	private float CLICK_CRACE_TIME = 0.5f;	// ジャンプしたい意志を受け取る時間

	// Use this for initialization
	void Start () {
	this.next_step = STEP.RUN;	// ゲーム開始を走る状態に
	}

	// Update is called once per frame
	void Update () {
		Vector3 velocity = this.GetComponent<Rigidbody>().velocity;	// 速度を設定
		this.current_speed = this.level_control.getPlayerSpeed();
		this.check_landed();	// 着地状態かどうかをチェック

		switch (this.step) {
			case STEP.RUN:
			case STEP.JUMP:
				// 現在の位置がしきい値よりも下ならば
				if(this.transform.position.y < NARAKU_HEIGHT) {
					this.next_step = STEP.MISS;	// ミス状態にする
				}
				break;
		}

		this.step_timer += Time.deltaTime;	// 経過時間を進める

		if(Input.GetMouseButtonDown(0)) {	// ボタンが押されたら
			this.click_timer = 0.0f;	// タイマーをリセット
		} else {
			if(this.click_timer >= 0.0f) {	// そうでなければ
				this.click_timer += Time.deltaTime;	// 経過時間を加算
			}
		}

		// 次の状態が決まっていなければ、状態の変化を調べる
		if(this.next_step == STEP.NONE) {
			switch(this.step) {	// Playerの現在の状態で分岐
				case STEP.RUN:	// 走行中の場合
					// click_timerが0以上、CLICK_GRACE_TIME以下の場合
					if(0.0f <= this.click_timer && this.click_timer <= CLICK_CRACE_TIME) {
						if(this.is_landed) {	// 着地しているならば
							this.click_timer = -1.0f;	// ボタンが押されていないことを表す-1.0fに
							this.next_step = STEP.JUMP;	// ジャンプ状態に
							}
						}
					break;
				case STEP.JUMP:	// ジャンプ中の場合
					if(this.is_landed) {
						// ジャンプ中で着地していたら、次の状態を走行中に変更
						this.next_step = STEP.RUN;
					}
					break;
			}
		}

		// 次の状態が状態情報なし以外の間
		while(this.next_step != STEP.NONE) {
			this.step = this.next_step;	// 現在の状態を次の状態に更新
			this.next_step = STEP.NONE;	// 次の情愛を状態なしに変更
			switch(this.step) {	// 更新された現在の状態が
				case STEP.JUMP:	// ジャンプの場合
					// ジャンプの高さからジャンプの初速を計算
					velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
					// ボタンが離されたフラグをクリアする
					this.is_key_released = false;
					break;
			}
			this.step_timer = 0.0f;	// 状態が変化したので、経過時間をリセット
		}

		// 状態ごとの、毎フレームの更新処理
		switch(this.step) {
			case STEP.RUN:	// 走行中の場合
				// 速度を上げる
				velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
				// 計算結果のスピードが設定すべきスピードを超えていたら
				if(Mathf.Abs(velocity.x) > this.current_speed) {
					// 超えないように調整
					velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
				}
				break;
			case STEP.JUMP:	// ジャンプ中の場合
				do {
					// ボタンが離された瞬間でなかった場合
					if(! Input.GetMouseButtonUp(0)) {
						break;
					}
					// 減速済みなら　(複数回減速しないように)
					if(this.is_key_released) {
						break;
					}
					// 上下方向の速度が0以下なら（下降中なら）
					if(velocity.y <= 0.0f) {
						break;
					}
					// ボタンが離されていて、上昇中なら減速開始
					// ジャンプの上昇はここで終了
					velocity.y *= JUMP_KEY_RELEASE_REDUCE;
					this.is_key_released = true;
				} while(false);
				break;
			case STEP.MISS:
				// 加速値(ACCELERATION)を引き算してPlayerの速度を遅くしていく
				velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
				if(velocity.x < 0.0f) {	// Playerの速度が負の場合
					velocity.x = 0.0f;
				}
				break;
		}
		// Rigidbodyの速度を上記で求めた速度で更新
		// この行は状態にかかわらず毎回実行される
		this.GetComponent<Rigidbody>().velocity = velocity;
	}

	private void check_landed() {
		this.is_landed = false;

		do {
			Vector3 s = this.transform.position;	// Playerの現在位置
			Vector3 e = s + Vector3.down * 1.0f;	// sから1.0fに移動した位置

			RaycastHit hit;
			if(! Physics.Linecast(s, e, out hit)) {	// sからeの間に何もない場合
				break;	// 何もせずdo-whileループを抜ける
			}

			// sからeの間に何かがあった場合の処理
			if(this.step == STEP.JUMP) {	// 現在がジャンプ状態ならば
				// 経過時間が3.0f未満ならば
				if(this.step_timer < Time.deltaTime * 3.0f) {
					break;
				}
			}

			// sからeの間に何かがあり、JUMP直後でない場合のみ以下を実行する
			this.is_landed = true;
		} while(false);
	}

}

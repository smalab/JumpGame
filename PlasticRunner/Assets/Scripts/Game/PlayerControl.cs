using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public static float ACCELERATION = 10.0f; // 加速度.
	public static float SPEED_MIN = 4.0f; // 速度の最小値.
	public static float SPEED_MAX = 8.0f; // 速度の最大値.
	public static float JUMP_HEIGHT_MAX = 3.0f; // ジャンプの高さ.
	public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // ジャンプからの減速値.

	public enum STEP { // Playerの各種状態を表すデータ型.
		NONE = -1, // 状態情報なし.
		RUN = 0, // 走る.
		JUMP, // ジャンプ.
		MISS, // ミス.
		NUM, // 状態が何種類あるかを示す（＝3）.
	};

	public static STEP step = STEP.NONE; // Playerの現在の状態.
	public STEP next_step = STEP.NONE; // Playerの次の状態.
	public float step_timer = 0.0f; // 経過時間.
	private bool is_landed = false; // 着地しているかどうか.
	//private bool is_colided = false; // 何かとぶつかっているかどうか.
	private bool is_key_released = false; // ボタンが離されているかどうか.


	public static float NARAKU_HEIGHT = -5.0f;


	public float current_speed = 0.0f; // 現在のスピード.
	public LevelControl level_control = null; // LevelControlを保持.

	private float click_timer = -3.0f; // ボタンが押されてからの時間.
	private float CLICK_GRACE_TIME = 0.5f; // 「ジャンプしたい意志」を受け付ける時間.
	private Rigidbody mRigidbody = null;
	private Animation mPlayerAnimation;

	void Start() {
		next_step = STEP.RUN;
		mRigidbody = GetComponent<Rigidbody>();
		mPlayerAnimation = transform.GetComponentInChildren<Animation>();
	}


	private void check_landed()
	{
		is_landed = false; // とりあえずfalseにしておく.
		do {
			Vector3 s = transform.position; // Playerの現在の位置.
			Vector3 e = s + Vector3.down * 1.0f; // sから下に1.0fに移動した位置.
			RaycastHit hit;
			if(! Physics.Linecast(s, e, out hit)) { // sからeの間に何もない場合.
				break; // 何もせずdo〜whileループを抜ける（脱出口へ）.
			}
			// sからeの間に何かがあった場合、以下の処理が行われる.
			if(step == STEP.JUMP) { // 現在、ジャンプ状態ならば.
				// 経過時間が3.0f未満ならば.
				if(step_timer < Time.deltaTime * 3.0f) {
					break; // 何もせずdo〜whileループを抜ける（脱出口へ）.
				}
			}
			// sからeの間に何かがあり、JUMP直後でない場合のみ、以下が実行される.
			is_landed = true;
		} while(false);
		// ループの脱出口.
	}


	void Update() {
		Vector3 velocity = mRigidbody.velocity; // 速度を設定.
		current_speed = level_control.getPlayerSpeed();
		check_landed(); // 着地状態かどうかをチェック.

		switch(step) {
		case STEP.RUN:
		case STEP.JUMP:
			// 現在の位置がしきい値よりも下ならば.
			if(transform.position.y < NARAKU_HEIGHT) {
				next_step = STEP.MISS; // 「ミス」状態にする.
			}
			break;
		}


		step_timer += Time.deltaTime; // 経過時間を進める.


		if(Input.GetMouseButtonDown(0)) { // ボタンが押されたら.
			click_timer = 0.0f; // タイマーをリセット.
		} else {
			if(click_timer >= 0.0f) { // そうでなければ.
				click_timer += Time.deltaTime; // 経過時間を加算.
			}
		}


		// 「次の状態」が決まっていなければ、状態の変化を調べる.
		if(next_step == STEP.NONE) {
			switch(step) { // Playerの現在の状態で分岐.
			case STEP.RUN: // 走行中の場合.
				/*
				if(! is_landed) {
					// 走行中で、着地していない場合、何もしない.
				} else {
					if(Input.GetMouseButtonDown(0)) {
						// 走行中で、着地していて、左ボタンが押されていたら.
						// 次の状態をジャンプに変更.
						next_step = STEP.JUMP;
					}
				}
				*/
				// click_timerが0以上、CLICK_GRACE_TIME以下ならば.
				if(0.0f <= click_timer && click_timer <= CLICK_GRACE_TIME){
					if(is_landed){ // 着地しているならば.
						click_timer = -1.0f; // 「ボタンが押されていない」ことを表す -1.0f に.
						next_step = STEP.JUMP; // ジャンプ状態にする.
					}
				}


				break;

			case STEP.JUMP: // ジャンプ中の場合.
				if(is_landed) {
					// ジャンプ中で、着地していたら、.
					// 次の状態を走行中に変更.
					next_step = STEP.RUN;
				}
				break;
			}
		}
		//

		// 「次の状態」が「状態情報なし」以外の間.
		while(next_step != STEP.NONE) {
			step = next_step; // 「現在の状態」を「次の状態」に更新.
			next_step = STEP.NONE; // 「次の状態」を「状態なし」に変更.
			switch(step) { // 更新された「現在の状態」が.
			case STEP.JUMP: // 「ジャンプ」の場合.
				// ジャンプの高さからジャンプの初速を計算（オマジナイ）.
				velocity.y = Mathf.Sqrt(
					2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
				// 「ボタンが離されたフラグ」をクリアーする.
				is_key_released = false;
				break;
			}
			// 状態が変化したので、経過時間をゼロにリセット.
			step_timer = 0.0f;
		}


		// 状態ごとの、毎フレームの更新処理.
		switch(step) {
		case STEP.RUN: // 走行中の場合.
			// 速度を上げる.
			velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

			// 速度が最高速度の制限を超えたら.
			// if(Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX) {
				// 最高速度の制限以下に保つ.
			//	velocity.x *= PlayerControl.SPEED_MAX /
			//		Mathf.Abs(this.rigidbody.velocity.x);
			// }

			// 計算で求めたスピードが、設定すべきスピードを超えていたら.
			if(Mathf.Abs(velocity.x) > current_speed) {
				// 超えないように調整する.
				velocity.x *= current_speed / Mathf.Abs(velocity.x);
			}
			break;


			//break;

		case STEP.JUMP: // ジャンプ中の場合.
			do {
				// 「ボタンが離された瞬間」じゃなかったら.
				if(/*! Input.GetMouseButtonUp(0) && */
					GetMicInput.loudness >= 1.0f)
				{
					break; // 何もせずにループを抜ける.
				}
				// 減速済みなら（二回以上減速しないように）.
				if(is_key_released) {
					break; // 何もせずにループを抜ける.
				}

				// 上下方向の速度が0以下なら（下降中なら）.
				if(velocity.y <= 0.0f) {
					break; // 何もせずにループを抜ける.
				}
				// ボタンが離されていて、上昇中なら、減速開始.
				// ジャンプの上昇はここでおしまい.
				velocity.y *= JUMP_KEY_RELEASE_REDUCE;
				is_key_released = true;
			} while(false);
			break;
		
		
		case STEP.MISS:
			// 加速値（ACCELERATION）を引き算して、Playerの速度を遅くしていく.
			velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
			// if(velocity.x < 0.0f) { // Playerの速度が負の数なら.
			// 	velocity.x = 0.0f; // ゼロにする.
			// }
			transform.position = new Vector3(transform.position.x, 4, transform.position.z);
			mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
			next_step = STEP.RUN;
			break;
		
		}

		// Rigidbodyの速度を、上記で求めた速度で更新.
		// （この行は、状態にかかわらず毎回実行される）.
		//this.GetComponent<Rigidbody>().velocity = velocity;
		mRigidbody.velocity = velocity;
	}


	public bool isPlayEnd() // ゲームが終わったかどうかを判定する.
	{
		bool ret = false;
		switch(step) {
		case STEP.MISS: // MISS状態なら.
			ret = true; // 「死んだよー」(true)と返す.
			break;
		}
		return(ret);
	}




}

﻿using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {
	public static float MOVE_AREA_RADIUS = 15.0f; // 島の半径.
	public static float MOVE_SPEED = 5.0f; // 移動速度.
	private struct Key { // キー操作情報の構造体.
		public bool up; // ↑.
		public bool down; // ↓.
		public bool right; // →.
		public bool left; // ←.
		public bool pick; // 拾う／捨てる.
		public bool action; // 食べる・修理する.
	};
	private Key key; // キー操作情報を保持する変数.
	public enum STEP { // プレイヤーの状態を表す列挙体.
		NONE = -1, // 状態情報なし.
		MOVE = 0, // 移動中.
		REPAIRING, // 修理中.
		EATING, // 食事中.
		NUM, // 状態が何種類あるかを示す（＝3）.
	};
	public STEP step = STEP.NONE; // 現在の状態.
	public STEP next_step = STEP.NONE; // 次の状態.
	public float step_timer = 0.0f; // タイマー.


	private GameObject closest_item = null; // プレイヤーの正面にあるGameObject.
	private GameObject carried_item = null; // プレイヤーが持ち上げたGameObject.
	private ItemRoot item_root = null; // ItemRootスクリプトを保持.
	public GUIStyle guistyle; // フォントスタイル.

	private GameObject closest_event = null; // 注目しているイベントを格納.
	private EventRoot event_root = null; // EventRootクラスを使うための変数.
	private GameObject rocket_model = null; // 宇宙船のモデルを使うための変数.


	void Start() {
		this.step = STEP.NONE; // 現ステップの状態を初期化.
		this.next_step = STEP.MOVE; // 次ステップの状態を初期化.

		this.item_root =
			GameObject.Find("GameRoot").GetComponent<ItemRoot>();
		this.guistyle.fontSize = 16;

		this.event_root =
			GameObject.Find("GameRoot").GetComponent<EventRoot>();
		this.rocket_model = GameObject.Find("rocket").transform.FindChild(
			"rocket_model").gameObject;

	}


	void Update() {
		this.get_input(); // 入力情報を取得.

		this.step_timer += Time.deltaTime;
		float eat_time = 2.0f; // リンゴは、2秒かけて食べる.

		float repair_time = 2.0f; // 修理にかかる時間も2秒.


		// 状態を変化させる---------------------.
		if(this.next_step == STEP.NONE) { // 次の予定がないなら.
			switch(this.step) {
			case STEP.MOVE: // 「移動中」状態の処理.
				do {
					if(! this.key.action) { // アクションキーが押されていない.
						break; // ループを脱出.
					}

					// 注目中のイベントがある場合.
					if(this.closest_event != null) {
						if(! this.is_event_ignitable()) { // イベントが開始不可なら.
							break; // 何もしない.
						}
						// イベントの種類を取得.
						Event.TYPE ignitable_event =
							this.event_root.getEventType(this.closest_event);
						switch(ignitable_event) {
						case Event.TYPE.ROCKET:
							// イベントの種類がROCKETなら.
							// REPAIRING（修理）状態に移行.
							this.next_step = STEP.REPAIRING;
							break;
						}
						break;
					}


					if(this.carried_item != null) {
						// 持っているアイテムを判別.
						Item.TYPE carried_item_type =
							this.item_root.getItemType(this.carried_item);
						switch(carried_item_type) {
						case Item.TYPE.APPLE: // リンゴなら.
						case Item.TYPE.PLANT: // 植物なら.
							// 「食事中」状態に移行.
							this.next_step = STEP.EATING;
							break;
						}
					}
				} while(false);
				break;
			case STEP.EATING: // 「食事中」状態の処理.
				if(this.step_timer > eat_time) { // 2秒待つ.
					this.next_step = STEP.MOVE; // 「移動」状態に移行.
				}
				break;

			case STEP.REPAIRING: // 「修理中」状態の処理.
				if(this.step_timer > repair_time) { // 2秒待つ.
					this.next_step = STEP.MOVE; // 「移動」状態に移行.
				}
				break;

			}
		}

		// 状態が変化した場合------------.
		while(this.next_step != STEP.NONE) { // 状態がNONE以外＝状態が変化した.
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch(this.step) {
			case STEP.MOVE:
				break;
			case STEP.EATING: // 「食事中」状態の処理.
				if(this.carried_item != null) {
					// 持っていたアイテムを破棄.
					GameObject.Destroy(this.carried_item);
					this.carried_item = null;
				}
				break;
			case STEP.REPAIRING: // 「修理中」になったら.
				if(this.carried_item != null) {
					// 持っているアイテムを削除.
					GameObject.Destroy(this.carried_item);
					this.carried_item = null;
					this.closest_item = null;
				}
				break;
			}
			this.step_timer = 0.0f;
		}


		// 各状況で繰り返しすること----------.
		switch(this.step) {
		case STEP.MOVE:
			this.move_control();
			this.pick_or_drop_control();
			break;
		case STEP.REPAIRING:
			// 宇宙船を回転させる.
			this.rocket_model.transform.localRotation *=
				Quaternion.AngleAxis(
					360.0f / 10.0f * Time.deltaTime, Vector3.up);
			break;
		}
	}


	private void get_input()
	{
		this.key.up = false;
		this.key.down = false;
		this.key.right = false;
		this.key.left = false;
		// ↑キーが押されていたらtrueを代入.
		this.key.up |= Input.GetKey(KeyCode.UpArrow);
		this.key.up |= Input.GetKey(KeyCode.Keypad8);
		// ↓キーが押されていたらtrueを代入.
		this.key.down |= Input.GetKey(KeyCode.DownArrow);
		this.key.down |= Input.GetKey(KeyCode.Keypad2);
		// →キーが押されていたらtrueを代入.
		this.key.right |= Input.GetKey(KeyCode.RightArrow);
		this.key.right |= Input.GetKey(KeyCode.Keypad6);
		// ←キーが押されていたらtrueを代入.
		this.key.left |= Input.GetKey(KeyCode.LeftArrow);
		this.key.left |= Input.GetKey(KeyCode.Keypad4);
		// Zキーが押されていたらtrueを代入.
		this.key.pick = Input.GetKeyDown(KeyCode.Z);
		// Xキーが押されていたらtrueを代入.
		this.key.action = Input.GetKeyDown(KeyCode.X);
	}

	private void move_control()
	{
		Vector3 move_vector = Vector3.zero; // 移動用ベクトル.
		Vector3 position = this.transform.position; // 現在位置を保管.
		bool is_moved = false;
		if(this.key.right) { // →キーが押されているなら.
			move_vector += Vector3.right; // 移動用ベクトルを右に向ける.
			is_moved = true; // 「移動中」フラグを立てる.
		}
		if(this.key.left) {
			move_vector += Vector3.left;
			is_moved = true;
		}
		if(this.key.up) {
			move_vector += Vector3.forward;
			is_moved = true;
		}
		if(this.key.down) {
			move_vector += Vector3.back;
			is_moved = true;
		}
		move_vector.Normalize(); // 長さを1に.
		move_vector *= MOVE_SPEED * Time.deltaTime; // 速度×時間＝距離.
		position += move_vector; // 位置を移動.
		position.y = 0.0f; // 高さを 0 にする.
		// 世界の中央から、更新した位置までの距離が、島の半径より大きくなった場合.
		if(position.magnitude > MOVE_AREA_RADIUS) {
			position.Normalize();
			position *= MOVE_AREA_RADIUS; // 位置を、島の端にとどめる.
		}
		// 新しく求めている位置（position）の高さを、現在の高さに戻す.
		position.y = this.transform.position.y;
		// 実際の位置を、新しく求めた位置に変更する.
		this.transform.position = position;
		// 移動ベクトルの長さが0.01より大きい場合
		// ＝ある程度以上の移動した場合.
		if(move_vector.magnitude > 0.01f) {
			// キャラクターの向きをじわっと変える.
			Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
			this.transform.rotation =
				Quaternion.Lerp(this.transform.rotation, q, 0.1f);
		}
	}


	void OnTriggerStay(Collider other)
	{
		GameObject other_go = other.gameObject;


		// トリガーのGameObjectのレイヤー設定がItemなら.
		if(other_go.layer == LayerMask.NameToLayer("Item")) {
			// 何にも注目していないなら.
			if(this.closest_item == null) {
				if(this.is_other_in_view(other_go)) { // 正面にあるなら.
					this.closest_item = other_go; // 注目する.
				}
				// 何かに注目しているなら.
			} else if(this.closest_item == other_go) {
				if(! this.is_other_in_view(other_go)) { // 正面にないなら.
					this.closest_item = null; // 注目をやめる.
				}
			}

		// トリガーのGameObjectのレイヤー設定がEventなら.
		} else if(other_go.layer == LayerMask.NameToLayer("Event")) {

			// 何にも注目していないなら.
			if(this.closest_event == null) {
				if(this.is_other_in_view(other_go)) { // 正面にあるなら.
					this.closest_event = other_go; // 注目する.
				}
				// 何かに注目しているなら.
			} else if(this.closest_event == other_go) {
				if(!this.is_other_in_view(other_go)) { // 正面にないなら.
					this.closest_event = null; // 注目をやめる.
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(this.closest_item == other.gameObject) {
			this.closest_item = null; // 注目をやめる.
		}
	}

	/*
	void OnGUI()
	{
		float x = 20.0f;
		float y = Screen.height - 40.0f;
		// 持ち上げているアイテムがあるなら.
		if(this.carried_item != null) {
			GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:すてる", guistyle);
			GUI.Label(new Rect(x+100.0f, y, 200.0f, 20.0f),
			          "X:たべる", guistyle);
		} else {
			// 注目しているアイテムがあるなら.
			if(this.closest_item != null) {
				GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:拾う", guistyle);
			}
		}

		switch(this.step) {
		case STEP.EATING:
			GUI.Label(new Rect(x, y, 200.0f, 20.0f),
			          "むしゃむしゃもぐもぐ……", guistyle);
			break;
		}
	}
	*/

	void OnGUI() {
		float x = 20.0f;
		float y = Screen.height - 40.0f;
		if(this.carried_item != null) {
			GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:すてる", guistyle);
			do {
				if(this.is_event_ignitable()) {
					break;
				}
				if(item_root.getItemType(this.carried_item) == Item.TYPE.IRON) {
					break;
				}
				GUI.Label(new Rect(x+100.0f, y, 200.0f, 20.0f),
				          "x:たべる", guistyle);
			}while(false);
		} else {
			if(this.closest_item != null) {
				GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:拾う", guistyle);
			}
		}
		switch(this.step) {
		case STEP.EATING:
			GUI.Label(new Rect(x, y, 200.0f, 20.0f),
			          "むしゃむしゃもぐもぐ……", guistyle);
			break;
		case STEP.REPAIRING:
			GUI.Label(new Rect(x+200.0f, y, 200.0f, 20.0f),
			          "修理中", guistyle);
			break;
		}
		if(this.is_event_ignitable()) { // イベントが開始可能な場合.
			// イベント用メッセージを取得.
			string message =
				this.event_root.getIgnitableMessage(this.closest_event);
			GUI.Label(new Rect(x+200.0f, y, 200.0f, 20.0f),
			          "X:" + message, guistyle);
		}
	}



	private void pick_or_drop_control()
	{
		do {
			if(! this.key.pick) { // 「拾う／捨てる」キーが押されていないなら.
				break; // 何もせずメソッド終了.
			}
			if(this.carried_item == null) { // 持ちあげ中アイテムがなく、.
				if(this.closest_item == null) { // 注目中アイテムがないなら.
					break; // 何もせずメソッド終了.
				}
				// 注目中のアイテムを、持ち上げる.
				this.carried_item = this.closest_item;
				// 持ち上げ中アイテムを、自分の子に設定.
				this.carried_item.transform.parent = this.transform;
				// 2.0f上に配置（頭の上に移動）.
				this.carried_item.transform.localPosition = Vector3.up * 2.0f;
				// 注目中アイテムをなくす.
				this.closest_item = null;
			} else { // 持ち上げ中アイテムがある場合.
				// 持ち上げ中アイテムをちょっと（1.0f）前に移動させて.
				this.carried_item.transform.localPosition =
					Vector3.forward * 1.0f;
				this.carried_item.transform.parent = null; // 子の設定を解除.
				this.carried_item = null; // 持ち上げ中アイテムをなくす.
			}
		} while(false);
	}

	private bool is_other_in_view(GameObject other)
	{
		bool ret = false;
		do {
			Vector3 heading = // 自分が現在向いている方向を保管.
				this.transform.TransformDirection(Vector3.forward);
			Vector3 to_other = // 自分から見たアイテムの方向を保管.
				other.transform.position - this.transform.position;
			heading.y = 0.0f;
			to_other.y = 0.0f;
			heading.Normalize(); // 長さを1にし、方向のみのベクトルに.
			to_other.Normalize(); // 長さを1にし、方向のみのベクトルに.
			float dp = Vector3.Dot(heading, to_other); // 両ベクトルの内積を取得.
			if(dp < Mathf.Cos(45.0f)) { // 内積が45度のコサイン値未満なら.
				break; // ループを抜ける.
			}
			ret = true; // 内積が45度のコサイン以上なら、正面にある.
		} while(false);
		return(ret);
	}

	private bool is_event_ignitable()
	{
		bool ret = false;
		do {
			if(this.closest_event == null) { // 注目イベントがなければ.
				break; // falseを返す.
			}
			// 持ち上げているアイテムの種類を取得.
			Item.TYPE carried_item_type =
				this.item_root.getItemType(this.carried_item);
			// 持ち上げているアイテムの種類と、注目しているイベントの種類から.
			// イベント可能かどうかを判定し、イベント不可ならfalseを返す.
			if(! this.event_root.isEventIgnitable(
				carried_item_type, this.closest_event)) {
				break;
			}
			ret = true; // ここまで来たらイベントを開始できると判定！.
		} while(false);
		return(ret);
	}






}

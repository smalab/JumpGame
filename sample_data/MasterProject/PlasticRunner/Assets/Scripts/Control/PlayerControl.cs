using UnityEngine;
using System.Collections;

// プレイヤー.
public class PlayerControl : MonoBehaviour {

	public static float	ACCELERATION = 10.0f;				// 加速度.
	public static float	SPEED_MIN = 4.0f;					// 速度の最小値.
	public static float	SPEED_MAX = 8.0f;					// 速度の最大値.
	
	public static float	JUMP_HEIGHT_MAX = 3.0f;				// ジャンプの高さ.
	public static float	JUMP_KEY_RELEASE_REDUCE = 0.5f;		// ジャンプ中にキーを離したときの、上昇速度の減衰.
	
	public static float	BIKKURI_HEIGHT_MAX = 2.0f;			// おじゃまキャラにさわったときに、飛び上がる高さ.

	public static float	NARAKU_HEIGHT = -5.0f;				// 画面下に消えたとみなせる、高さ.

	public static float	COLLISION_SIZE = 1.0f;				// コリジョン球の大きさ（直径）.

	public static float	COIN_DROP_INTERVAL = 0.2f;			// [sec] ジャンプ中にコインを落とす間隔.

	private static float	CLICK_GRACE_TIME = 0.3f;		// クリック後、ジャンプできるまでの猶予時間.

	// ---------------------------------------------------------------- //

	private	ScoreControl	score_control = null;		// スコアー管理.
	private CoinCreator		coin_creator  = null;		// コインツクラー.
	public  LevelControl	level_control = null;

	public	float		passage_time = 0.0f;
	public	float		current_speed = 0.0f;			// 現在の速度.
	private bool		is_landed = false;				// 着地してる？.
	private bool		is_colided = false;
	private bool		is_launched = false;			// ジャンプ直後、『着地した判定』が消えた？

	private bool		is_key_released = false;		// ジャンプ後、スペースキーを離した？.

	private	float		drop_timer = 0.0f;				// コイン落とし用のタイマー.

	private	float		click_timer = -1.0f;			// クリックされてからの時間.

	// ---------------------------------------------------------------- //

	public enum STEP {

		NONE = -1,

		RUN = 0,			// たち.
		JUMP,				// ジャンプ.
		MISS,				// ミス.

		TOUCH_ENEMY,		// おじゃまキャラに触った.
		OUT,				// おじゃまキャラに触ったあと、画面のしたに消えた.

		NUM,
	};

	public STEP			step      = STEP.NONE;
	public STEP			next_step = STEP.NONE;
	public float		step_timer = 0.0f;


	private Animation 		anim_player;		// motion.

	private SoundControl	sound_control = null;

	private BlockControl	stepped_block = null;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.next_step     = STEP.RUN;
		this.current_speed = SPEED_MIN;

		this.score_control = ScoreControl.getInstance();
		this.coin_creator  = CoinCreator.getInstance();

		this.anim_player = this.transform.GetComponentInChildren<Animation>();		//motion.

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();

	}

	private	bool	is_debug_immortal = false;

	// Update is called once per frame
	void	Update()
	{
		Vector3		velocity = this.rigidbody.velocity;

		// ---------------------------------------------------------------- //
		// クリックしてからの経過時間.
		//
		// 着地直後にボタンをクリックしても、キャラがジャンプしてくれない
		// ことがあります。これは着地の直後にボタンをクリックしたつもりで
		// も、実際には 着地の "直前" にクリックしているためです。
		// このようなときでもジャンプできるよう、『クリックした瞬間』の判
		// 定が数フレーム続くようにします。
		// 
		if(Input.GetMouseButtonDown(0)) {

			this.click_timer = 0.0f;

		} else {

			if(this.click_timer >= 0.0f) {

				this.click_timer += Time.deltaTime;
			}
		}

		// ---------------------------------------------------------------- //
		// スピードのコントロール.

		this.current_speed = this.level_control.getPlayerSpeed();

		// ---------------------------------------------------------------- //
		// Unity の物理エンジンのクセ？　で床ブロックの継ぎ目でへこへこ
		// はねてしまうので、むりやり下に押し付ける.

		if(this.step == STEP.TOUCH_ENEMY) {

		} else {

			if(this.is_colided) {

				if(velocity.y > Physics.gravity.y*Time.deltaTime) {

					velocity.y = Physics.gravity.y*Time.deltaTime;

					this.rigidbody.velocity = velocity;
				}
			}
	
			// 『着地してる？』を調べる.
			// （本来は Unity の機能を使えば十分だけど、押し出しによって
			// 　宙に浮いてしまうことがあるので、自前で調べる）.
			//
			this.check_landed();
		}

		// ---------------------------------------------------------------- //
		// 画面下に落下したらミス.

		switch(this.step) {

			case STEP.RUN:
			case STEP.JUMP:
			{
				if(this.transform.position.y < NARAKU_HEIGHT) {

					this.next_step = STEP.MISS;
				}
			}
			break;
		}

		// ---------------------------------------------------------------- //
		// ステップ内の経過時間を進める.

		this.step_timer += Time.deltaTime;

		// ---------------------------------------------------------------- //
		// 次の状態に移るかどうかを、チェックする.


		if(this.next_step == STEP.NONE) {

			switch(this.step) {
	
				case STEP.RUN:
				{
					// マウスの左ボタンが押されたらジャンプ.
					if(0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME) {

						if(this.is_landed || this.is_colided) {

							this.click_timer = -1.0f;
							this.next_step = STEP.JUMP;

						} else {

							// 足元に床がないときはジャンプできない.
						}
					}
				}
				break;

				case STEP.JUMP:
				{
					// 着地したら走りへ.
					if(this.is_launched) {

						if(this.is_landed) {

							this.sound_control.playSound(Sound.SOUND.TDOWN);

							if(this.stepped_block != null) {

								this.stepped_block.onStepped();
							}

							this.next_step = STEP.RUN;
						}
					}
				}
				break;

				case STEP.TOUCH_ENEMY:
				{
					if(this.transform.position.y < NARAKU_HEIGHT) {

						this.next_step = STEP.OUT;
					}
				}
				break;
			}
		}

		// ---------------------------------------------------------------- //
		// 状態が遷移したときの初期化.

		while(this.next_step != STEP.NONE) {

			this.step      = this.next_step;
			this.next_step = STEP.NONE;

			switch(this.step) {
	
				case STEP.RUN:
				{
					this.is_launched = false;
					this.anim_player.CrossFade("02_Move", 0.1f);		//motion
				}
				break;

				case STEP.JUMP:
				{
					velocity.y = PlayerControl.calcJumpVelocityY();

					this.is_launched     = false;
					this.is_key_released = false;
					this.anim_player.CrossFade("03_jumpup", 0.1f);		//motion
					
					this.sound_control.playSound(Sound.SOUND.JUMP);

					// ジャンプ時の音声は、jump1 jump2をランダムで再生しつつ、30％の確率で再生されないくらいに調整する..
					int rnd = Random.Range(0,3);
					switch(rnd){
					case 0:	this.sound_control.playSound(Sound.SOUND.JUMP1);	break;
					case 1:	this.sound_control.playSound(Sound.SOUND.JUMP2);	break;
					}

				}
				break;

				case STEP.TOUCH_ENEMY:
				{
					// 敵にさわった.

					// 画面の下に消えていくよう、コリジョンを無効にする.
					this.collider.enabled = false;

					this.next_step = STEP.MISS;
				}
				break;

				case STEP.MISS:
				{
					velocity.y = PlayerControl.calcBikkuriVelocityY();

					velocity.x = 0.0f;

					this.collider.enabled = false;

					this.anim_player.CrossFade("05_died", 0.1f);		//motion

					this.sound_control.playSound(Sound.SOUND.JINGLE);
				}
				break;
			}

			this.step_timer = 0.0f;
		}

		// ---------------------------------------------------------------- //
		// 各状態での実行処理.

		switch(this.step) {

			case STEP.RUN:
			{
				// 右方向に加速.
				velocity.x += PlayerControl.ACCELERATION*Time.deltaTime;

				// 最大速度以上にならないように.
				if(Mathf.Abs(velocity.x) > this.current_speed) {
					velocity.x *= this.current_speed/Mathf.Abs(velocity.x);
				}
			}
			break;

			case STEP.JUMP:
			{
				if(!this.is_landed || this.is_key_released || velocity.y <= 0.0f) {

					this.is_launched = true;
				}

				// ジャンプ中に左ボタンを離したら、上昇速度を減らす.
				// （左ボタンを押す長さでジャンプの高さを制御できるように）.
				do {

					if(Input.GetMouseButton(0)) {

						break;
					}

					// 一度離した後はやらない（連打対策）.
					if(this.is_key_released) {

						break;
					}

					// 下降中はやらない.
					if(velocity.y <= 0.0f) {

						break;
					}

					//
				
					velocity.y *= JUMP_KEY_RELEASE_REDUCE;

					this.is_key_released = true;

				} while(false);

				if(velocity.y <= 0.0f){

					this.anim_player.CrossFade("04_jumpdown", 0.3f);	// motion.
				}

			}
			break;

			case STEP.MISS:
			{
				// 減速.
				velocity.x -= PlayerControl.ACCELERATION*Time.deltaTime;

				// 0.0 以下にならないように.
				if(velocity.x < 0.0f) {

					velocity.x = 0.0f;
				}
			}
			break;

			case STEP.TOUCH_ENEMY:
			{
				velocity.x = 0.0f;
			}
			break;

		}

		// 『ジャンプ中にコイン落とす』のコントロール.
		this.coin_drop_control();

		// ---------------------------------------------------------------- //

		this.rigidbody.velocity = velocity;

		this.passage_time += Time.deltaTime;

		this.is_colided = false;
		//this.is_landed = false;

		// ---------------------------------------------------------------- //
		// デバッグ用無敵モード.
	#if true
		if(Input.GetMouseButtonDown(1)) {
			this.is_debug_immortal = !this.is_debug_immortal;
			if(this.is_debug_immortal) {
				this.collider.enabled     = false;
				this.rigidbody.useGravity = false;
				this.rigidbody.velocity   = new Vector3(this.rigidbody.velocity.x, 0.0f, this.rigidbody.velocity.z);
			} else {
				this.collider.enabled     = true;
				this.rigidbody.useGravity = true;
			}
		}
	#endif
	}

	// ---------------------------------------------------------------- //

	// コリジョンにヒットしている間中よばれるメソッド.
	void 	OnCollisionStay(Collision other)
	{
		do {
			if(other.gameObject.tag != "Floor") {
				break;
			}

			// 上昇中ならやらない.
			// ジャンプした瞬間に着地したことにならないように.
			// 相対速度が下向き＝自分がブロックから上方向に向かっている＝上昇中.
			if(other.relativeVelocity.y <= 0.0f) {
	
				break;
			}

			if(this.step == STEP.TOUCH_ENEMY) break;
			if(this.step == STEP.MISS) break;
			if(this.step == STEP.OUT) break;

			//this.is_landed = true;
			this.is_colided = true;

		} while(false);
	}

	// ================================================================ //
	
	// おじゃまキャラにさわったときによばれる.
	public void		onTouchEnemy(EnemyControl enemy)
	{
		do{

			if(this.step == STEP.TOUCH_ENEMY) break;
			if(this.step == STEP.MISS) break;
			if(this.step == STEP.OUT) break;

			this.next_step = STEP.TOUCH_ENEMY;

			Debug.Log("miss");

		}while(false);
	}

	// ゲームオーバー？.
	public bool		isPlayEnd()
	{
		bool	ret = false;

		switch(this.step) {

			case STEP.MISS:
			case STEP.OUT:
			{
				ret = true;
			}
			break;
		}

		return(ret);
	}

	// ジャンプ中？.
	public bool		isJumping()
	{
		bool	ret = false;

		switch(this.step) {

			case STEP.JUMP:
			{
				ret = true;
			}
			break;
		}

		return(ret);
	}

	// ================================================================ //

	// ジャンプするときの速度（上方向）を求める.
	public static float		calcJumpVelocityY()
	{
		// JUMP_HEIGHT_MAX の高さまでジャンプできる速度を求める.

		float	glavity = Mathf.Abs(Physics.gravity.y);
		float	vy      = Mathf.Sqrt(2.0f*glavity*JUMP_HEIGHT_MAX);

		return(vy);
	}

	// 敵にやられたあとの速度（上方向）を求める.
	public static float		calcBikkuriVelocityY()
	{
		// JUMP_HEIGHT_MAX の高さまでジャンプできる速度を求める.

		float	glavity = Mathf.Abs(Physics.gravity.y);
		float	vy      = Mathf.Sqrt(2.0f*glavity*BIKKURI_HEIGHT_MAX);

		return(vy);
	}

	// ================================================================ //

	// 『着地してる？』を調べる.
	private void	check_landed()
	{
		this.is_landed = false;

		do {

			// 真下に向かって線を伸ばして、他のオブジェクトとヒットするか、
			// 調べる.

			float		line_length;

			line_length = COLLISION_SIZE;

			// ジャンプ直後.
			if(this.step == STEP.JUMP) {

				if(this.is_launched) {

					line_length = COLLISION_SIZE/2.0f;
				}
			}

			Vector3		s = this.transform.position;
			Vector3		e = s + Vector3.down*line_length;
			RaycastHit	hit;

			// レイヤーマスクを指定して、フロアーだけ調べるようにする.
			// （コインとかは調べない）.

			int		layer_mask = 0;

			layer_mask += 1 << LayerMask.NameToLayer("Floor Block");

			if(!Physics.Linecast(s, e, out hit, layer_mask)) {

				// 他のオブジェクトとヒットしなかった.
				break;
			}

			// ブロックなら覚えておく.

			BlockControl	block = hit.collider.GetComponent<BlockControl>();

			if(block != null) {

				this.stepped_block = block;
			}

			//

			this.is_landed = true;

		} while(false);
	}

	// 『ジャンプ中にコイン落とす』のコントロール.
	// ジャンプ中、一定時間ごとにコインを落とす.
	private void	coin_drop_control()
	{
		float	drop_timer_prev = this.drop_timer;
		if(this.step == STEP.JUMP) {
			this.drop_timer += Time.deltaTime;
		} else {
			drop_timer_prev = 0.0f;
			this.drop_timer = 0.0f;
		}

		// drop_timer が "COIN_DROP_INTERVAL" をまたいだらコインを落とす.
		//
		// drop_timer_prev = COIN_DROP_INTERVAL*n
		// drop_timer      = COIN_DROP_INTERVAL*(n + 1)
		//
		// となったときが、drop_timer が "COIN_DROP_INTERVAL" の間隔をまたいだとき.
		//
		if(Mathf.Ceil(this.drop_timer/COIN_DROP_INTERVAL) > Mathf.Ceil(drop_timer_prev/COIN_DROP_INTERVAL)) {

			this.score_control.subDropScore();
			this.coin_creator.createDroppedCoin(this.transform.position);
		}
	}
}

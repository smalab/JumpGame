using UnityEngine;
using System.Collections;

// ブロック.
public struct Block {

	public static float		COLLISION_SIZE = 1.0f;

	// いろ.
	public enum COLOR {

		NONE = -1,

		PINK = 0,
		BLUE,
		GREEN,
		ORANGE,
		YELLOW,
		MAGENTA,

		NECO,

		GRAY,

		NUM,

		FIRST = PINK,
		//LAST = ORANGE,

		NORMAL_COLOR_NUM = GRAY,
	};

	// ４方向.
	public enum DIR4 {

		NONE = -1,

		RIGHT,
		LEFT,
		UP,
		DOWN,

		NUM,
	};

	public static int	BLOCK_NUM_X = 9;		// ブロックの数　横.
	public static int	BLOCK_NUM_Y = 9;		// ブロックの数　縦.

	// ブロックのマス目上の位置.
	public struct iPosition {

		public iPosition(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// クリアーする（無効な値にする）.
		public void		clear()
		{
			this.x = -1;
			this.y = -1;
		}

		// 有効？.
		public bool		isValid()
		{
			return(this.x >= 0 && this.y >= 0);
		}

		public int		x;
		public int		y;
	};

	// ブロックの状態.
	public enum STEP {

		NONE = -1,

		IDLE = 0,			// まち.
		GRABBED,			// つかまれ中.
		RELEASED,			// つかまれが終わった（連鎖チェック待ち中）.
		SLIDE,				// スライド中（となりのブロックと入れ替わる）.

		VACANT,				// 消えたあと.
		//RESPAWN,			// 消えたあとの復活（降ってくるのを作るまでのテスト用）.

		FALL,				// 下にあるブロックがきえたあとの、落ち中.

		LONG_SLIDE,

		NUM,
	};
}

// ブロックのコントロール.
public class BlockControl : MonoBehaviour {

	// ================================================================ //

	public	Block.COLOR	color = (Block.COLOR)0;

	public	BlockRoot			block_root  = null;
	public	LeaveBlockRoot		leave_block_root  = null;

	public	Block.iPosition		i_pos;

	public	Block.DIR4		slide_dir = Block.DIR4.NONE;

	private Vector3		position_offset_initial = Vector3.zero;
	public Vector3		position_offset         = Vector3.zero;

	public	float		vanish_timer = -1.0f;		// 色がそろった後の消え中用タイマー.
	
	protected float			vanish_spin = 0.0f;
	public float			grab_timer = 0.0f;
	public bool				slide_forward = true;
	protected float			vanish_facing_timer = 0.0f;		// [sec] 正面に向くようタイマー.

	protected static float	SLIDE_TIME = 0.2f;				// [sec] スライドにかかる時間.
	protected static float	GRAB_EFFECT_TIME = 0.1f;		// [sec] つかまれ演出の時間.


	public Material		opaque_material;			// 不透明用マテリアル.
	public Material		transparent_material;		// 半透明用マテリアル.

	private	Block.iPosition[]		connected_block;	// となりのブロック（同じ色のときのみ）.

	protected bool	is_visible = true;		// 表示中？.

	// ================================================================ //

	public Block.STEP	step      = Block.STEP.NONE;
	public Block.STEP	next_step = Block.STEP.NONE;
	public float		step_timer = 0.0f;

	// STEP.FALL で使う.
	private struct StepFall {

		public float	velocity;		// 現在の落下速度.
	}
	private StepFall	fall;

	private	GameObject		models_root = null;		// ブロックモデルたちの親.
	private	GameObject[]	models = null;			// 各色のブロックのモデル.

	public Transform	nekoAtama = null;
	private	Animation	neko_motion = null;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Awake()
	{
		// 各色のブロックのモデルを探しておく.

		this.models = new GameObject[(int)Block.COLOR.NORMAL_COLOR_NUM];

		this.models_root = this.transform.FindChild("models").gameObject;

		this.models[(int)Block.COLOR.PINK]    = this.models_root.transform.FindChild("block_pink").gameObject;
		this.models[(int)Block.COLOR.BLUE]    = this.models_root.transform.FindChild("block_blue").gameObject;
		this.models[(int)Block.COLOR.GREEN]   = this.models_root.transform.FindChild("block_green").gameObject;
		this.models[(int)Block.COLOR.ORANGE]  = this.models_root.transform.FindChild("block_orange").gameObject;
		this.models[(int)Block.COLOR.YELLOW]  = this.models_root.transform.FindChild("block_yellow").gameObject;
		this.models[(int)Block.COLOR.MAGENTA] = this.models_root.transform.FindChild("block_purple").gameObject;
		this.models[(int)Block.COLOR.NECO]    = this.models_root.transform.FindChild("neco").gameObject;

		// 非表示にするととれなくなるので注意.
		this.neko_motion = this.models[(int)Block.COLOR.NECO].GetComponentInChildren<Animation>();

		// いったん全部非表示.
		for(int i = 0;i < this.models.Length;i++) {

			this.models[i].SetActive(false);
		}

		// この色のブロックだけ表示する.
		this.setColor(this.color);

		if(this.next_step == Block.STEP.NONE) {

			this.next_step = Block.STEP.IDLE;
		}
		
		this.connected_block = new Block.iPosition[(int)Block.DIR4.NUM];

		for(int i = 0;i < this.connected_block.Length;i++) {

			this.connected_block[i].clear();
		}

		//
	}

	void 	Start()
	{
	}

	void 	Update()
	{
		// ---------------------------------------------------------------- //
		// ３D空間でのマウスの位置座標を求めておく.

		Vector3		mouse_position;

		this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition);

		Vector2		mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

		// ---------------------------------------------------------------- //

		// 消える演出タイマー.
		if(this.vanish_timer >= 0.0f) {

			this.vanish_timer -= Time.deltaTime;

			if(this.vanish_timer < 0.0f) {

				if(this.step != Block.STEP.SLIDE) {

					this.vanish_timer = -1.0f;

					// 退場演出用のブロックを作る.	
					this.leave_block_root.createLeaveBlock(this);

					for(int i = 0;i < this.connected_block.Length;i++) {
			
						this.connected_block[i].clear();
					}

					// （仮）.
					this.next_step = Block.STEP.VACANT;

				} else {

					this.vanish_timer = 0.0f;
				}
			}

			this.vanish_facing_timer += Time.deltaTime;
		}

		// ---------------------------------------------------------------- //
		// ステップ内の経過時間を進める.

		this.step_timer += Time.deltaTime;

		// ---------------------------------------------------------------- //
		// 次の状態に移るかどうかを、チェックする.


		if(this.next_step == Block.STEP.NONE) {

			switch(this.step) {
	
				case Block.STEP.IDLE:
				{
				}
				break;

				case Block.STEP.SLIDE:
				{
					if(this.step_timer >= SLIDE_TIME) {

						if(this.vanish_timer == 0.0f) {

							this.next_step = Block.STEP.VACANT;

						} else {

							this.next_step = Block.STEP.IDLE;
						}
					}
				}
				break;

				case Block.STEP.FALL:
				{
					if(this.position_offset.y <= 0.0f) {

						this.next_step = Block.STEP.IDLE;
						this.position_offset.y = 0.0f;
					}
				}
				break;
			}
		}

		// ---------------------------------------------------------------- //
		// 状態が遷移したときの初期化.

		while(this.next_step != Block.STEP.NONE) {

			this.step      = this.next_step;
			this.next_step = Block.STEP.NONE;

			switch(this.step) {
	
				case Block.STEP.IDLE:
				{
					this.setVisible(true);
					this.position_offset = Vector3.zero;
					this.vanish_spin     = 0.0f;
					this.vanish_facing_timer = 0.0f;
				}
				break;

				case Block.STEP.GRABBED:
				{
					this.setVisible(true);
				}
				break;

				case Block.STEP.SLIDE:
				{
				}
				break;

				case Block.STEP.RELEASED:
				{
					this.setVisible(true);
					this.position_offset = Vector3.zero;
				}
				break;

				case Block.STEP.VACANT:
				{
					this.position_offset = Vector3.zero;
					this.setVisible(false);
				}
				break;

				case Block.STEP.FALL:
				{
					this.setVisible(true);
					this.vanish_spin = 0.0f;
					this.vanish_facing_timer = 0.0f;
					this.fall.velocity = 0.0f;
				}
				break;
			}

			this.step_timer = 0.0f;
		}

		// ---------------------------------------------------------------- //
		// 各状態での実行処理.

		float	scale = 1.0f;

		// つかまれスケール.

		if(this.step == Block.STEP.GRABBED) {

			this.grab_timer += Time.deltaTime;

		} else {

			this.grab_timer -= Time.deltaTime;
		}

		this.grab_timer = Mathf.Clamp(this.grab_timer, 0.0f, GRAB_EFFECT_TIME);

		float	grab_ratio = Mathf.Clamp01(this.grab_timer/GRAB_EFFECT_TIME);

		scale = Mathf.Lerp(1.0f, 1.3f, grab_ratio);

		//

		this.models_root.transform.localPosition = Vector3.zero;

		if(this.vanish_timer > 0.0f) {

			// 他のブロックより手前に表示されるように.
			this.models_root.transform.localPosition = Vector3.back;
		}

		switch(this.step) {

			case Block.STEP.IDLE:
			{
			}
			break;

			case Block.STEP.FALL:
			{
				this.fall.velocity += Physics.gravity.y*Time.deltaTime*0.3f;
				this.position_offset.y += this.fall.velocity*Time.deltaTime;

				if(this.position_offset.y < 0.0f) {

					this.position_offset.y = 0.0f;
				}
			}
			break;

			case Block.STEP.GRABBED:
			{
				// スライド方向.
				this.slide_dir = this.calcSlideDir(mouse_position_xy);

				// 他のブロックより手前に表示されるように.
				this.models_root.transform.localPosition = Vector3.back;
			}
			break;

			case Block.STEP.SLIDE:
			{
				float	ratio = this.step_timer/SLIDE_TIME;
			
				ratio = Mathf.Min(ratio, 1.0f);
				ratio = Mathf.Sin(ratio*Mathf.PI/2.0f);

				this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, ratio);

				//

				ratio = this.step_timer/SLIDE_TIME;
				ratio = Mathf.Min(ratio, 1.0f);
				ratio = Mathf.Sin(ratio*Mathf.PI);

				if(this.slide_forward) {

					scale += Mathf.Lerp(0.0f, 0.5f, ratio);

				} else {

					scale += Mathf.Lerp(0.0f, -0.5f, ratio);
				}
			}
			break;
		}

		// ---------------------------------------------------------------- //
		// ポジション.

		Vector3		position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;

		this.transform.position = position;

		// ---------------------------------------------------------------- //
		// 消える演出.

		this.models_root.transform.localRotation = Quaternion.identity;

		if(this.vanish_timer >= 0.0f) {

			// facing ... ブロックの上面（ぽっちがある面）を手前にむける回転.
			// vanish ... くるくる回る回転.

			float	facing_ratio = Mathf.InverseLerp(0.0f, 0.2f, this.vanish_facing_timer);

			facing_ratio = Mathf.Clamp01(facing_ratio);

			float	vanish_ratio = Mathf.InverseLerp(0.0f, this.block_root.level_control.getVanishTime(), this.vanish_timer);
			float	spin_speed   = vanish_ratio;

			this.vanish_spin += spin_speed*10.0f;

			if(this.color != Block.COLOR.NECO) {

				this.models_root.transform.localRotation *= Quaternion.AngleAxis(-90.0f*facing_ratio, Vector3.right);
				this.models_root.transform.localRotation *= Quaternion.AngleAxis(this.vanish_spin, Vector3.up);
			}
		}

		this.nekoAtama.localScale = Vector3.one*1.0f;
		this.models_root.transform.localScale = Vector3.one*scale;

		// ---------------------------------------------------------------- //

		if(this.color == Block.COLOR.NECO) {

			float	anim_speed = 1.0f;

			if(this.vanish_timer >= 0.0f) {

				float	vanish_ratio = this.calc_neko_vanish_ratio();

				anim_speed  = Mathf.Lerp(1.0f, 1.5f, vanish_ratio);
			}

			this.neko_motion["00_Idle"].speed = anim_speed;
		}
	}

	void	LateUpdate()
	{
		// ねこのくびふりIK.
		// アニメーションの結果を上書きしないといけないので、
		// UPdate() じゃなくて LateUpdate() でやる.
		do {

			if(this.color != Block.COLOR.NECO) {

				break;
			}
			if(this.vanish_timer < 0.0f) {

				break;
			}

			float	vanish_ratio = this.calc_neko_vanish_ratio();

			float	size_scale  = Mathf.Lerp(1.0f, 1.5f, vanish_ratio);
			float	angle_scale = Mathf.Lerp(1.0f, 2.5f, vanish_ratio);

			this.nekoAtama.localScale = Vector3.one*size_scale;

			float		angle;
			Vector3		axis;

			this.nekoAtama.localRotation.ToAngleAxis(out angle, out axis);
			this.nekoAtama.localRotation = Quaternion.AngleAxis(angle*angle_scale, axis);

		} while(false);
	}

	// ねこの消失演出の補間率計算.
	private float	calc_neko_vanish_ratio()
	{
		float	vanish_ratio = Mathf.InverseLerp(this.block_root.level_control.getVanishTime(), 0.0f, this.vanish_timer);

		if(vanish_ratio < 0.1f) {

			vanish_ratio = Mathf.InverseLerp(0.0f, 0.1f, vanish_ratio);
			vanish_ratio = Mathf.Lerp(0.0f, 1.0f, vanish_ratio);

		} else if(vanish_ratio < 0.5f) {

			vanish_ratio = Mathf.InverseLerp(0.1f, 0.5f, vanish_ratio);
			vanish_ratio = Mathf.Lerp(1.0f, 0.9f, vanish_ratio);

		} else {

			vanish_ratio = Mathf.InverseLerp(0.5f, 1.0f, vanish_ratio);
			vanish_ratio = vanish_ratio*vanish_ratio;

			vanish_ratio = Mathf.Lerp(0.9f, 0.0f, vanish_ratio);
		}

		return(vanish_ratio);
	}

	// ================================================================ //

	// ねこの Animation コンポーネントをゲットする.
	public Animation	getNekoMotion()
	{
		return(this.neko_motion);
	}

	// となりのブロックをセットする（同じ色のときのみ）.
	public void		setConnectedBlock(Block.DIR4 dir, Block.iPosition connected)
	{
		this.connected_block[(int)dir] = connected;
	}

	// となりのブロックをゲットする（同じ色のときのみ）.
	public Block.iPosition	getConnectedBlock(Block.DIR4 dir)
	{
		return(this.connected_block[(int)dir]);
	}

	// 落下はじめ（下にあるブロックが消えたとき）.
	public void		beginFall(BlockControl start)
	{
		this.next_step = Block.STEP.FALL;

		this.position_offset.y = (float)(start.i_pos.y - this.i_pos.y)*Block.COLLISION_SIZE;
	}

	// 『スライド』動作はじめ.
	public void	beginSlide(Vector3 offset)
	{
		this.position_offset_initial = offset;
		this.position_offset         = this.position_offset_initial;
		this.next_step = Block.STEP.SLIDE;
	}
	// 『つかまれ動作』はじめ.
	public void	beginGrab()
	{
		this.next_step = Block.STEP.GRABBED;
	}
	// 『つかまれ動作』おしまい.
	public void	endGrab()
	{
		this.block_root.hideArrow();
		this.next_step = Block.STEP.IDLE;
	}

	// 『着火演出』はじめ.
	public void		toVanishing()
	{
		float	vanish_time = this.block_root.level_control.getVanishTime();

		this.vanish_timer = vanish_time;

		this.block_root.effect_control.createEffect(this);
	}

	// 『消えた後に上から降ってくる』はじめ.
	public void		beginRespawn(int start_ipos_y)
	{
		this.position_offset.y = (float)(start_ipos_y - this.i_pos.y)*Block.COLLISION_SIZE;

		this.next_step = Block.STEP.FALL;

		Block.COLOR		color = this.block_root.selectBlockColor();

		this.setColor(color);

		for(int i = 0;i < this.connected_block.Length;i++) {

			this.connected_block[i].clear();
		}
	}

	// ================================================================ //

	// dir 方向のオフセットを求める.
	public float		calcDirOffset(Vector2 position, Block.DIR4 dir)
	{
		float	offset = 0.0f;

		Vector2	v = position - new Vector2(this.transform.position.x, this.transform.position.y);

		switch(dir) {

			case Block.DIR4.RIGHT:	offset =  v.x;	break;
			case Block.DIR4.LEFT:	offset = -v.x;	break;
			case Block.DIR4.UP:		offset =  v.y;	break;
			case Block.DIR4.DOWN:	offset = -v.y;	break;
		}

		return(offset);
	}

	// つかめる？.
	public bool		isGrabbable()
	{
		bool	is_grabbable = false;

		switch(this.step) {

			case Block.STEP.IDLE:
			{
				// 発火中は移動できない
				if(!this.isVanishing()) {
 
					is_grabbable = true;
				}
			}
			break;
		}

		return(is_grabbable);
	}

	// スライド中？.
	public bool		isSliding()
	{
		bool	is_sliding = (this.position_offset.x != 0.0f);

		return(is_sliding);
	}

	// 着火中？.
	public bool		isVanishing()
	{
		bool	is_vanishing = (this.vanish_timer > 0.0f);

		return(is_vanishing);
	}
	
	// 着火中タイマーを戻す.
	public void		rewindVanishTimer()
	{
		float	vanish_time = this.block_root.level_control.getVanishTime();

		this.vanish_timer = vanish_time;
	}

	// そっちにスライドできる？
	public bool		isSlidable(Block.DIR4 dir)
	{
		bool	ret = false;

		switch(dir) {

			case Block.DIR4.RIGHT:	ret = (this.i_pos.x < Block.BLOCK_NUM_X - 1);	break;
			case Block.DIR4.LEFT:	ret = (this.i_pos.x > 0);						break;
			case Block.DIR4.UP:		ret = (this.i_pos.y < Block.BLOCK_NUM_Y - 1);	break;
			case Block.DIR4.DOWN:	ret = (this.i_pos.y > 0);						break;
		}

		return(ret);
	}

	// スライド入力の方向を求める.
	public Block.DIR4	calcSlideDir(Vector2 mouse_position)
	{
		Block.DIR4	dir = Block.DIR4.NONE;

		Vector2		v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y);

		if(v.magnitude > 0.1f) {

			if(v.y > v.x) {
	
				if(v.y > -v.x) {
	
					dir = Block.DIR4.UP;
	
				} else {
	
					dir = Block.DIR4.LEFT;
				}
	
			} else {
	
				if(v.y > -v.x) {
	
					dir = Block.DIR4.RIGHT;
	
				} else {
	
					dir = Block.DIR4.DOWN;
				}
			}
		}

		return(dir);
	}

	// 反対の方向を求める.
	static public Block.DIR4	getOppose(Block.DIR4 dir)
	{
		Block.DIR4	oppose = Block.DIR4.NONE;

		switch(dir) {

			case Block.DIR4.RIGHT:	oppose = Block.DIR4.LEFT;	break;
			case Block.DIR4.LEFT:	oppose = Block.DIR4.RIGHT;	break;
			case Block.DIR4.UP:		oppose = Block.DIR4.DOWN;	break;
			case Block.DIR4.DOWN:	oppose = Block.DIR4.UP;	break;
		}

		return(oppose);
	}

	public static Block.iPosition		getNext_iPosition(Block.iPosition i_pos, Block.DIR4 dir)
	{
		Block.iPosition		next_ipos = i_pos;

		switch(dir) {

			case Block.DIR4.RIGHT:	next_ipos.x += 1;	break;
			case Block.DIR4.LEFT:	next_ipos.x -= 1;	break;
			case Block.DIR4.UP:		next_ipos.y += 1;	break;
			case Block.DIR4.DOWN:	next_ipos.y -= 1;	break;
		}

		return(next_ipos);
	}

	public static bool		isValid_iPosition(Block.iPosition i_pos)
	{
		bool	is_valid;

		do {

			is_valid = false;

			if(i_pos.x < 0 || Block.BLOCK_NUM_X <= i_pos.x) {

				break;
			}
			if(i_pos.y < 0 || Block.BLOCK_NUM_Y <= i_pos.y) {

				break;
			}
		
			is_valid = true;

		} while(false);

		return(is_valid);
	}

	// ブロックがアイドル状態？（何もしてない中？）.
	public bool		isIdle()
	{
		bool	is_idle = false;

		if(this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE) {

			is_idle = true;
		}

		return(is_idle);
	}

	// ブロックがからっぽ？（消えた後）.
	public bool		isVacant()
	{
		bool	is_vacant = false;

		if(this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE) {

			is_vacant = true;
		}

		return(is_vacant);
	}

	// ブロックが表示中？.
	public bool		isVisible()
	{
		return(this.is_visible);
	}
	// ブロックの表示/非表示をセットする.
	public void		setVisible(bool is_visible)
	{
		if(this.is_visible != is_visible) {

			this.is_visible = is_visible;

			this.models_root.SetActive(this.is_visible);
		}
	}


	// position がブロックの中にある？
	public bool		isContainedPosition(Vector2 position)
	{
		bool		ret = false;
		Vector3		center = this.transform.position;
		float		h = Block.COLLISION_SIZE/2.0f;

		do {

			if(position.x < center.x - h || center.x + h < position.x) {

				break;
			}
			if(position.y < center.y - h || center.y + h < position.y) {

				break;
			}

			ret = true;

		} while(false);

		return(ret);
	}

	// ブロックの色をセットする.
	public void		setColor(Block.COLOR color)
	{
		this.color = color;

		if(this.models != null) {

			foreach(var model in this.models) {
	
				model.SetActive(false);
			}
	
			switch(this.color) {
	
				case Block.COLOR.PINK:
				case Block.COLOR.BLUE:
				case Block.COLOR.YELLOW:
				case Block.COLOR.GREEN:
				case Block.COLOR.MAGENTA:
				case Block.COLOR.ORANGE:
				case Block.COLOR.NECO:
				{
					this.models[(int)this.color].SetActive(true);
				}
				break;
			}
		}
	}

	// ModelRoot（各色ブロックモデルの親）をゲットする.
	public GameObject	getModelsRoot()
	{
		return(this.models_root);
	}
}

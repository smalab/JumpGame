using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockRoot : MonoBehaviour {

	private	GameObject		main_camera      = null;
	private ScoreCounter	score_counter    = null;
	public	GameObject		blockPrefab      = null;
	public	GameObject		leaveBlockPrefab = null;

	public	GameObject		arrowPrefab = null;
	public	GameObject		arrow = null;

	public 	BlockControl[,]	blocks;							// ブロック.

	private int[,]			block_islands;					// 着火時に島の数を数えるために使う.
	private BlockControl	grabbed_block = null;

	private bool			is_vanishing_prev = false;

	public	TextAsset		levelData = null;				// レベルデーターのテキストファイル.
	public	LevelControl	level_control;					// レベルデーターのコントロール.

	public SoundControl		sound_control;

	public VanishEffectControl	effect_control = null;		// 着火したときエフェクトをつくるクラス.

	public LeaveBlockRoot	leave_block_root = null;

	private bool	neco_fever = false;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Awake()
	{
		this.main_camera   = GameObject.FindGameObjectWithTag("MainCamera");
		this.score_counter = this.gameObject.GetComponent<ScoreCounter>();

		this.arrow = GameObject.Instantiate(this.arrowPrefab) as GameObject;

		this.hideArrow();

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();

		this.effect_control = this.gameObject.GetComponent<VanishEffectControl>();
		this.leave_block_root = this.gameObject.GetComponent<LeaveBlockRoot>();
	}

	void	Start()
	{
	}

	void	Update()
	{
		Vector3		mouse_position;

		this.unprojectMousePosition(out mouse_position, Input.mousePosition);

		Vector2		mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);

		if(this.grabbed_block == null) {

			// ブロックつかみ中じゃない.
			do {

				// 画面のどこかに落下中のブロックがあるときはつかめない.
				if(this.is_has_falling_block()) {

					//break;
				}

				// マウスをクリックした瞬間じゃない.
				if(!Input.GetMouseButtonDown(0)) {

					break;
				}

				// つかめるかどうか、全部のブロックでチェックする.
				foreach(BlockControl block in this.blocks) {

					// つかめない状態のブロックはパス.
					if(!block.isGrabbable()) {

						continue;
					}

					// マウスカーソルが "重なっていない" ブロックはパス.
					if(!block.isContainedPosition(mouse_position_xy)) {

						continue;
					}

					this.grabbed_block = block;
					this.grabbed_block.beginGrab();

					this.sound_control.playSound(Sound.SOUND.GRAB);		// sound.

					break;
				}

			} while(false);

		} else {

			// ブロックつかみ中.
			do {

				// （仮）で、いったんやじるしを消す.
				this.hideArrow();

				BlockControl	swap_target = this.getNextBlock(grabbed_block, grabbed_block.slide_dir);

				if(swap_target == null) {

					break;
				}

				// つかめないブロック（消え中、落下中など）は入れかえられない.
				if(!swap_target.isGrabbable()) {

					break;
				}

				// スライドできるときは、やじるし表示.
				this.dispArrow(grabbed_block, grabbed_block.slide_dir);

				// ブロックの中心位置からマウスカーソルまでの距離.
				float	offset = this.grabbed_block.calcDirOffset(mouse_position_xy, this.grabbed_block.slide_dir);

				// グラブ中のブロックが半分以上スライドしたら、となりのブロックと入れ替え.
				if(offset < Block.COLLISION_SIZE/2.0f) {

					break;
				}

				// いれかえ開始.
				this.swapBlock(grabbed_block, grabbed_block.slide_dir, swap_target);
				this.hideArrow();
				this.grabbed_block = null;

				this.sound_control.playSound(Sound.SOUND.SLIDE);		// sound.

			} while(false);

			// ボタンを離したらはなす.
			if(!Input.GetMouseButton(0)) {
				this.grabbed_block.endGrab();
				this.grabbed_block = null;
			}
		}

		// ---------------------------------------------------------------- //
		// 着火チェック.

		// 落下中のブロックがある＝連鎖が途切れたとき.
		if(this.is_has_falling_block()){

			// 連鎖数をクリアーする.
			this.score_counter.clearIgniteCount();
		}

		if(this.is_has_falling_block() || this.is_has_sliding_block()) {

			// 画面内のどこかに『落下中』か『スライド中』のブロックがある
			// ときは着火チェックしない

		} else {

			// 着火したブロックの数を数える.
			int		ignite_count = 0;

			foreach(BlockControl block in this.blocks) {

				if(!block.isIdle()) {

					continue;
				}

				if(this.checkConnection(block)) {

					ignite_count++;
				}
			}

			// どこかでブロックが着火したら……
			if(ignite_count > 0) {

				// this.sound_control.isSoundPlay(Sound.SOUND.IGNIT1);		// sound.

				if(!this.is_vanishing_prev) {

					// 着火開始時に着火数カウントをクリアーする.
					this.score_counter.clearIgniteCount();
				}

				// 着火回数をプラスする.
				this.score_counter.addIgniteCount(ignite_count);

				this.score_counter.updateTotalScore();		// asuna

				// 着火中のブロック全ての着火中タイマーを戻す.
				//ついでに着火ブロック数も数える.

				int		block_count = 0;

				foreach(BlockControl block in this.blocks) {

					if(block.isVanishing()) {

						block.rewindVanishTimer();

						block_count++;
					}
				}

				// 着火したブロックの数をセットする.
				this.score_counter.setIgniteBlockCount(block_count);

				if(this.score_counter.getIgniteCount() >= 5) {

					this.neco_fever = true;
				}

				// 島の数を数える.
				this.count_islands();
			}
		}

		// ---------------------------------------------------------------- //
		// 消えたブロックの上にあるブロックを、下に落とす.

		bool	is_vanishing = this.is_has_vanishing_block();

		do {

			// 消える中のブロック（後づけできる）がある間は
			// 落下を開始しない.
			if(is_vanishing) {

				break;
			}

			// スライド中のブロックがある場合も落下を開始しない.
			if(this.is_has_sliding_block()) {

				break;
			}

			// ------------------------------------------------------ //

			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {
	
				// この列のどこかにスライド中のブロックがあったらパス.
				if(this.is_has_sliding_block_in_column(x)) {
	
					continue;
				}
	
				for(int y = 0;y < Block.BLOCK_NUM_Y - 1;y++) {
	
					if(!this.blocks[x, y].isVacant()) {
	
						continue;
					}
	
					for(int y1 = y + 1;y1 < Block.BLOCK_NUM_Y;y1++) {
	
						if(this.blocks[x, y1].isVacant()) {
		
							continue;
						}
	
						// [x, y] ～ [x, y1 - 1] のブロックが消えたので、[x, y1] にある
						// ブロックを下に落とす.
						this.fallBlock(this.blocks[x, y], Block.DIR4.UP, this.blocks[x, y1]);
						break;
					}
				}
			}

			// スキマの上のブロックを下に落とした後、からになったところに
			// 新たにブロックをつくる（画面の上から降ってくる）.
			//
			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {
	
				// 新しいブロックが出現する位置.
				int		fall_start_y = Block.BLOCK_NUM_Y;
	
				for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {
	
					if(!this.blocks[x, y].isVacant()) {
	
						continue;
					}
					this.blocks[x, y].beginRespawn(fall_start_y);
	
					// 同時に出現するブロックがかさなってしまわないよう、
					// 出現位置を上にずらしていく.
					fall_start_y++;
				}
			}

		} while(false);

		// ---------------------------------------------------------------- //

		if(!is_vanishing && this.is_vanishing_prev) {

			// トータルスコアーを更新する.
			// 着火が終わったときに、今回の着火でかせいだ点数がまとめて入るように.
			this.score_counter.updateTotalScore();
			this.sound_control.playSound(Sound.SOUND.CLEAR);		// sound.
		}

		this.is_vanishing_prev = is_vanishing;
	}

	// 島の数を数える.
	private void	count_islands()
	{
		for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {

			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {

				this.block_islands[x, y] = -1;
			}
		}

		int				island_index = 0;
		int				max_connect  = 0;
		Block.COLOR		block_color  = Block.COLOR.NONE;

		for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {

			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {

				block_color = this.blocks[x, y].color;

				int	connect_count = this.check_island_sub(x, y, block_color, island_index, 0);

				if(connect_count > 0) {

					island_index++;
					max_connect = Mathf.Max(max_connect, connect_count);
				}
			}
		}

		this.score_counter.setIslandCount(island_index);
		this.score_counter.setMaxIslandSize(max_connect);
	}

	private int		check_island_sub(int x, int y, Block.COLOR color, int island_index, int connect_count)
	{
		do {

			if(this.block_islands[x, y] != -1) {

				break;
			}
			if(!this.blocks[x, y].isVanishing()) {

				continue;
			}
			if(this.blocks[x, y].color != color) {

				break;
			}

			//

			this.block_islands[x, y] = island_index;

			connect_count++;

			if(0 < x) {

				connect_count = this.check_island_sub(x - 1, y, color, island_index, connect_count);
			}
			if(x < Block.BLOCK_NUM_X - 1) {

				connect_count = this.check_island_sub(x + 1, y, color, island_index, connect_count);
			}
			if(0 < y) {

				connect_count = this.check_island_sub(x, y - 1, color, island_index, connect_count);
			}
			if(y < Block.BLOCK_NUM_Y - 1) {

				connect_count = this.check_island_sub(x, y + 1, color, island_index, connect_count);
			}

		} while(false);

		return(connect_count);
	}

	// 画面のどこかに消える中のブロックがある？
	private bool	is_has_vanishing_block()
	{
		bool	ret = false;

		foreach(BlockControl block in this.blocks) {

			if(block.vanish_timer > 0.0f) {

				ret = true;
				break;
			}
		}

		return(ret);
	}

	// 画面のどこかにスライド中のブロックがある？
	private bool	is_has_sliding_block()
	{
		bool	ret = false;

		foreach(BlockControl block in this.blocks) {

			if(block.step == Block.STEP.SLIDE) {

				ret = true;
				break;
			}
		}

		return(ret);
	}
	// 画面のどこかに落下中のブロックがある？
	private bool	is_has_falling_block()
	{
		bool	ret = false;

		foreach(BlockControl block in this.blocks) {

			if(block.step == Block.STEP.FALL) {

				ret = true;
				break;
			}
		}

		return(ret);
	}


	// たて一行のどこかに、スライド中のブロックがある？.
	private bool	is_has_sliding_block_in_column(int x)
	{
		bool	ret = false;

		for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {

			if(this.blocks[x, y].isSliding()) {

				ret = true;
				break;
			}
		}

		return(ret);
	}

	// ================================================================ //

	// ブロックを落とす.
	public void		fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		// 落下させるブロックがつかみ中だったら離す.
		if(this.grabbed_block == block0 || this.grabbed_block == block1) {

			this.hideArrow();
			this.grabbed_block = null;
		}

		//

		Block.COLOR		color0 = block0.color;
		Block.COLOR		color1 = block1.color;

		Vector3	scale0 = block0.transform.localScale;
		Vector3	scale1 = block1.transform.localScale;

		float	vanish_timer0 = block0.vanish_timer;
		float	vanish_timer1 = block1.vanish_timer;

		bool	visible0 = block0.isVisible();
		bool	visible1 = block1.isVisible();

		Block.STEP	step0 = block0.step;
		Block.STEP	step1 = block1.step;

		float	frame0 = 0.0f;
		float	frame1 = 0.0f;

		if(color1 == Block.COLOR.NECO) {

			frame0 = block0.getNekoMotion()["00_Idle"].time;
			frame1 = block1.getNekoMotion()["00_Idle"].time;
		}

		//

		block0.setColor(color1);
		block1.setColor(color0);

		block0.transform.localScale = scale1;
		block1.transform.localScale = scale0;

		block0.vanish_timer = vanish_timer1;
		block1.vanish_timer = vanish_timer0;

		block0.setVisible(visible1);
		block1.setVisible(visible0);

		block0.step = step1;
		block1.step = step0;

		if(color1 == Block.COLOR.NECO) {

			block0.getNekoMotion()["00_Idle"].time = frame1;
			block1.getNekoMotion()["00_Idle"].time = frame0;
		}

		block0.beginFall(block1);
	}

	// ================================================================ //

	public void		create()
	{
		this.level_control = new LevelControl();
		this.level_control.initialize();
		this.level_control.loadLevelData(this.levelData);
		this.level_control.selectLevel();
	}

	// 初期配置する.
	public void		initialSetUp()
	{
		// ブロックを生成、配置する.

		this.blocks       = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];

		Block.COLOR			color = Block.COLOR.FIRST;

		for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {

			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {

				GameObject game_object = Instantiate(this.blockPrefab) as GameObject;

				BlockControl	block = game_object.GetComponent<BlockControl>();

				this.blocks[x, y] = block;

				block.i_pos.x = x;
				block.i_pos.y = y;
				block.block_root = this;
				block.leave_block_root = this.leave_block_root;

				//

				Vector3	position = BlockRoot.calcBlockPosition(block.i_pos);

				block.transform.position = position;

				color = this.selectBlockColor();
				block.setColor(color);

				// ヒエラルキービューで位置が確認しやすいよう、ブロックの座標を名前につけておく.
				block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
			}
		}

		//

		this.block_islands = new int[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];

		for(int y = 0;y < Block.BLOCK_NUM_Y;y++) {

			for(int x = 0;x < Block.BLOCK_NUM_X;x++) {

				this.block_islands[x, y] = -1;
			}
		}
	}

	// レベルデーターの確率で、ランダムにブロックの色を選択する.
	public Block.COLOR	selectBlockColor()
	{
		Block.COLOR	color = Block.COLOR.FIRST;

		LevelData	level_data = this.level_control.getCurrentLevelData();

		float	rand = Random.Range(0.0f, 1.0f);
		float	sum  = 0.0f;
		int		i = 0;

		for(i = 0;i < level_data.probability.Length - 1;i++) {

			if(level_data.probability[i] == 0.0f) {

				continue;
			}

			sum += level_data.probability[i];

			if(rand < sum) {

				break;
			}
		}

		color = (Block.COLOR)i;

		if(this.neco_fever) {

			if(color == Block.COLOR.BLUE) {
	
				color = Block.COLOR.NECO;
			}
		}

		return(color);
	}

	// グリッド番号（iPosition）から座標を求める.
	public static Vector3	calcBlockPosition(Block.iPosition i_pos)
	{
		Vector3		position = new Vector3(-(Block.BLOCK_NUM_X/2.0f - 0.5f), -(Block.BLOCK_NUM_Y/2.0f - 0.5f), 0.0f);

		position.x += (float)i_pos.x*Block.COLLISION_SIZE;
		position.y += (float)i_pos.y*Block.COLLISION_SIZE;

		return(position);
	}

	// ================================================================ //

	// dir 方向のとなりのブロックをゲットする.
	public BlockControl		getNextBlock(BlockControl block, Block.DIR4 dir)
	{
		BlockControl	next_block = null;

		switch(dir) {

			case Block.DIR4.RIGHT:
			{
				if(block.i_pos.x < Block.BLOCK_NUM_X - 1) {

					next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
				}
			}
			break;

			case Block.DIR4.LEFT:
			{
				if(block.i_pos.x > 0) {

					next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
				}
			}
			break;

			case Block.DIR4.UP:
			{
				if(block.i_pos.y < Block.BLOCK_NUM_Y - 1) {

					next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
				}
			}
			break;

			case Block.DIR4.DOWN:
			{
				if(block.i_pos.y > 0) {

					next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
				}
			}
			break;
		}

		return(next_block);
	}

	// ４方向（dir4）からベクトルをゲットする.
	public static Vector3	getDirVector(Block.DIR4 dir)
	{
		Vector3		v = Vector3.zero;

		switch(dir) {

			case Block.DIR4.RIGHT:	v = Vector3.right;	break;
			case Block.DIR4.LEFT:	v = Vector3.left;	break;
			case Block.DIR4.UP:		v = Vector3.up;		break;
			case Block.DIR4.DOWN:	v = Vector3.down;	break;
		}

		v *= Block.COLLISION_SIZE;

		return(v);
	}

	// 反対方向をゲットする.
	public static Block.DIR4	getOppositDir(Block.DIR4 dir)
	{
		Block.DIR4		opposit = dir;

		switch(dir) {

			case Block.DIR4.RIGHT:	opposit = Block.DIR4.LEFT;	break;
			case Block.DIR4.LEFT:	opposit = Block.DIR4.RIGHT;	break;
			case Block.DIR4.UP:		opposit = Block.DIR4.DOWN;	break;
			case Block.DIR4.DOWN:	opposit = Block.DIR4.UP;	break;
		}

		return(opposit);
	}

	// ふたつのブロックを入れかえる.
	public void		swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		// 入れかえる相手は反対方向にスライド.
		block1.slide_dir = BlockRoot.getOppositDir(dir);

		Block.COLOR		color0 = block0.color;
		Block.COLOR		color1 = block1.color;

		Vector3	scale0 = block0.transform.localScale;
		Vector3	scale1 = block1.transform.localScale;

		float	vanish_timer0 = block0.vanish_timer;
		float	vanish_timer1 = block1.vanish_timer;

		Vector3	offset0 = BlockRoot.getDirVector(dir);
		Vector3	offset1 = BlockRoot.getDirVector(block1.slide_dir);

		float	grab_timer0 = block0.grab_timer;	
		float	grab_timer1 = block1.grab_timer;	

		//

		block0.setColor(color1);
		block1.setColor(color0);

		block0.transform.localScale = scale1;
		block1.transform.localScale = scale0;

		block0.vanish_timer = vanish_timer1;
		block1.vanish_timer = vanish_timer0;

		block0.grab_timer = grab_timer1;
		block1.grab_timer = grab_timer0;

		block0.slide_forward = false;
		block1.slide_forward = true;

		block0.beginSlide(offset0);
		block1.beginSlide(offset1);
	}

	// マウスの位置を、３D空間のワールド座標に変換する.
	//
	// ・マウスカーソルとカメラの位置を通る直線
	// ・地面の当たり判定となる平面
	//　↑の二つが交わるところを求めます.
	//
	public bool		unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
	{
		bool	ret;

		// 地面の当たり判定となる平面.
		Plane	plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE/2.0f));

		// カメラ位置とマウスカーソルの位置を通る直線.
		Ray		ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);

		// 上の二つが交わるところを求める.

		float	depth;

		if(plane.Raycast(ray, out depth)) {

			world_position = ray.origin + ray.direction*depth;

			ret = true;

		} else {

			world_position = Vector3.zero;

			ret = false;
		}

		return(ret);
	}

	// スライド矢印を表示する.
	public void		dispArrow(BlockControl block, Block.DIR4 dir)
	{
		this.arrow.gameObject.SetActive(true);
		this.arrow.gameObject.transform.position = block.transform.position + Vector3.back*(Block.COLLISION_SIZE*5.0f + 0.01f);

		float	angle = 0.0f;

		switch(dir) {

			case Block.DIR4.RIGHT:	angle =   0.0f;	break;
			case Block.DIR4.LEFT:	angle = 180.0f;	break;
			case Block.DIR4.UP:		angle =  90.0f;	break;
			case Block.DIR4.DOWN:	angle = -90.0f;	break;
		}

		this.arrow.gameObject.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	// スライド矢印を非表示にする.
	public void		hideArrow()
	{
		this.arrow.gameObject.SetActive(false);
	}

	// ================================================================ //

	// 『同じ色が並んでる？』チェック.
	public bool		checkConnection(BlockControl start)
	{
		bool	ret = false;

		int		normal_block_num = 0;

		if(!start.isVanishing()) {

			normal_block_num = 1;
		}

		// 横方向.
		// 同じ色が並んでいる範囲を調べる.

		int		rx = start.i_pos.x;
		int		lx = start.i_pos.x;

		// start と同じ色のブロックなら左へ進む.
		for(int x = lx - 1;x > 0;x--) {

			BlockControl	next_block = this.blocks[x, start.i_pos.y];

			if(next_block.color != start.color) {

				break;
			}
			if(next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
	
				break;
			}
			if(next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
	
				break;
			}
			if(!next_block.isVanishing()) {
	
				normal_block_num++;
			}
			lx = x;
		}
		// start と同じ色のブロックなら右へ進む.
		for(int x = rx + 1;x < Block.BLOCK_NUM_X;x++) {

			BlockControl	next_block = this.blocks[x, start.i_pos.y];

			if(next_block.color != start.color) {

				break;
			}
			if(next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
	
				break;
			}
			if(next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
	
				break;
			}
			if(!next_block.isVanishing()) {
	
				normal_block_num++;
			}
			rx = x;
		}

		// rx から lx までのブロックが同じ色.
		// 
		do {

			// ３こ以下なら消えない.
			if(rx - lx + 1 < 3) {

				break;
			}
			// 『消え演出中じゃない』ブロックがいっこもなかったら、消えない.
			if(normal_block_num == 0) {

				break;
			}

			for(int x = lx;x < rx + 1;x++) {

				if(this.blocks[x, start.i_pos.y] == this.grabbed_block) {

					this.hideArrow();
					this.grabbed_block.endGrab();
					this.grabbed_block = null;
				}

				// 着火演出はじめ.
				this.blocks[x, start.i_pos.y].toVanishing();

				// 『隣りのブロック』をつないでおく.
				if(x > lx) {

					this.connect_x(x - 1, x, start.i_pos.y);
				}
				if(x < rx) {

					this.connect_x(x, x + 1, start.i_pos.y);
				}
			}

			ret = true;

		} while(false);

		// ---------------------------------------------------------------- //
		// 縦方向.

		normal_block_num = 0;

		if(!start.isVanishing()) {

			normal_block_num = 1;
		}


		int		uy = start.i_pos.y;
		int		dy = start.i_pos.y;

		for(int y = dy - 1;y > 0;y--) {

			BlockControl	next_block = this.blocks[start.i_pos.x, y];

			if(next_block.color != start.color) {

				break;
			}
			if(next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
	
				break;
			}
			if(next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
	
				break;
			}
			if(!next_block.isVanishing()) {
	
				normal_block_num++;
			}
			dy = y;
		}
		for(int y = uy + 1;y < Block.BLOCK_NUM_Y;y++) {

			BlockControl	next_block = this.blocks[start.i_pos.x, y];

			if(next_block.color != start.color) {

				break;
			}
			if(next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) {
	
				break;
			}
			if(next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) {
	
				break;
			}
			if(!next_block.isVanishing()) {
	
				normal_block_num++;
			}
			uy = y;
		}

		do {

			if(uy - dy + 1 < 3) {

				break;
			}
			if(normal_block_num == 0) {

				break;
			}

			for(int y = dy;y < uy + 1;y++) {
	
				if(this.blocks[start.i_pos.x, y] == this.grabbed_block) {

					this.hideArrow();
					this.grabbed_block.endGrab();
					this.grabbed_block = null;
				}

				this.blocks[start.i_pos.x, y].toVanishing();

				// 『隣りのブロック』をつないでおく.
				if(y > dy) {

					this.connect_y(y - 1, y, start.i_pos.x);
				}
				if(y < uy) {

					this.connect_y(y, y + 1, start.i_pos.x);
				}
			}

			ret = true;

		} while(false);

		// ---------------------------------------------------------------- //

		return(ret);
	}

	// 横に並んだブロックをリンクする（同じいろ）.
	private void		connect_x(int lx, int rx, int y)
	{
		this.blocks[rx, y].setConnectedBlock(Block.DIR4.LEFT,  new Block.iPosition(lx, y));
		this.blocks[lx, y].setConnectedBlock(Block.DIR4.RIGHT, new Block.iPosition(rx, y));
	}

	// たてに並んだブロックをリンクする（同じいろ）.
	private void		connect_y(int uy, int dy, int x)
	{
		this.blocks[x, uy].setConnectedBlock(Block.DIR4.DOWN, new Block.iPosition(x, dy));
		this.blocks[x, dy].setConnectedBlock(Block.DIR4.UP,   new Block.iPosition(x, uy));
	}

}

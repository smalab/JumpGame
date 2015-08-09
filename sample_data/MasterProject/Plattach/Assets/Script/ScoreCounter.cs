using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour {

	public struct Count {
		public int		ignite;			// 着火回数.
		public int		block;			// 着火したブロックの総数.
		public int		score;			// スコアー（今回消したブロックの）.
		public int		total_socre;	// トータルスコアー.
		public int		islands;		// しまの数.
		public int		island_size;	// いちばん大きな島のおおきさ（ブロック数）.
	};

	public Count	last;			// 直前の結果.
	public Count	best;			// 今回のプレイのベスト.

	public static int		QUOTA_SCORE = 10000;			// クリアーするためのスコアー.
	private int				repairment_bak = 10000;

	private ScoreDisp		score_disp = null;			// スコアー表示.
	private static float	POS_FIRE_X	= 160.0f;
	private static float	POS_FIRE_Y	= 450.0f;
	private static float	POS_SCORE_X	= 10.0f;
	private static float	POS_SCORE_Y	= 10.0f;


	private float 	disp_pos_fire_y = 450.0f;
	private float 	eff_fire_y = 0.0f;
	private float	disp_pos_score_y = 10.0f;
	private float	eff_score_y = 0.0f;

	private SoundControl sound_control = null;

	private unitoControl unito_control = null;

	// ばんどえいど----.
	public GameObject[]		bansoco;
	
	// ロケットモーション---.
	private Animation 	rocket_motion;		// (rocket)motion--.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.last.ignite      = 0;
		this.last.block       = 0;
		this.last.score       = 0;
		this.last.total_socre = 0;
		this.last.islands     = 0;
		this.last.island_size = 0;

		this.best = this.last;

		this.score_disp = GameObject.FindGameObjectWithTag("Score Disp").GetComponent<ScoreDisp>();
		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
		this.unito_control = GameObject.FindGameObjectWithTag("Player").GetComponent<unitoControl>();

		this.rocket_motion = GameObject.Find("rocket_model").gameObject.GetComponentInChildren<Animation>();		//motion

	}
	
	void	Update()
	{
	}

	void	OnGUI()
	{
		// chain----------.
		if(this.last.ignite >1){
			this.score_disp.dispNumber(new Vector2(POS_FIRE_X , disp_pos_fire_y), this.last.ignite, 96.0f,2);
			disp_pos_fire_y += eff_fire_y;
			disp_pos_fire_y = Mathf.Clamp(disp_pos_fire_y, POS_FIRE_Y-64.0f, POS_FIRE_Y);
			eff_fire_y += 4.0f *Time.deltaTime;
		}

		int disp_score = Mathf.Clamp((QUOTA_SCORE -this.last.total_socre), 0, QUOTA_SCORE);

		// SCORE-----.
		this.score_disp.dispNumber(new Vector2(POS_SCORE_X , disp_pos_score_y), disp_score, 32.0f,5);
		if(disp_score < QUOTA_SCORE/10){
			disp_pos_score_y += eff_score_y;
			eff_score_y += 8.0f *Time.deltaTime;
		}
		disp_pos_score_y = Mathf.Clamp(disp_pos_score_y, POS_SCORE_Y-64.0f, POS_SCORE_Y);




		int	x = 20 +600;
		int	y = 50;
		GUI.color = Color.black;

		GUI.Label(new Rect(x, y, 100, 20), "今回");
		y += 20;
			this.print_value(x + 20, y, "着火ウント", this.last.ignite);
			y += 30;
			this.print_value(x + 20, y, "ブロック",   this.last.block);
			y += 30;
			this.print_value(x + 20, y, "スコアー",   this.last.score);
			y += 30;
			this.print_value(x + 20, y, "トータル",   this.last.total_socre);
			y += 30;
			this.print_value(x + 20, y, "島",         this.last.islands);
			y += 30;
			this.print_value(x + 20, y, "島サイズ",   this.last.island_size);
			y += 30;

		GUI.Label(new Rect(x, y, 100, 20), "最高");
		y += 20;
			this.print_value(x + 20, y, "着火ウント", this.best.ignite);
			y += 30;
			this.print_value(x + 20, y, "ブロック",   this.best.block);
			y += 30;
			this.print_value(x + 20, y, "島",         this.best.islands);
			y += 30;
			this.print_value(x + 20, y, "島サイズ",   this.best.island_size);
			y += 30;
	}

	public void		print_value(int x, int y, string label, int value)
	{
		GUI.Label(new Rect(x, y, 100, 20), label);
		y += 15;

		GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString());
		y += 15;
	}

	// ================================================================ //

	// 着火回数をプラスする.
	public void		addIgniteCount(int count)
	{
		this.last.ignite += count;
		this.best.ignite = Mathf.Max(this.best.ignite, this.last.ignite);
		//this.update_score();

		eff_fire_y = -3.0f;
		eff_score_y = -3.0f;

		this.sound_control.ignitSePlay(this.last.ignite);
		this.unito_control.actionUnito(unitoControl.STEP.REPAIR);

		// 絆創膏が落ちる.
		if(this.repairment_bak <= 2000 && this.last.total_socre > 2000){
			this.bansoco[0].GetComponent<bansocoControl>().getoff();
		}
		if(this.repairment_bak <= 4000 && this.last.total_socre > 4000){
			this.bansoco[1].GetComponent<bansocoControl>().getoff();
		}
		if(this.repairment_bak <= 6000 && this.last.total_socre > 6000){
			this.bansoco[2].GetComponent<bansocoControl>().getoff();
		}
		if(this.repairment_bak <= 8000 && this.last.total_socre > 8000){
			this.bansoco[3].GetComponent<bansocoControl>().getoff();
		}
		if(this.repairment_bak <= 9000 && this.last.total_socre >= 9000){
			this.bansoco[4].GetComponent<bansocoControl>().getoff();
		}

		this.rocket_motion.Play("01_repair");
		repairment_bak = this.last.total_socre;
	}

	// 着火したブロックの数をセットする.
	public void		setIgniteBlockCount(int count)
	{
		this.last.block = count;
		this.best.block = Mathf.Max(this.best.block, this.last.block);
		this.update_score();
	}

	// 島の数をセットする.
	public void		setIslandCount(int count)
	{
		this.last.islands = count;
		this.best.islands = Mathf.Max(this.best.islands, this.last.islands);
	}

	public void		setMaxIslandSize(int size)
	{
		this.last.island_size = size;
		this.best.island_size = Mathf.Max(this.best.island_size, this.last.island_size);
	}

	// 着火回数をクリアーする.
	public void		clearIgniteCount()
	{
		this.last.ignite = 0;
	}

	// 着火回数をゲットする.
	public int		getIgniteCount()
	{
		return(this.last.ignite);
	}
	// トータルスコアーを更新する.
	// 着火が終わったときに、今回の着火でかせいだ点数がまとめて入るように.
	public void		updateTotalScore()
	{
		this.last.total_socre += this.last.score;



	}

	public bool		isGameClear()
	{
		bool	is_clear = false;
		if(this.last.total_socre > QUOTA_SCORE) {
			is_clear = true;
			this.unito_control.clear();
		}
		return(is_clear);
	}

	// ================================================================ //

	// 今回の着火でのスコアーを計算する.
	private void	update_score()
	{
		this.last.score = this.last.ignite*10 +this.last.block*10;
		this.best.score = Mathf.Max(this.best.score, this.last.score);
	}


	// 今回の最高の着火数を返す.
	public int bestIgnit(){
		int r = 0;
		r = this.best.ignite;
		return(r);
	}


}

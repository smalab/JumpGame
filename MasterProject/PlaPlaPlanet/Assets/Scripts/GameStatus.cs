using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

	// 修理をしたときに上昇する修理度.
	public static float		GAIN_REPARIMENT_IRON = 0.15f;		//!< 鉄.
	public static float		GAIN_REPARIMENT_PLANT = 0.05f;		//!< きゅうり.
	
	// 持ち運んでいるときにおなかが減る量（１秒あたり）.
	public static float		CONSUME_SATIETY_IRON  = 0.15f/2.0f;		//!< 鉄.
	public static float		CONSUME_SATIETY_APPLE = 0.1f/2.0f;		//!< リンゴ.
	public static float		CONSUME_SATIETY_PLANT = 0.1f/2.0f;		//!< 植物.
	public static float		CONSUME_SATIETY_ALWAYS	= 0.03f;	//!< 生きてるだけ.
	
	// リンゴをたべたときに回復する、おなか.
	public static float		REGAIN_SATIETY_APPLE = 0.6f;
	public static float		REGAIN_SATIETY_PLANT = 0.2f;


	// たき火にくべたときに回復する火の量.
	public static float		REGAIN_FIRE_APPLE	= 0.3f;
	public static float		REGAIN_FIRE_PLANT	= 0.7f;

	// 自動的に減るたき火の火.
	public static float		FIRE_ALWAYS			= 0.05f/2.0f;

	// ゲーーージ.
	public Texture	texture_gauge_sita = null;			// 下地.
	public Texture	texture_gauge_ue   = null;			// 上.
	public Texture	texture_gauge_waku = null;			// わく.

	// ================================================================ //
	public float	repairment = 0.0f;		// 修理度(0.0f ～ 1.0f).
	public float	satiety    = 1.0f;		// 満腹度.
	public float	fire		= 1.0f;		// たき火.

	// 効果音ならし用--.
	public float	past_repairment = 0.0f;		// 修理度(0.0f ～ 1.0f).
	public float	past_satiety    = 1.0f;		// 満腹度.
	public float	past_fire		= 1.0f;		// たき火.



	public float	repairment_bak = 0.0f;

	public GUIStyle guistyle;

	public Texture 	icon_repariment	= null;
	public Texture 	icon_fire		 = null;
	public Texture 	icon_satiety	 = null;


	// ばんどえいど----.
	public GameObject[]		bansoco;

	// ロケットモーション---.
	private Animation 	animation;		// (rocket)motion--.

	// sound-----.
	private SoundControl	sound_control;

	// たき火----.
	private GameObject		fire_object;



	void	Start()
	{
		this.guistyle.fontSize = 32;
		this.animation = GameObject.Find("rocket").transform.FindChild("rocket_model").gameObject.GetComponentInChildren<Animation>();		//motion

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();

		this.fire_object = GameObject.Find("Fire").gameObject;
	}
	
	void	Update()
	{
		this.alwaysFire();

		// たき火が小さくなっていく---.
		float fire_size = 0.7f *this.fire;
		fire_size = Mathf.Clamp(fire_size, 0.3f, 0.7f);
		this.fire_object.transform.localScale = Vector3.one *fire_size;

		// 効果音を鳴らす--.
		// たき火が0.3f以下になったら----.
		if((this.past_fire >0.3f)&&(this.fire <=0.3f)){
			this.sound_control.SoundPlay(Sound.SOUND.FIRE_M);
		}
		if((this.past_repairment < 0.7f)&&(this.repairment >=0.7f)){
			this.sound_control.SoundPlay(Sound.SOUND.SHIP_M);
		}
		if((this.past_satiety >0.3f)&&(this.satiety <=0.3f)){
			this.sound_control.SoundPlay(Sound.SOUND.STOMACH);
		}

		this.past_fire		= this.fire;
		this.past_repairment= this.repairment;
		this.past_satiety	= this.satiety;

		// デバッグ操作（Cキーですぐにゲームクリアー）.
		if(Input.GetKeyDown(KeyCode.C)) {
			this.repairment = 1.0f;
		}
	}

	void	OnGUI()
	{
		// icon.
		GUI.DrawTexture(new Rect(80, 8, icon_satiety.width, icon_satiety.height), icon_satiety);
		GUI.DrawTexture(new Rect(260, 8, icon_fire.width, icon_fire.height), icon_fire);
		GUI.DrawTexture(new Rect(440, 8, icon_repariment.width, icon_repariment.height), icon_repariment);
		
		//float	y = 20.0f;
		//GUI.Label(new Rect(80+48, y, 200.0f, 20.0f), (this.satiety*100.0f).ToString("000"), guistyle);
		//GUI.Label(new Rect(280+48, y, 200.0f, 20.0f), (this.fire*100.0f).ToString("000"), guistyle);
		//GUI.Label(new Rect(440+48, y, 200.0f, 20.0f), (this.repairment*100.0f).ToString("000"), guistyle);

		// ゲーーーージ.
		this.draw_gauge(120.0f, 24.0f, this.satiety, true);
		this.draw_gauge(300.0f, 24.0f, this.fire, true);
		this.draw_gauge(480.0f, 24.0f, 1.0f -this.repairment, true);
	}


	// ゲーーーーーーーーージを表示する.
	protected void	draw_gauge(float x, float y, float length, bool shake)
	{
		Texture	sita = this.texture_gauge_sita;
		Texture	ue   = this.texture_gauge_ue;
		Texture	waku = this.texture_gauge_waku;

		if(shake && length <= 0.3f){
			x += Random.Range(-2,2);
			y += Random.Range(-2,2);
		}

		GUI.DrawTexture(new Rect(x, y, sita.width,      sita.height), sita);
		GUI.DrawTexture(new Rect(x, y, ue.width*length, ue.height),   ue);
		GUI.DrawTexture(new Rect(x, y, waku.width,      waku.height), waku);
	}


	// ================================================================ //
	
	// 修理度を増減する.
	public void		addRepairment(float add)
	{
		this.repairment_bak = this.repairment;
		this.repairment = Mathf.Clamp01(this.repairment + add);

		// 絆創膏が落ちる.
		if (this.repairment >= 0.2f && this.repairment_bak < 0.2f) {
			this.bansoco[0].GetComponent<bansocoControl>().getoff();
		}
		if (this.repairment >= 0.4f && this.repairment_bak < 0.4f) {
			this.bansoco[1].GetComponent<bansocoControl>().getoff();
		}
		if (this.repairment >= 0.6f && this.repairment_bak < 0.6f) {
			this.bansoco[2].GetComponent<bansocoControl>().getoff();
		}
		if (this.repairment >= 0.8f && this.repairment_bak < 0.8f) {
			this.bansoco[3].GetComponent<bansocoControl>().getoff();
		}
		if (this.repairment >= 1.0f && this.repairment_bak < 1.0f) {
			this.bansoco[4].GetComponent<bansocoControl>().getoff();
		}

		this.animation.Play("01_repair");

	}
	
	// 満腹度を増減する.
	public void		addSatiety(float add)
	{
		this.satiety = Mathf.Clamp01(this.satiety + add);
	}
	
	// 移動しているだけでお腹が減る.
	public void	alwaysSatiety()
	{
		this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS *Time.deltaTime);
	}

	// たき火を増減させる.
	public void addFire(float add)
	{
		this.fire = Mathf.Clamp01(this.fire +add);
	}

	// たき火が減っていく.
	public void	alwaysFire()
	{
		this.fire = Mathf.Clamp01(this.fire - FIRE_ALWAYS *Time.deltaTime);
	}
	

	// クリアした？-------------.
	public bool		isGameClear()
	{
		bool	is_clear = false;
		if(this.repairment >= 1.0f) {
			is_clear = true;
		}
		return(is_clear);
	}
	
	
	// 死んだ？-----.
	public bool		isGameOver()
	{
		bool	is_over = false;
		if(this.satiety <= 0.0f){
			is_over = true;
		}
		if(this.fire <=0.0f){
			is_over = true;
		}
		return(is_over);
	}









}

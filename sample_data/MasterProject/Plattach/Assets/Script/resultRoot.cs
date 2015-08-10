using UnityEngine;
using System.Collections;

public class resultRoot : MonoBehaviour {

	private static float	RESULT_SCORE_POS_X 	= 96.0f;		// リザルト画面　スコアーの表示位置.
	private static float	RESULT_SCORE_POS_Y 	= 180.0f;		// リザルト画面　スコアーの表示位置.
	
	private static float	RESULT_HIGH_SCORE_POS_X = 96.0f;		// リザルト画面　ハイスコアーの表示位置.
	private static float	RESULT_HIGH_SCORE_POS_Y	= 270.0f;		// リザルト画面　ハイスコアーの表示位置.
	
	private static float	RESULT_HIGH_COIN_POS_X = 96.0f;		// リザルト画面　ハイスコアーのコインの表示位置.
	private static float	RESULT_HIGH_COIN_POS_Y = 360.0f;		// リザルト画面　ハイスコアーのコインの表示位置.
	
	// ---------------------------------------------------------------- //
	public Texture	result_texture = null;
	public Texture	next_texture   = null;
	private ScoreDisp		score_disp = null;		// スコアー表示.
	
	// ---------------------------------------------------------------- //
	private GlobalParam.Score		high_score;				// ハイスコア―.
	private GlobalParam.Score		last_socre;				// 今回のスコアー.	
	// ---------------------------------------------------------------- //

	private SoundControl sound_control = null;


	public enum STEP {
		
		NONE = -1,
		
		RESULT = 0,			// リザルト.
		RESULT_ACTION,		// リザルトでクリックされたあとの演出.
		TITLE,				// タイトル画面へ.
		
		NUM,
	};
	
	public STEP			step      = STEP.NONE;
	public STEP			next_step = STEP.NONE;
	public float		step_timer = 0.0f;
	
	static private	float	ACTION_TIME = 1.0f;

	private float		disp_score = 0.0f;
	
	// ================================================================ //
	// MonoBehaviour からの継承.
	
	void	Start()
	{
		this.score_disp = GameObject.FindGameObjectWithTag("Score Disp").GetComponent<ScoreDisp>();
		this.high_score = GlobalParam.getInstance().getHighScore();
		this.last_socre = GlobalParam.getInstance().getLastScore();
		this.next_step = STEP.RESULT;

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
	}
	
	void	Update()
	{
		
		// -------------------------------------------------------------------- //
		// ステップ内の経過時間を進める.
		this.step_timer += Time.deltaTime;
		// -------------------------------------------------------------------- //
		// 次の状態に移るかどうかを、チェックする.
		if(this.next_step == STEP.NONE) {
			switch(this.step) {
			case STEP.RESULT:
				if(Input.GetMouseButtonDown(0)) {
					this.next_step = STEP.RESULT_ACTION;
				}
				break;
			case STEP.RESULT_ACTION:
				if(this.step_timer > ACTION_TIME) {
					this.next_step = STEP.TITLE;
				}
				break;
			}
		}
		
		// -------------------------------------------------------------------- //
		// 状態が遷移したときの初期化.
		
		while(this.next_step != STEP.NONE) {
			this.step      = this.next_step;
			this.next_step = STEP.NONE;
			switch(this.step) {
			case STEP.RESULT:
				this.sound_control.playBgm(Sound.BGM.BGM2);
				break;
			case STEP.RESULT_ACTION:
				this.sound_control.playSound(Sound.SOUND.CLICK);
				disp_score = this.last_socre.score;
				break;
			case STEP.TITLE:
				this.sound_control.stopBgm();
				Application.LoadLevel("TitleScene");
				break;
			}
			
			this.step_timer = 0.0f;
		}
		
		// -------------------------------------------------------------------- //
		// 各状態での実行処理.
		
		switch(this.step) {
		case STEP.RESULT:



			break;
		}
	}
	
	void	OnGUI()
	{
		Rect	rect = new Rect();
		// -------------------------------------------------------------------- //
		// 背景.
		Texture		back_texture;
		switch(this.step) {
		default:
		{
			back_texture = this.result_texture;
		}
			break;
		}
		
		rect.x = 0.0f;
		rect.y = 0.0f;
		rect.width  = back_texture.width/2;
		rect.height = back_texture.height/2;

		GUI.DrawTexture(rect, back_texture);
		
		// -------------------------------------------------------------------- //
		// ネクストボタン.
		
		float	scale = 1.0f;
		
		if(this.step == STEP.RESULT_ACTION) {
			
			// クリックされると一瞬でかくなる（適当）.
			
			scale = this.step_timer/(ACTION_TIME/4.0f);
			scale = Mathf.Min(scale, 1.0f);
			scale = Mathf.Sin(scale*Mathf.PI);
			
			scale = Mathf.Lerp(1.0f, 1.2f, scale);
		}
		
		rect.width  = this.next_texture.width*scale;
		rect.height = this.next_texture.height*scale;

		rect.x = Screen.width*0.8f - rect.width/2.0f;
		rect.y = Screen.height*0.9f - rect.height/2.0f;
		
		GUI.DrawTexture(rect, this.next_texture);
		
		// -------------------------------------------------------------------- //
		// スコアーなど.
		
		switch(this.step) {
			
		case STEP.RESULT:
		case STEP.RESULT_ACTION:
		case STEP.TITLE:
			if(disp_score < this.last_socre.score){
				disp_score += Time.deltaTime*30;
				this.sound_control.playSound(Sound.SOUND.GRAB);
			}
			disp_score = Mathf.Clamp(disp_score, 0.0f, this.last_socre.score);

			/*
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X, RESULT_SCORE_POS_Y), (int)this.last_socre.score, 64.0f, 2);
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X+92, RESULT_SCORE_POS_Y+32), (int)(this.last_socre.score*100)/10%10, 32.0f, 1);
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X+112, RESULT_SCORE_POS_Y+32), (int)(this.last_socre.score*100)/1%10, 32.0f, 1);
			*/
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X, RESULT_SCORE_POS_Y), (int)disp_score, 64.0f, 2);
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X+92, RESULT_SCORE_POS_Y+32), (int)(disp_score*100)/10%10, 32.0f, 1);
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X+112, RESULT_SCORE_POS_Y+32), (int)(disp_score*100)/1%10, 32.0f, 1);


			this.score_disp.dispNumber(new Vector2(RESULT_HIGH_SCORE_POS_X, RESULT_HIGH_SCORE_POS_Y), (int)this.high_score.score,64.0f, 2);
			this.score_disp.dispNumber(new Vector2(RESULT_HIGH_SCORE_POS_X+92, RESULT_HIGH_SCORE_POS_Y+32), (int)(this.high_score.score*100)/10%10, 32.0f, 1);
			this.score_disp.dispNumber(new Vector2(RESULT_HIGH_SCORE_POS_X+112, RESULT_HIGH_SCORE_POS_Y+32), (int)(this.high_score.score*100)/1%10, 32.0f, 1);

			this.score_disp.dispNumber(new Vector2(RESULT_HIGH_COIN_POS_X, RESULT_HIGH_COIN_POS_Y), this.high_score.ignit,64.0f, 2);

			break;
		}
	}
}

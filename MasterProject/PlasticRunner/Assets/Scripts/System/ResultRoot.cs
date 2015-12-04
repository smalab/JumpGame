using UnityEngine;
using System.Collections;

// タイトル画面のシーケンス.
public class ResultRoot : MonoBehaviour {

	private static float	RESULT_SCORE_POS_X 	= 640.0f/2.0f -64.0f;		// リザルト画面　スコアーの表示位置.
	private static float	RESULT_SCORE_POS_Y 	= 120.0f;					// リザルト画面　スコアーの表示位置.

	private static float	RESULT_HIGH_SCORE_POS_X = 640.0f/2.0f -64.0f;	// リザルト画面　ハイスコアーの表示位置.
	private static float	RESULT_HIGH_SCORE_POS_Y	= 240.0f;				// リザルト画面　ハイスコアーの表示位置.

	private static float	RESULT_HIGH_COIN_POS_X = 640.0f/2.0f -64.0f;	// リザルト画面　ハイスコアーのコインの表示位置.
	private static float	RESULT_HIGH_COIN_POS_Y = 340.0f;				// リザルト画面　ハイスコアーのコインの表示位置.

	// ---------------------------------------------------------------- //

	public Texture	result_texture = null;
	public Texture	next_texture   = null;

	private ScoreDisp		score_disp = null;		// スコアー表示.

	// ---------------------------------------------------------------- //

	private ScoreControl.Score		high_score;				// ハイスコア―.
	private ScoreControl.Score		last_socre;				// 今回のスコアー.

	private int 	disp_last_score = 0;	// 表示用.

	// ---------------------------------------------------------------- //

	private SoundControl sound_control = null;


	public enum STEP {

		NONE = -1,

		RESULT = 0,				// リザルト.
		RESULT_ACTION,		// リザルトでクリックされたあとの演出.
		TITLE,				// タイトル画面へ.

		NUM,
	};

	public STEP			step      = STEP.NONE;
	public STEP			next_step = STEP.NONE;
	public float		step_timer = 0.0f;

	static private	float	ACTION_TIME = 1.0f;

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
				{
					if(Input.GetMouseButtonDown(0)) {

						this.next_step = STEP.RESULT_ACTION;
						this.sound_control.playSound(Sound.SOUND.CLICK);

					}
				}
				break;

				case STEP.RESULT_ACTION:
				{
					if(this.step_timer > ACTION_TIME) {

						this.next_step = STEP.TITLE;
					}
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
				{
					// ハイスコア―を保存.
					GlobalParam.getInstance().saveSaveData();

					this.sound_control.playBgm(Sound.BGM.RESULT);
				}
				break;

				case STEP.RESULT_ACTION:
				{
					this.sound_control.stopBgm();
					this.disp_last_score = this.last_socre.score;
				}
				break;



				case STEP.TITLE:
				{
					Application.LoadLevel("TitleScene");
				}
				break;
			}

			this.step_timer = 0.0f;
		}

		// -------------------------------------------------------------------- //
		// 各状態での実行処理.

		switch(this.step) {

			case STEP.RESULT:
			this.disp_last_score += (int)(100 *Time.deltaTime);
			this.disp_last_score = Mathf.Clamp(this.disp_last_score, 0, this.last_socre.score);
			if(this.disp_last_score < this.last_socre.score) {
				this.sound_control.playSound(Sound.SOUND.COIN_GET);
			}
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
		rect.width  = back_texture.width;
		rect.height = back_texture.height;

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

		rect.x = Screen.width*0.9f - rect.width/2.0f;
		rect.y = Screen.height*0.9f - rect.height/2.0f;

		GUI.DrawTexture(rect, this.next_texture);

		// -------------------------------------------------------------------- //
		// スコアー、コイン数など.

		switch(this.step) {
			case STEP.RESULT:
			case STEP.RESULT_ACTION:
			case STEP.TITLE:
			{
			this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X, RESULT_SCORE_POS_Y), this.disp_last_score);// this.last_socre.score);
				this.score_disp.dispNumber(new Vector2(RESULT_HIGH_SCORE_POS_X, RESULT_HIGH_SCORE_POS_Y), this.high_score.score);
				this.score_disp.dispNumber(new Vector2(RESULT_HIGH_COIN_POS_X, RESULT_HIGH_COIN_POS_Y), this.high_score.coins);
			}
			break;
		}
	}
}

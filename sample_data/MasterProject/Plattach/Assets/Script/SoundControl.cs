using UnityEngine;
using System.Collections;

public class Sound{
	public enum SOUND{
		NON = -1,

		GRAB = 0,

		SLIDE,
		CLEAR,
		CLICK,
		RESTORE,
		IGNIT1,
		IGNIT2,
		IGNIT3,
		IGNIT4,
		IGNIT5,
		IGNIT6,
		IGNIT7,
		IGNIT8,
		IGNIT9,
		IGNIT10,
		GAME_CLEAR,
		NUM,
	}

	public enum BGM{
		NON = -1,
		BGM1 = 0,
		BGM2,
	}


}

public class SoundControl : MonoBehaviour {

	public AudioClip[]	audioclip;
	public AudioSource[] audiosource;

	public AudioClip[]	bgmclip;
	public AudioSource[] bgmsource;

	private	Sound.BGM	current_bgm = Sound.BGM.NON;			// 再生中の BMG.

	void Start () {
		this.audiosource = new AudioSource[this.audioclip.Length];
		for(int i=0; i<this.audiosource.Length; i++){
			this.audiosource[i]   = this.gameObject.AddComponent<AudioSource>();
			this.audiosource[i].clip = this.audioclip[i];
		}

		this.bgmsource = new AudioSource[this.bgmclip.Length];
		for(int i=0; i<this.bgmsource.Length; i++){
			this.bgmsource[i]   = this.gameObject.AddComponent<AudioSource>();
			this.bgmsource[i].clip = this.bgmclip[i];
			this.bgmsource[i].loop = true;
		}
	}
	

	// 着火音-------------.
	public void ignitSePlay(int count){
		switch(count){
		case 1: this.audiosource[ (int)Sound.SOUND.IGNIT1].Play (); break;
		case 2:	this.audiosource[ (int)Sound.SOUND.IGNIT2].Play (); break;
		case 3: this.audiosource[ (int)Sound.SOUND.IGNIT3].Play (); break;
		case 4: this.audiosource[ (int)Sound.SOUND.IGNIT4].Play (); break;
		case 5: this.audiosource[ (int)Sound.SOUND.IGNIT5].Play (); break;
		case 6: this.audiosource[ (int)Sound.SOUND.IGNIT6].Play (); break;
		case 7: this.audiosource[ (int)Sound.SOUND.IGNIT7].Play (); break;
		case 8: this.audiosource[ (int)Sound.SOUND.IGNIT8].Play (); break;
		default: this.audiosource[ (int)Sound.SOUND.IGNIT9].Play (); break;
		}
	}

	// 指定されたseを鳴らす.
	public void playSound(Sound.SOUND s){
		this.audiosource[(int)s].Play ();
	}

	// BGM の再生を開始する.
	public void playBgm(Sound.BGM b){
		this.current_bgm = b;
		this.bgmsource[ (int)this.current_bgm].Play();
	}

	// 再生中の BGM を止める.
	public void stopBgm(){
		this.bgmsource[ (int)this.current_bgm].Stop();
	}

	// BGM のループフラグをセットする.
	public void		setBgmLoopPlay(Sound.BGM bgm, bool is_loop_play)
	{
		this.bgmsource[(int)bgm].loop = is_loop_play;
	}

	// BGM の再生時刻をゲットする.
	public float	getBgmPlayingTime()
	{
		float	time = 0.0f;

		if(this.current_bgm != Sound.BGM.NON) {

			time = this.bgmsource[(int)this.current_bgm].time;
		}

		return(time);
	}

	// BGM の再生時刻をセットする.
	public void		setBgmPlayingTime(float time)
	{
		if(this.current_bgm != Sound.BGM.NON) {

			this.bgmsource[(int)this.current_bgm].time = time;
		}
	}

	// BGM のトータル時間をゲットする.
	public float	getBgmTotalTime(Sound.BGM b = Sound.BGM.NON)
	{
		if(b == Sound.BGM.NON) {

			b = this.current_bgm;
		}

		float	time = 0.0f;

		if(b != Sound.BGM.NON) {

			time = this.bgmsource[(int)b].clip.length;
		}

		return(time);
	}

	







}

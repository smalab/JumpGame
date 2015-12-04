using UnityEngine;
using System.Collections;

public class KuchiPakuRoot : MonoBehaviour {

	private SoundControl	sound_control = null;

	private bool	is_bgm_playing = false;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
	}

	void	Update()
	{
		if(!this.is_bgm_playing) {

			if(Input.GetMouseButtonDown(0)) {

				this.sound_control.setBgmLoopPlay(Sound.BGM.PLAY, false);
				this.sound_control.playBgm(Sound.BGM.PLAY);

				this.is_bgm_playing = true;
			}
		}

		this.seek_slider.is_button_down = Input.GetMouseButton(0);
	}

	void	OnGUI()
	{
		this.seek_slider_control();
	}

	// シークスライダー
	private struct SeekSlider {

		public bool		is_now_dragging;		// ドラッグ中？.
		public float	dragging_poisition;		// ドラッグ位置.
		public bool		is_button_down;			// マウスの左ボタン.Input.GetMouseButton(0) の結果
												// ドキュメントに
												// Note also that the Input flags are not reset until "Update()", 
												// so its suggested you make all the Input Calls in the Update Loop
												// とあるので、念のため（実際には大丈夫っぽい？）.
	};
	private SeekSlider	seek_slider;

	// シークスライダーの制御.
	private void	seek_slider_control()
	{
		Rect	slider_rect = new Rect(500, 100, 130, 40 );

		float	current_time = this.sound_control.getBgmPlayingTime();
		float	total_time   = this.sound_control.getBgmTotalTime();

		if(!seek_slider.is_now_dragging) {

			float	new_position = GUI.HorizontalSlider(slider_rect, current_time, 0, total_time);

			// ドラッグ開始.
			if(new_position != current_time) {

				seek_slider.dragging_poisition = new_position;
				seek_slider.is_now_dragging = true;
			}

		} else {

			seek_slider.dragging_poisition = GUI.HorizontalSlider(slider_rect, seek_slider.dragging_poisition, 0, total_time);

			// ボタンが離された（ドラッグ終了）.
			if(!seek_slider.is_button_down) {

				this.sound_control.setBgmPlayingTime(seek_slider.dragging_poisition);

				// ドラッグ終了.
				seek_slider.is_now_dragging = false;
			}

			current_time = seek_slider.dragging_poisition;
		}

		GUI.Label(new Rect(500, 120, 130,40), current_time.ToString());
	}
}

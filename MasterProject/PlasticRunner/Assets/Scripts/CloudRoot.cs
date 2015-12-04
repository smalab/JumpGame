using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ.
public class KuchiPakuData {
	
	public KuchiPakuData()
	{
		this.line_number = -1;
		this.time        = 0.0f;
		this.mouth_type  = CloudControl.MOUTH_TYPE.CLOSE;
	}

	public int						line_number;	// 繝?く繧ｹ繝医ヵ繧｡繧､繝ｫ縺ｮ陦檎分蜿ｷ.
	public float					time;			// [sec] 譎る俣.
	public CloudControl.MOUTH_TYPE	mouth_type;		// 縺上■縺ｮ蠖｢迥ｶ.
};

public class CloudRoot : MonoBehaviour {

	private List<KuchiPakuData>		kuchi_paku_datas = null;		// 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ.
	public TextAsset				kuchi_paku_text  = null;		// 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ縺ｮ繝?く繧ｹ繝?

	private SoundControl	sound_control = null;
	private CameraControl	main_camera   = null;
	private PlayerControl	player        = null;

	public GameObject			kumo01Prefab = null;
	public GameObject			kumo02Prefab = null;

	private class Cloud {

		public GameObject	cloud;				// 縺上ｂ縺ｮ繧ｲ繝ｼ繝?繧ｪ繝悶ず繧ｧ繧ｯ繝?
		public float		speed;				// 繧ｹ繝斐?繝峨?豈皮紫.
	};
	private List<Cloud>		clouds;

	private static float	DEPTH      = 15.0f;						// 螂･陦後″.
	private static float	CLIP_RANGE = 16.0f;						// 逕ｻ髱｢縺ｮ蟾ｦ蜿ｳ縺ｮ螟ｧ縺阪＆.
	private static float	Y_MAX =  6.0f;							// 鬮倥＆縲?譛?螟ｧ.
	private static float	Y_MIN = -6.0f;							// 鬮倥＆縲?譛?蟆?

	// ---------------------------------------------------------------- //

	public bool		is_kuchi_paku_scene = false;			// true ... 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ菴懈?逕ｨ繧ｷ繝ｼ繝ｳ false ... 騾壼ｸｸ繧ｲ繝ｼ繝?.
	public bool		is_recording_mode   = false;			// true ... 繝??繧ｿ繝ｼ菴懈?繝｢繝ｼ繝?        false ... 繝??繧ｿ繝ｼ蜀咲函繝｢繝ｼ繝?

	private CloudControl.MOUTH_TYPE	mouth_type = CloudControl.MOUTH_TYPE.NONE;

	private string kuchi_paku_record = "";

	// ================================================================ //
	// MonoBehaviour 縺九ｉ縺ｮ邯呎価.

	void	Start()
	{
		this.kuchi_paku_datas = new List<KuchiPakuData>();

		this.loadKuchiPakuData(this.kuchi_paku_text);

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
		this.main_camera   = GameObject.Find("Main Camera").GetComponent<CameraControl>();
		this.player        = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

		this.clouds = new List<Cloud>();

		for(int i = 0;i < 4;i++) {

			GameObject		go;

			if(i%2 == 0) {

				go = GameObject.Instantiate(this.kumo01Prefab) as GameObject;

			} else {

				go = GameObject.Instantiate(this.kumo02Prefab) as GameObject;
			}

			Cloud			cloud = new Cloud();

			this.clouds.Add(cloud);

			cloud.cloud = go;

			// 螟夐㍾繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ縺｣縺ｽ縺上∩縺帙ｋ縺溘ａ縺ｫ縲∝･･縺ｫ陦後￥縺ｻ縺ｩ繧ｹ繝斐?繝峨ｒ驕?￥縺吶ｋ.
			cloud.speed = 0.5f*Mathf.Pow(0.7f, i);

			cloud.cloud.transform.parent        = this.main_camera.transform;
			cloud.cloud.transform.localPosition = new Vector3(CLIP_RANGE*(i*1.0f + 0.0f), 1.0f + i*2.0f, DEPTH + i*1.0f);

			float		s = Mathf.Lerp(1.0f, 0.5f, i/(3.0f - 1.0f));

			Vector3		scale = cloud.cloud.transform.localScale;

			scale.x *= s;
			scale.y *= s;

			cloud.cloud.transform.localScale = scale;
		}
	}

	void	Update()
	{
		// 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ菴懈?繝｢繝ｼ繝?
		if(this.is_kuchi_paku_scene) {

			if(this.is_recording_mode) {
	
				this.update_recording_mode();

			} else {

			}

		} else {

			foreach(Cloud cloud in this.clouds) {

				// 逕ｻ髱｢蟾ｦ遶ｯ縺ｫ豸医∴縺溘ｉ縲∝燕譁ｹ?亥承遶ｯ?峨↓繝ｯ繝ｼ繝?
				if(cloud.cloud.transform.localPosition.x < -CLIP_RANGE) {

					Vector3		p = cloud.cloud.transform.localPosition;

					p.x = CLIP_RANGE;
					p.y = Mathf.InverseLerp(Y_MIN, Y_MAX, p.y);
					p.y += Random.Range(0.25f, 0.75f);
					p.y = Mathf.Repeat(p.y, 1.0f);
					p.y = Mathf.Lerp(Y_MIN, Y_MAX, p.y);

					cloud.cloud.transform.localPosition = p;
				}
				cloud.cloud.transform.Translate(-Vector3.right*cloud.speed*this.player.GetComponent<Rigidbody>().velocity.x*Time.deltaTime);
			}
		}
	}
	
	void	OnGUI()
	{
		if(this.is_kuchi_paku_scene) {

			this.on_gui_recording();
		}
	}

	// ================================================================ //

	// 蜿｣縺ｱ縺上ち繧､繝励ｒ繧ｲ繝?ヨ縺吶ｋ.
	public CloudControl.MOUTH_TYPE	getMouthType()
	{
		CloudControl.MOUTH_TYPE		mouth_type;

		if(this.is_recording_mode) {

			mouth_type = this.get_mouth_type_record();

		} else {

			mouth_type = this.get_mouth_type_play();
		}

		return(mouth_type);
	}


	// 蜿｣縺ｱ縺上ち繧､繝励ｒ繧ｲ繝?ヨ縺吶ｋ縲?繧ｲ繝ｼ繝?荳ｭ逕ｨ.
	private CloudControl.MOUTH_TYPE	get_mouth_type_play()
	{
		// BGM 縺ｮ蜀咲函譎ょ綾繧偵ご繝?ヨ縺吶ｋ.
		float	time = this.sound_control.getBgmPlayingTime();

		CloudControl.MOUTH_TYPE	mouth_type = CloudControl.MOUTH_TYPE.CLOSE;

		do {

			int		index = this.get_kuchi_paku_data_index(time);

			if(index < 0) {

				break;
			}

			mouth_type = this.kuchi_paku_datas[index].mouth_type;

		} while(false);

		return(mouth_type);
	}

	// 迴ｾ蝨ｨ譎ょ綾縺ｮ蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ縺ｮ繧､繝ｳ繝?ャ繧ｯ繧ｹ繧偵ご繝?ヨ縺吶ｋ.
	private int		get_kuchi_paku_data_index(float time)
	{
		int		index = -1;

		do {

			if(this.kuchi_paku_datas.Count == 0) {

				break;
			}

			index = this.kuchi_paku_datas.Count - 1;

			for(int i = 0;i < this.kuchi_paku_datas.Count;i++) {
	
				if(time < this.kuchi_paku_datas[i].time) {

					index = i - 1;
					break;
				}
			}

		} while(false);

		return(index);
	}

	// 蜿｣縺ｱ縺上ち繧､繝励ｒ繧ｲ繝?ヨ縺吶ｋ縲?蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ菴懈?繝｢繝ｼ繝臥畑.
	private CloudControl.MOUTH_TYPE	get_mouth_type_record()
	{
		return(this.mouth_type);
	}

	// ================================================================ //

	// 豈弱ヵ繝ｬ繝ｼ繝?縺ｮ螳溯｡後??蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ菴懈?繝｢繝ｼ繝臥畑.
	private void	update_recording_mode()
	{
		CloudControl.MOUTH_TYPE	mouth_type = CloudControl.MOUTH_TYPE.CLOSE;

		if(Input.GetMouseButton(0)) {

			mouth_type = CloudControl.MOUTH_TYPE.HALF;
		}
		if(Input.GetMouseButton(1)) {

			mouth_type = CloudControl.MOUTH_TYPE.FULL;
		}

		if(mouth_type != this.mouth_type) {

			this.mouth_type = mouth_type;

			KuchiPakuData	data = new KuchiPakuData();

			data.time       = this.sound_control.getBgmPlayingTime();
			data.mouth_type = this.mouth_type;

			this.kuchi_paku_datas.Add(data);

			this.kuchi_paku_record += data.time.ToString();
			this.kuchi_paku_record += "\t";
			this.kuchi_paku_record += data.mouth_type.ToString().ToLower();
			this.kuchi_paku_record += "\n";
		}
	}

	// 蜿｣縺ｱ縺上ョ繝ｼ繧ｿ繝ｼ菴懈?繧ｷ繝ｼ繝ｳ逕ｨ GUI
	private	void	on_gui_recording()
	{
		if(this.is_recording_mode) {

			GUI.TextField(new Rect(10, 10, 400, 400), this.kuchi_paku_record);

		} else {


			// BGM 縺ｮ蜀咲函譎ょ綾繧偵ご繝?ヨ縺吶ｋ.
			float	time = this.sound_control.getBgmPlayingTime();
	
			do {
	
				int		index = this.get_kuchi_paku_data_index(time);
	
				if(index < 0) {
	
					break;
				}
	
				string			line_text = "";

				int		st, ed;
				int		line_count = 12;

				if(this.kuchi_paku_datas.Count < line_count) {

					st = 0;
					ed = line_count - 1;

				} else {

					if(index < line_count - 1) {

						st = 0;
						ed = line_count - 1;

					} else {

						ed = index + 1;
	
						if(ed >= this.kuchi_paku_datas.Count) {
	
							ed = this.kuchi_paku_datas.Count - 1;
						}
	
						st = ed - (line_count - 1);
					}
				} 

				for(int i = st;i <= ed;i++) {

					KuchiPakuData	data = this.kuchi_paku_datas[i];

					if(i == index) {

						line_text += ">\t";

					} else {

						line_text += "\t";
					}
					line_text += 	  (data.line_number + 1).ToString("d3") + "\t\t" 
									+ data.time.ToString("0.00") + "\t\t" 
									+ data.mouth_type.ToString().ToLower() + "\n";
				}

				GUI.TextField(new Rect(10, 10, 400, 200), line_text);

			} while(false);
		}
	}

	// 繝ｬ繝吶Ν繝??繧ｿ繝ｼ繧偵ユ繧ｭ繧ｹ繝医ヵ繧｡繧､繝ｫ縺九ｉ繧医?.
	public void		loadKuchiPakuData(TextAsset kuchi_paku_text)
	{
		// 繝?く繧ｹ繝亥?菴薙ｒ縺ｲ縺ｨ縺､縺ｮ譁?ｭ怜?縺ｫ.
		string		all_texts = kuchi_paku_text.text;

		// 謾ｹ陦後さ繝ｼ繝峨〒蛹ｺ蛻?ｋ縺薙→縺ｧ縲?		// 繝?く繧ｹ繝亥?菴薙ｒ荳?陦悟腰菴阪?驟榊?縺ｫ縺吶ｋ.
		string[]	lines = all_texts.Split('\n');

		int			line_number = -1;

		foreach(var line in lines) {

			line_number++;

			if(line == "") {

				continue;
			}

			// 遨ｺ逋ｽ縺ｧ蛹ｺ蛻?▲縺ｦ縲∝腰隱槭?驟榊?縺ｫ縺吶ｋ.
			string[]	words = line.Split();

			int				n = 0;
			KuchiPakuData	kuchi_paku_data = new KuchiPakuData();

			kuchi_paku_data.line_number = line_number;

			foreach(var word in words) {

				// "#" 莉･髯阪?繧ｳ繝｡繝ｳ繝医↑縺ｮ縺ｧ縺昴ｌ莉･髯阪?繧ｹ繧ｭ繝??.
				if(word.StartsWith("#")) {

					break;
				}
				if(word == "") {

					continue;
				}

				switch(n) {

					case 0:		kuchi_paku_data.time       = float.Parse(word);			break;
					case 1:		kuchi_paku_data.mouth_type = this.toMouthType(word);	break;
				}

				n++;
			}

			if(n >= 2) {

				this.kuchi_paku_datas.Add(kuchi_paku_data);

			} else {

				if(n == 0) {

					// 蜊倩ｪ槭′縺ｪ縺九▲縺滂ｼ晁｡悟?菴薙′繧ｳ繝｡繝ｳ繝医□縺｣縺?

				} else {

					// 繝代Λ繝｡繝ｼ繧ｿ繝ｼ縺瑚ｶｳ繧翫↑縺?
					Debug.LogError("[KuchiPakuData] Out of parameter.\n");
				}
			}
		}

		if(this.kuchi_paku_datas.Count == 0) {

			// 繝??繧ｿ繝ｼ縺後＞縺｣縺薙ｂ縺ｪ縺九▲縺溘→縺?

			Debug.LogError("[KuchiPakuData] Has no data.\n");

			// 繝?ヵ繧ｩ繝ｫ繝医?繝??繧ｿ繝ｼ繧偵＞縺｣縺楢ｿｽ蜉?縺励※縺翫￥.
			this.kuchi_paku_datas.Add(new KuchiPakuData());
		}
	}

	// 譁?ｭ怜?縲?竊偵??蜿｣縺ｱ縺上ち繧､繝?
	public CloudControl.MOUTH_TYPE	toMouthType(string str)
	{
		CloudControl.MOUTH_TYPE mouth_type = CloudControl.MOUTH_TYPE.CLOSE;

		switch(str.ToLower()) {

			case "half":	mouth_type = CloudControl.MOUTH_TYPE.HALF;	break;
			case "full":	mouth_type = CloudControl.MOUTH_TYPE.FULL;	break;
		}

		return(mouth_type);
	}
}

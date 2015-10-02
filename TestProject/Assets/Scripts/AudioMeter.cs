using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioMeter : MonoBehaviour {
	//操作するイメージ
	public Image MeterImage;

	//メーターの値(0~1)
	public float MeterValue {
		set {
			if(MeterImage.fillAmount < value) {
				MeterImage.fillAmount = value;
			}
		}
	}

	void Awake() {
		var child = transform.GetChild(0);
		if(child.name == "Bar") {
				MeterImage = child.GetComponent<Image>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//メーターが自動で0に近づくようにする
		var newValue = MeterImage.fillAmount - (Time.deltaTime * 2);
		MeterImage.fillAmount = System.Math.Max(0, (float)newValue);
	}
}

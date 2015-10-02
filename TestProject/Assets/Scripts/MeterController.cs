using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MeterController : MonoBehaviour {
	//メーター感度(必要かは不明)
	public float MeterFactor = 1;

	// Update is called once per frame
	void Update () {
		AudioMeter.MeterImage.fillAmount = GetMicInput.loudness / 1;
	}
}

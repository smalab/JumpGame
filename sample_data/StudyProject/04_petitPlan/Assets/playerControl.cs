using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {

	private float power;
	public float POWERPLUS = 300.0f;	// 100.0f;
	void Start() {
	}
	void Update() {
		if(Input.GetMouseButton(0)) { // 左ボタンが押されている間.
			power += POWERPLUS * Time.deltaTime; // 力を溜める.
		}
		if(Input.GetMouseButtonUp(0)) { // 左ボタンが離されたら.
			// 溜めた力をxとyに反映させ、右上にジャンプ！.
			this.rigidbody.AddForce(new Vector3(power, power, 0));
			power = 0.0f; // 力をゼロに.
		}
		if(this.transform.position.y < -5.0f) { // 地面より下(-5.0f)まで落ちたら
			Application.LoadLevel("gameScene"); // シーンをリロード.
		}
	}
}

using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {
	public float power;
	public float POWERPLUS = 100.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKey(KeyCode.Space) ) {	// 左ボタンが押されている間
			power += POWERPLUS * Time.deltaTime;	// 力を溜める
			Debug.Log("Pressed space key.");
		}
		
		if( Input.GetKeyUp(KeyCode.Space) ) {	// 左ボタンが離されたら
			// 溜めた力をxとyに反映させ、右上にジャンプする
			this.GetComponent<Rigidbody>().AddForce( new Vector3(power, power, 0) );
			power = 0.0f;
		}
		
		if(this.transform.position.y < -5.0f) {	// 地面より下(-5.0f)まで落ちたら
			Application.LoadLevel("gameScene");	// シーンをリロード
		}
	}
	
}


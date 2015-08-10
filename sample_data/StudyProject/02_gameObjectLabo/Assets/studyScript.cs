using UnityEngine;
using System.Collections;

public class studyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A)) {
			// 0.0〜0.5の間の乱数を作り出す.
			float rnd = Random.Range(0.0f, 5.0f);
			// 自分（Capsule）の位置を変更.
			this.transform.position = new Vector3(0.0f, 0.0f, rnd);
		}

		if(Input.GetKeyDown(KeyCode.B)) {
			float rnd = Random.Range(0.0f, 360.0f);
			// X軸方向の回転具合をランダムに変更.
			this.transform.rotation = Quaternion.Euler(rnd, 0.0f, 0.0f);
		}

		if(Input.GetKeyDown(KeyCode.C)) {
			float rnd = Random.Range(0.5f, 2.0f);
			// サイズをランダムに変更.
			this.transform.localScale = new Vector3(rnd, rnd, rnd);
		}
		if(Input.GetKey(KeyCode.UpArrow)) { // 上キーで、forward（前）.
			// this.transform.Translate(
			// new Vector3(0.0f, 0.0f, 3.0f *Time.deltaTime));
			this.transform.Translate(Vector3.forward * 3.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow)) { // ↓キーで、back（後）.
			this.transform.Translate(Vector3.back * 2.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.LeftArrow)) { // ←キーで、left（左）.
			this.transform.Translate(Vector3.left * 2.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.RightArrow)) { // →キーで、right（右）.
			this.transform.Translate(Vector3.right * 2.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.U)) { // Uキーで、up（上）.
			this.transform.Translate(Vector3.up * 2.0f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.D)) { // Dキーで、down（下）.
			this.transform.Translate(Vector3.down * 2.0f * Time.deltaTime);
		}

		if(Input.GetKey(KeyCode.R)) { // Rキーで、右回転.
			this.transform.Rotate(90.0f * Time.deltaTime, 0.0f, 0.0f);
		}
		if(Input.GetKey(KeyCode.L)) { // Lキーで、左回転.
			this.transform.Rotate(-90.0f * Time.deltaTime, 0.0f, 0.0f);
		}


		if(Input.GetKey(KeyCode.P)) { // Cubeの親を自分（Cupsule）にする.
			GameObject go = GameObject.Find("Cube") as GameObject;
			go.transform.parent = this.transform;
		}
		if(Input.GetKey(KeyCode.N)) { // Cubeの親を解除する.
			GameObject go = GameObject.Find("Cube") as GameObject;
			go.transform.parent = null;
		}

		if(Input.GetKey(KeyCode.G)) {
			GameObject go = GameObject.Find("Cube") as GameObject;
			go.GetComponent<cubeScript>().bigsize();
		}
	}
}

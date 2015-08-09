using UnityEngine;
using System.Collections;

public class RigidBodyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow)) { // ↑キーで奥方向.
			this.transform.rigidbody.AddForce(
				Vector3.forward * 300 * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow)) { // ↓キーで手前方向.
			this.transform.rigidbody.AddForce(
				Vector3.back * 300* Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.LeftArrow)) { // ←キーで左方向.
			this.transform.rigidbody.AddForce(
				Vector3.left * 300 * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.RightArrow)) { // →キーで右方向.
			this.transform.rigidbody.AddForce(
				Vector3.right * 300 * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.U)) { // Uキーで上方向.
			this.transform.rigidbody.AddForce(
				Vector3.up * 300 * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.D)) { // Dキーで下方向.
			this.transform.rigidbody.AddForce(
				Vector3.down * 300 * Time.deltaTime);
		}


		if(Input.GetKeyDown(KeyCode.Keypad0)) { // テンキーの0.
			Physics.gravity = Vector3.zero; // 重力をゼロにする.
		}
		if(Input.GetKeyDown(KeyCode.Keypad8)) { // テンキーの8.
			Physics.gravity = Vector3.up; // 重力を上方向にする.
		}
		if(Input.GetKeyDown(KeyCode.Keypad2)) { // テンキーの2.
			Physics.gravity = Vector3.down; // 重力を下方向にする.
		}


	}
}

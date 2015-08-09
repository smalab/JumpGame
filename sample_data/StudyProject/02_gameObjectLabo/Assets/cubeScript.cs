using UnityEngine;
using System.Collections;

public class cubeScript : MonoBehaviour {

	void Start () {
	}
	void Update () {
	}

	public void bigsize() {
		// x, y, zの全方向について、サイズを3倍にする.
		this.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
	}
}

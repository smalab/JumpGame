using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour {
	
	MicInput micInput;				//マイクの音量を取得するためのオブジェクト.
	public float threshold = 1.0f;	//マイク音量と比べる閾値.

	//生成するオブジェクトを決定します.
	public enum SpawnType {
		Cube,	//キューブ.
		Sphere,	//球.
		Free,	//自分で定義.
	}
	public SpawnType spawnType;

	public GameObject objToSpawn;	//呼び出すオブジェクト.
	Transform obj;					//実際の呼び出しに使用するオブジェクトです.

	void Start () {	
		Init();
	}

	//初期化します.
	void Init() {
		//オブジェクトの検索.
		micInput = GameObject.FindObjectOfType<MicInput>();
		//呼び出すオブジェクトが設定されていなければ.
		if (spawnType == SpawnType.Free && !objToSpawn)
			Debug.LogError("呼び出すオブジェクトが設定されていません!");
	}

	void Update () {		
		SpawnObjs();
	}

	//オブジェクトを呼び出します.
	//Rigidbody + Collider がついているオブジェクトを使えば勝手に反発するので面白いです.
	void SpawnObjs() {
		//何度も呼ぶので始めにキャッシュします.
		float l = micInput.GetLoudness();
		//閾値より大きいので呼び出します.
		if (l > threshold)
		{
			//呼び出すオブジェクトの初期化.
			obj = null;

			//設定したタイプに合わせてオブジェクトを生成します.
			switch(spawnType) {
				case SpawnType.Cube:	obj = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;		break;
				case SpawnType.Sphere:	obj = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;	break;
				case SpawnType.Free:
					//呼び出すオブジェクトが空で無ければ.
					if(objToSpawn) {
						var go = (GameObject)Instantiate(objToSpawn);
						obj = go.transform;
					}
					break;
			}

			//呼び出しに失敗していれば処理を中断します.
			if(!obj) return;
			Debug.Log("call");
			//生成したオブジェクトの位置を自分に合わせます.
			obj.position = transform.position;
			//大きさを初期化します.
			obj.localScale = Vector3.one;

			//音量に合わせて大きさを変更させます.
			Vector3 scale = new Vector3(l/threshold,l/threshold,l/threshold);
			obj.localScale += scale;
			//吹っ飛ぶように,Rigidbodyがなければ付加します.
			if(!obj.GetComponent<Rigidbody>()) obj.gameObject.AddComponent<Rigidbody>();

			//時間経過で勝手に消えるようにします.
			//今回は2秒にしました.
			Destroy(obj.gameObject,2f);
		}
	}
}

using UnityEngine;
using System.Collections;

public class studyScript : MonoBehaviour {


	void Start(){

		// 変数-----------------.
		int hako = 10;


		// 条件分岐---------------.
		if(hako ==10){
			Debug.Log( hako);
		}

		if(hako != 9){
			Debug.Log( hako);
		}

		if(hako > 10){
			Debug.Log( "10より大きい");
		}else if(hako > 5){
			Debug.Log( "5より大きい");
		}else{
			Debug.Log( "5以下");
		}

		switch(hako){
		case 10:
			Debug.Log( "10だね");
			break;
		case 5:
			Debug.Log( "5だね");
			break;
		default:
			Debug.Log( "10と5以外だね");
			break;
		}


		if(hako ==10){
			int memo1 = 30;		// ローカル変数-------.
			Debug.Log( memo1);
		}
		// Debug.Log(memo1);	// ここのコメントアウトを外すとエラーがでる.



		// 配列----------.
		int[] tana = {123, 234, 345, 456, 567};


		// 繰返し文-----------.
		for(int i=0; i<tana.Length; i++){
			Debug.Log( tana[i]);
		}

		// 繰返し文-----------.
		foreach(int i in tana){
			Debug.Log(i);
		}

		hissatuwaza();			// メソッド発動--------.

		int dmg = damage();		// メソッドから返ってきた値を受け取る---------.
		Debug.Log(dmg);

		kaifuku (200);			// メソッドに値を渡す-----------.
	}


	void Update(){
		if(Input.GetKey(KeyCode.Space)){
			Debug.Log( "space");
		}
		if(Input.GetKeyDown(KeyCode.A)){
			Debug.Log( "A");
		}
		if(Input.GetKeyUp(KeyCode.Z)){
			Debug.Log( "Z");
		}

		if(Input.GetMouseButtonDown(0)){
			Debug.Log(Input.mousePosition);
		}
	}


	// 実行するメソッド-----------.
	void hissatuwaza(){
		Debug.Log("体当たり！");
	}


	// 値を返すメソッド-------------.
	int damage(){
		Debug.Log("ダメージ");
		int ret = 100;
		return(ret);
	}

	// 値を受け取るメソッド-------------.
	void kaifuku(int power){
		Debug.Log("回復！" +power);
	}
}

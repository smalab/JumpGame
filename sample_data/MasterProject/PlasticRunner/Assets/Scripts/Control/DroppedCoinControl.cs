using UnityEngine;
using System.Collections;

public class DroppedCoinControl : MonoBehaviour {

	public	MapCreator		map_creator = null;			// マップツクラー.

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
		// 画面からはみ出たら、消す.
		if(this.map_creator.isDelete(this.gameObject)) {

			GameObject.Destroy(this.gameObject);
		}
	}
}

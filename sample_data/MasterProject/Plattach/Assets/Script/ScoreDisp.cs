using UnityEngine;
using System.Collections;

// スコアーの表示.
// ゲーム中、リザルトで共通で使いたいので、
// スクリプトもプレハブも GameRoot から独立しました.
public class ScoreDisp : MonoBehaviour {

	public	Texture[]	number_textures;

	// ================================================================ //

//	private static float	TEXTURE_WIDTH 	= 64;		// テクスチャーの幅.
//	private static float	TEXTURE_HEIGHT	= 64;		// テクスチャーの高さ.

//	private static int		MAX_DIGITS		= 3;		// 最大で表示する桁.


	// ================================================================ //


	// MonoBehaviour からの継承.
	void	Start()
	{
	}
	
	void	Update()
	{
	}
	
	// ================================================================ //

	// 数字を表示する.
	// スコアー用のフォントを使って、数字を表示します.
	public void	dispNumber(Vector2 pos, int number, float font_size, int figre)
	{
		int		i;
		// 現在のスコアーが何桁か調べる.
		int		digits;
		if(number <= 0) {
			// ０以下のときは Log10 が正しく計算できないので.
			number = 0;
			digits = 1;
		} else {
			digits = (int)Mathf.Log10(number) + 1;
		}

		// 表示位置を求める.
		Vector2		p = pos;
		float 	space = font_size *0.7f;


		// p.x += (MAX_DIGITS - 1)* space;
		 p.x += (figre - 1)* space;

		int		n = number;
		// nを１０で割っていくことで、n の１の位が
		//
		// スコアーの１の位
		// スコアーの１０の位
		// スコアーの１００の位
		//		:
		//
		// と変わっていきます.

		for(i = 0;i < digits;i++) {
			// 今回表示する桁の数字（０～９）.
			int		digit = n%10;
			Texture		texture = this.number_textures[digit];
			GUI.DrawTexture(new Rect(p.x, p.y, font_size, font_size), texture);
			p.x -= space;
			n /= 10;
		}
	}





}

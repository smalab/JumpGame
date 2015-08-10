﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item {
	public enum TYPE {
		NONE = -1,
		IRON = 0,		// 鉄鉱石.
		APPLE,			// リンゴ.
		PLANT,			// きゅうり.
		NUM,
	};
};

public class ItemRoot : MonoBehaviour {

	// アイテムのタイプを調べる.
	public Item.TYPE getItemType(GameObject item_go)
	{
		Item.TYPE	type = Item.TYPE.NONE;
		if(item_go != null) {
			switch(item_go.tag) {
			case "Iron":	type = Item.TYPE.IRON;	break;
			case "Apple":	type = Item.TYPE.APPLE;	break;
			case "Plant":	type = Item.TYPE.PLANT;	break;
			}
		}
		return(type);
	}

	// iron_respawn
	protected List<Vector3>	respawn_points;

	public GameObject	ironPrefab = null;
	public GameObject 	plantPrefab = null;
	public GameObject	applePrefab = null;
	
	public float step_timer =0.0f;
	public static float		RESPAWN_TIME_APPLE = 20.0f;
	public static float		RESPAWN_TIME_IRON =  12.0f;
	public static float		RESPAWN_TIME_PLANT =  6.0f;
	private float	respawn_timer_apple = 0.0f;
	private float	respawn_timer_iron	= 0.0f;
	private float	respawn_timer_plant = 0.0f;

	public GameObject 	treePrefab = null;

	void	Start()
	{
		// リスポーンポイントのゲームオブジェクトを探して、座標を配列にしておく.
		this.respawn_points = new List<Vector3>();	// list
		GameObject[] respawns = GameObject.FindGameObjectsWithTag("ItemRespawn");	// ItemRespawn(TAG)
		foreach(GameObject go in respawns) {
			// メッシュは非表示にする.
			MeshRenderer	renderer = go.GetComponentInChildren<MeshRenderer>();
			if(renderer != null) {
				renderer.enabled = false;
			}
			this.respawn_points.Add(go.transform.position);
		}


		// GameObject applerespawn = GameObject.Find("AppleRespawn");
		// applerespawn.GetComponentInChildren<MeshRenderer>().enabled = false;
		GameObject ironrespawn = GameObject.Find("IronRespawn");
		ironrespawn.GetComponentInChildren<MeshRenderer>().enabled = false;

		this.respawnIron();
		this.respawnPlant();
	}


	void Update () {
		// respawn_timer_apple	+= Time.deltaTime;
		respawn_timer_iron	+= Time.deltaTime;
		respawn_timer_plant += Time.deltaTime;

		// リンゴのリボーンはtreeControlに移行しました-----.
		/*
		if(respawn_timer_apple > RESPAWN_TIME_APPLE){
			respawn_timer_apple = 0.0f;
			this.respawnApple();
		}
		*/
		if(respawn_timer_iron > RESPAWN_TIME_IRON){
			respawn_timer_iron = 0.0f;
			this.respawnIron();
		}
		if(respawn_timer_plant > RESPAWN_TIME_PLANT){
			respawn_timer_plant = 0.0f;
			this.respawnPlant();
		}
	}


	// 鉄鉱石リスポーン----------------------------.
	public void respawnIron()
	{
		GameObject iron_go = GameObject.Instantiate(this.ironPrefab) as GameObject;
		Vector3 pos = GameObject.Find("IronRespawn").transform.position;
		pos.y = 1.0f;
		pos.x += Random.Range(-1.0f, 1.0f);
		pos.z += Random.Range(-1.0f, 1.0f);
		iron_go.transform.position = pos;
	}
	
	// きゅうりリスポーン----------------------------.
	public void respawnPlant()
	{
		if(this.respawn_points.Count > 0) {
			GameObject iron_go = GameObject.Instantiate(this.plantPrefab) as GameObject;
			int		n = Random.Range(0, this.respawn_points.Count);
			Vector3 pos = this.respawn_points[n];
			pos.y = 1.0f;
			pos.x += Random.Range(-1.0f, 1.0f);
			pos.z += Random.Range(-1.0f, 1.0f);
			iron_go.transform.position = pos;
		}
	}
	
	// りんごリスポーン------------------------------------------------
	public void respawnApple()
	{
		GameObject iron_go = GameObject.Instantiate(this.applePrefab) as GameObject;
		Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
		pos.y = 1.0f;
		pos.x += Random.Range(-1.0f, 1.0f);
		pos.z += Random.Range(-1.0f, 1.0f);
		iron_go.transform.position = pos;
	}



	// 修理した時に上昇する修理度.
	public float	getGainRepairment(GameObject item_go)
	{
		float	gain = 0.0f;
		if(item_go == null) {
			// 何も持ってない.
			gain = 0.0f;
		} else {
			Item.TYPE	type = this.getItemType(item_go);
			switch(type) {
			case Item.TYPE.IRON:	gain = GameStatus.GAIN_REPARIMENT_IRON;	break;
			case Item.TYPE.PLANT:	gain = GameStatus.GAIN_REPARIMENT_PLANT;	break;
			}
		}
		return(gain);
	}
	
	// アイテムを持ちながら歩いたときに減る満腹度.
	public float	getConsumeSatiety(GameObject item_go)
	{
		float	consume = 0.0f;
		if(item_go == null) {
			// 何も持ってない.
			consume = 0.0f;
		} else {
			Item.TYPE	type = this.getItemType(item_go);
			switch(type) {
			case Item.TYPE.IRON:	consume = GameStatus.CONSUME_SATIETY_IRON;	break;
			case Item.TYPE.APPLE:	consume = GameStatus.CONSUME_SATIETY_APPLE;	break;
			case Item.TYPE.PLANT:	consume = GameStatus.CONSUME_SATIETY_PLANT;	break;
			}
		}
		return(consume);
	}
	
	// 食べたときに回復する満腹度.
	public float	getRegainSatiety(GameObject item_go)
	{
		float	regain = 0.0f;
		if(item_go == null) {
			// 何も持ってない.
			regain = 0.0f;
		} else {
			Item.TYPE	type = this.getItemType(item_go);
			switch(type) {
			case Item.TYPE.APPLE:	regain = GameStatus.REGAIN_SATIETY_APPLE;	break;
			case Item.TYPE.PLANT:	regain = GameStatus.REGAIN_SATIETY_PLANT;	break;
			}
		}
		return(regain);
	}

	// たきびした時に上昇する修理度.
	public float	getRegainFire(GameObject item_go)
	{
		float	regain = 0.0f;
		if(item_go == null) {
			regain = 0.0f;
		} else {
			Item.TYPE	type = this.getItemType(item_go);
			switch(type) {
			case Item.TYPE.APPLE:	regain = GameStatus.REGAIN_FIRE_APPLE;	break;
			case Item.TYPE.PLANT:	regain = GameStatus.REGAIN_FIRE_PLANT;	break;
			}
		}
		return(regain);
	}


	// リンゴを埋めた後、木が生える----------.
	public void plantTree(Vector3 t)
	{
		GameObject go = GameObject.Instantiate(this.treePrefab) as GameObject;
		t.x += Random.Range(-1.0f, 1.0f);
		go.transform.position = t;
	}


}

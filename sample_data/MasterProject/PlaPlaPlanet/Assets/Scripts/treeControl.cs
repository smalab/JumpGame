using UnityEngine;
using System.Collections;

public class treeControl : MonoBehaviour {

	public enum STEP{
		NONE = -1,
		GLOW = 0,
		TREE,
	}
	private STEP step;
	private STEP next_step;


	public GameObject	applePrefab = null;
	public float step_timer =0.0f;
	public static float		RESPAWN_TIME_APPLE = 20.0f;
	public static float		GLOW_TREE_TIME	 = 5.0f;
	private float	respawn_timer_apple = 0.0f;
	private float	glow_timer = 0.0f;

	private float	tree_size = 0.2f;

	void Start () {
		this.next_step = STEP.GLOW;
		this.tree_size = 0.2f;
	}
	
	void Update () {

		this.step_timer += Time.deltaTime;


		// 変化待ち------.
		if(this.next_step == STEP.NONE){
			switch(this.step){
			case STEP.GLOW:
				// ぐんぐん成長する--------.
				glow_timer	+= Time.deltaTime;
				if(glow_timer >= GLOW_TREE_TIME){
					this.next_step = STEP.TREE;
				}
				break;
			}
		}

		// 変化時------.
		while(this.next_step != STEP.NONE) {
			this.step      = this.next_step;
			this.next_step = STEP.NONE;
			switch(this.step){
			case STEP.GLOW:
				break;
			case STEP.TREE:
				break;
			}
		}
		

		// 繰返し----.

		switch(step){
		case STEP.GLOW:
			float size = Mathf.Lerp(0.2f, 0.8f, this.glow_timer/GLOW_TREE_TIME);
			this.transform.localScale = Vector3.one*size;
			break;

		case STEP.TREE:
			// 定期的にリンゴを作り出す--------.
			respawn_timer_apple	+= Time.deltaTime;
			if(respawn_timer_apple > RESPAWN_TIME_APPLE){
				respawn_timer_apple = 0.0f;
				this.respawnApple();
			}
			break;
		}


	}


	// りんごリスポーン------------------------------------------------.
	public void respawnApple()
	{
		GameObject iron_go = GameObject.Instantiate(this.applePrefab) as GameObject;
		// Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
		Vector3 pos = this.gameObject.transform.position;
		pos.y = 0.5f;
		pos.x += Random.Range(-1.0f, 1.0f);
		pos.z += Random.Range(-1.0f, 1.0f);
		iron_go.transform.position = pos;
	}
}

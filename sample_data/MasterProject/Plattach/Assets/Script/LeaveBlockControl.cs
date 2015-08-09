using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaveBlockControl : MonoBehaviour {

	public	Block.COLOR		color = (Block.COLOR)0;

	public	LeaveBlockRoot	leave_block_root = null;

	public	Block.iPosition		i_pos;

	private	GameObject		models_root;	// ブロックモデルたちの親.
	private	GameObject[]	models;			// 各色のブロックのモデル.

	private static float	DISP_AREA_SIDE   =  6.0f;		// 画面に見える範囲の横はし.
	private static float	DISP_AREA_BOTTOM = -8.0f;		// 画面に見える範囲の下はし.

	public	Block.iPosition[]		connected_block;	// となりのブロック（同じ色のときのみ）.

	private	Animation	neko_motion = null;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Awake()
	{
		// 各色のブロックのモデルを探しておく.

		this.models = new GameObject[(int)Block.COLOR.NORMAL_COLOR_NUM];

		this.models_root = this.transform.FindChild("models").gameObject;

		this.models[(int)Block.COLOR.PINK]    = this.models_root.transform.FindChild("block_pink").gameObject;
		this.models[(int)Block.COLOR.BLUE]    = this.models_root.transform.FindChild("block_blue").gameObject;
		this.models[(int)Block.COLOR.GREEN]   = this.models_root.transform.FindChild("block_green").gameObject;
		this.models[(int)Block.COLOR.ORANGE]  = this.models_root.transform.FindChild("block_orange").gameObject;
		this.models[(int)Block.COLOR.YELLOW]  = this.models_root.transform.FindChild("block_yellow").gameObject;
		this.models[(int)Block.COLOR.MAGENTA] = this.models_root.transform.FindChild("block_purple").gameObject;
		this.models[(int)Block.COLOR.NECO]    = this.models_root.transform.FindChild("neco").gameObject;

		// 非表示にするととれなくなるので注意.
		this.neko_motion = this.models[(int)Block.COLOR.NECO].GetComponentInChildren<Animation>();

		// いったん全部非表示.
		for(int i = 0;i < this.models.Length;i++) {

			this.models[i].SetActive(false);
		}

		// この色のブロックだけ表示する.
		this.setColor(this.color);

		this.connected_block = new Block.iPosition[(int)Block.DIR4.NUM];

		for(int i = 0;i < this.connected_block.Length;i++) {

			this.connected_block[i].clear();
		}
	}

	void 	Start()
	{
	}
	void	Update ()
	{
	}

	// ================================================================ //
	
	// ブロックの色をセットする.
	public void		setColor(Block.COLOR color)
	{
		this.color = color;

		if(this.models != null) {

			foreach(var model in this.models) {
	
				model.SetActive(false);
			}
	
			switch(this.color) {
	
				case Block.COLOR.PINK:
				case Block.COLOR.BLUE:
				case Block.COLOR.YELLOW:
				case Block.COLOR.GREEN:
				case Block.COLOR.MAGENTA:
				case Block.COLOR.ORANGE:
				case Block.COLOR.NECO:
				{
					this.models[(int)this.color].SetActive(true);
				}
				break;
			}
		}
	}

	// となりのブロックをセットする（同じ色のときのみ）.
	public void		setConnectedBlock(Block.DIR4 dir, Block.iPosition connected)
	{
		this.connected_block[(int)dir] = connected;
	}

	// となりのブロックをゲットする（同じ色のときのみ）.
	public Block.iPosition	getConnectedBlock(Block.DIR4 dir)
	{
		return(this.connected_block[(int)dir]);
	}

	// ModelRoot（各色ブロックモデルの親）をゲットする.
	public GameObject	getModelsRoot()
	{
		return(this.models_root);
	}

	public Animation	getNecoMotion()
	{
		return(this.neko_motion);
	}

	public void		setNecoRotation(Vector3 forward)
	{
		if(this.color == Block.COLOR.NECO) {

			Quaternion	rot = this.models_root.transform.rotation;

			rot = Quaternion.Lerp(rot, Quaternion.LookRotation(-forward), 0.05f);

			this.models_root.transform.rotation = rot;
		}
	}

	// ================================================================ //

	// ブロックが表示範囲（画面内）にいる？.
	public	bool	isInDispArea()
	{
		bool	ret = false;

		do {

			if(this.transform.position.x < -DISP_AREA_SIDE || DISP_AREA_SIDE < this.transform.position.x) {

				break;
			}
			if(this.transform.position.y < DISP_AREA_BOTTOM) {

				break;
			}

			ret = true;

		} while(false);

		return(ret);
	}
}

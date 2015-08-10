using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaveBlockIsland : MonoBehaviour {

	private List<LeaveBlockControl>		blocks = null;

	private class Chain {

		public Vector3	move_direction;
		public Vector3	velocity;
		public float	omega;
	};
	private Chain		chain;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void 	Awake()
	{
		this.blocks = new List<LeaveBlockControl>();
	}

	void	Start()
	{
		this.chain_initialize();
	}

	void	Update()
	{
		do {

			if(!this.is_in_range()) {

				GameObject.Destroy(this.gameObject);
				break;
			}

			this.chain_execute();

		} while(false);
	}

	// 画面内に入ってる？.
	private bool	is_in_range()
	{
		bool	is_in_range = false;

		foreach(LeaveBlockControl block in this.blocks) {

			if(block.isInDispArea()) {

				is_in_range = true;
				break;
			}
		}

		return(is_in_range);
	}
	
	// ================================================================ //

	// 初期化.
	private void	chain_initialize()
	{
		do {

			LeaveBlockControl[]	children = this.GetComponentsInChildren<LeaveBlockControl>();

			if(children.Length == 0) {

				break;
			}

			Block.DIR4		dir = this.select_move_dir(children);

			this.chain = new Chain();

			// 先頭のブロックをきめる.
			for(int j = 0;j < children.Length;j++) {
	
				int		sel = -1;
	
				for(int i = 0;i < children.Length;i++) {
		
					if(children[i] == null) {
		
						continue;
					}

					bool	sw = false;

					if(sel == -1) {

						sw = true;

					} else {

						switch(dir) {

							case Block.DIR4.RIGHT:	sw = (children[i].i_pos.x > children[sel].i_pos.x);	break;
							case Block.DIR4.LEFT:	sw = (children[i].i_pos.x < children[sel].i_pos.x);	break;
							case Block.DIR4.UP:		sw = (children[i].i_pos.y > children[sel].i_pos.y);	break;
							case Block.DIR4.DOWN:	sw = (children[i].i_pos.y < children[sel].i_pos.y);	break;
						}
					}

					if(sw) {

						sel = i;
					}
				}
	
				if(sel == -1) {
	
					break;
				}
	
				this.blocks.Add(children[sel]);
				children[sel] = null;
			}

			switch(dir) {

				case Block.DIR4.RIGHT:	this.chain.move_direction = Vector3.right;	break;
				case Block.DIR4.LEFT:	this.chain.move_direction = Vector3.left;	break;
				case Block.DIR4.UP:		this.chain.move_direction = Vector3.up;		break;
				case Block.DIR4.DOWN:	this.chain.move_direction = Vector3.down;	break;
			}

			this.chain.velocity = this.chain.move_direction*2.0f;
			this.chain.omega    = 0.0f;

		} while(false);
	}

	// 毎フレームの実行.
	private void	chain_execute()
	{
		// 先頭のブロック.

		this.chain.velocity += this.chain.move_direction*4.0f*Time.deltaTime;
		this.chain.omega    += 360.0f*Time.deltaTime;

		LeaveBlockControl	trailer = this.blocks[0];

		trailer.transform.Translate(this.chain.velocity*Time.deltaTime);

		if(trailer.color == Block.COLOR.NECO) {

			trailer.setNecoRotation(this.chain.velocity);

		} else {

			trailer.getModelsRoot().transform.Rotate(Vector3.up, this.chain.omega*Time.deltaTime);
		}

		// ２番目以降のブロック.
		// 前のブロックにひっぱられるように移動する.

		float	omega = this.chain.omega*0.75f;

		float	prev_velocity = this.chain.velocity.magnitude;

		for(int i = 1;i < this.blocks.Count;i++) {

			LeaveBlockControl	prev = this.blocks[i - 1];
			LeaveBlockControl	crnt = this.blocks[i];

			// 位置移動.

			Vector3		distance_vector = crnt.transform.position - prev.transform.position;

			float		distance_limit = 1.0f;
			float		distance       = distance_vector.magnitude;

			if(distance > distance_limit) {

				float		velocity = prev_velocity + (distance - distance_limit)*0.1f;
	
				if(distance - velocity*Time.deltaTime < distance_limit) {
	
					velocity = (distance - distance_limit)/Time.deltaTime;
				}
	
				distance -= velocity*Time.deltaTime;
	
				distance_vector.Normalize();
				distance_vector *= distance;
	
				crnt.transform.position = prev.transform.position + distance_vector;

				prev_velocity = velocity;
			}

			// 回転.
			if(trailer.color == Block.COLOR.NECO) {

				crnt.setNecoRotation(-distance_vector);

			} else {

				crnt.getModelsRoot().transform.Rotate(Vector3.up, omega*Time.deltaTime);
			}

			omega *= 0.75f;
		}
	}

	// ブロックが退場する方向を決める.
	private Block.DIR4	select_move_dir(LeaveBlockControl[]	children)
	{
		Block.DIR4	dir = Block.DIR4.RIGHT;

		float		cx = 0.0f;

		foreach(LeaveBlockControl block in children) {

			cx += (float)block.i_pos.x;
		}

		cx /= (float)children.Length;

		if(cx < (float)Block.BLOCK_NUM_X/2) {

			dir = Block.DIR4.LEFT;

		} else {

			dir = Block.DIR4.RIGHT;
		}

		return(dir);
	}

	// ================================================================ //
}

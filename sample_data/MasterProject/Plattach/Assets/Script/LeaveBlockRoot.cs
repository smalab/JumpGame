using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaveBlockRoot : MonoBehaviour {

	public	GameObject		blockPrefab      = null;

	public	BlockRoot		block_root = null;

	// ================================================================ //
	// MonoBehaviour からの継承.

	void	Start()
	{
	}
	
	void	Update()
	{
		// 『退場まちブロックリスト』の更新.
		this.process_waiting_queue();
	}

	// 『退場まちブロックリスト』の更新.
	public void		process_waiting_queue()
	{
		List<LeaveBlockControl>		blocks = new List<LeaveBlockControl>();

		bool		is_found = false;

		for(int i = 0;i < this.waiting_blocks.Count;i++) {

			foreach(LeaveBlockControl waiting_block in this.waiting_blocks) {
	
				blocks.Clear();
	
				if(!this.check_ready_to_leave(waiting_block, blocks)) {
	
					continue;
				}

				// しまのブロックが全部そろったら、退場演出をはじめる.
	
				GameObject	go = new GameObject("island");

				LeaveBlockIsland	island = go.AddComponent<LeaveBlockIsland>();

				foreach(LeaveBlockControl block in blocks) {

					block.transform.parent = island.transform;

					if(block.color == Block.COLOR.NECO) {

						block.getNecoMotion().CrossFade("10_Move", 0.1f);
					}

					this.waiting_blocks.Remove(block);
				}

				is_found = true;
				break;
			}

			if(!is_found) {

				break;
			}
		}
	}

	// ================================================================ //

	public List<LeaveBlockControl>	waiting_blocks = new List<LeaveBlockControl>();

	// 退場演出用ブロックをつくる.
	public LeaveBlockControl		createLeaveBlock(BlockControl block)
	{
		GameObject 		game_object = Instantiate(this.blockPrefab) as GameObject;

		LeaveBlockControl	leave_block = game_object.GetComponent<LeaveBlockControl>();

		//
		leave_block.leave_block_root = this;
		leave_block.i_pos = block.i_pos;
		leave_block.transform.position = block.transform.position;
		leave_block.getModelsRoot().transform.localScale    = block.getModelsRoot().transform.localScale;
		leave_block.getModelsRoot().transform.localPosition = block.getModelsRoot().transform.localPosition;
		leave_block.getModelsRoot().transform.localRotation = block.getModelsRoot().transform.localRotation;

		leave_block.setColor(block.color);

		if(block.color == Block.COLOR.NECO) {

			leave_block.getNecoMotion()["00_Idle"].time = block.getNekoMotion()["00_Idle"].time;
		}

		for(int i = 0;i < (int)Block.DIR4.NUM;i++) {

			Block.DIR4	dir = (Block.DIR4)i;

			leave_block.setConnectedBlock(dir, block.getConnectedBlock(dir));
		}

		// ヒエラルキービューで位置が確認しやすいよう、ブロックの座標を名前につけておく.
		leave_block.name = "leave_block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";

		// しま（同じ色のブロックが隣あっているカタマリ）のブロックが
		// 全部そろうまでまつ.
		this.waiting_blocks.Add(leave_block);

		return(leave_block);
	}

	// しまのブロックがぜんぶそろったか調べる.
	private bool	check_ready_to_leave(LeaveBlockControl block, List<LeaveBlockControl> blocks)
	{
		bool	ready = true;

		do {

			if(blocks.Contains(block)) {
	
				break;
			}
	
			blocks.Add(block);
	
			foreach(Block.iPosition ipos in block.connected_block) {
	
				if(!ipos.isValid()) {
	
					continue;
				}
	
				LeaveBlockControl	next_block = this.waiting_blocks.Find(x => x.i_pos.Equals(ipos));
	
				if(next_block == null) {
	
					ready = false;
					break;
				}
	
				if(!this.check_ready_to_leave(next_block, blocks)) {
		
					ready = false;
					break;
				}
			}

		} while(false);

		return(ready);
	}

}

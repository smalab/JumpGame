﻿using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour
{
    public MapCreator map_creator = null;

    void Start()
    {
        map_creator = GameObject.Find("GameRoot").GetComponent<MapCreator>();
    }

    void Update()
    {
        //画面外に出たブロックを削除
        if (this.map_creator.isDelete(this.gameObject))
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
